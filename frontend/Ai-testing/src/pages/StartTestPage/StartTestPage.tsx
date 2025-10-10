import { useNavigate, useParams } from "react-router-dom";
import styles from "./StartTestPage.module.css";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { useEffect, useState } from "react";
import type { TestPreviewDto } from "../../types/test";
import { getTestPreview } from "../../api/testService";
import { getAccessToken } from "../../api/api";
import { jwtDecode } from "jwt-decode";
import { createAttempt } from "../../api/testAttemptService";
import type { AddTestAttemptDto } from "../../types/testAttempt";

const StartTestPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [test, setTest] = useState<TestPreviewDto | null>(null);
  const [testFounded, setTestFounded] = useState<boolean | null>(null);

  const [loggedUserId, setLoggedUserId] = useState<string | null>(null);
  const [loggedUserName, setLoggedUserName] = useState<string | null>(null);
  const [guestName, setGuestName] = useState<string>();

  useEffect(() => {
    const fetchTest = async () => {
      if (!id) return;
      try {
        const data = await getTestPreview(id);
        setTest(data);

        if (data !== null) setTestFounded(true);
      } catch (error) {
        console.error(error);
        setTestFounded(false);
      }
    };

    const fetchUserMetadata = () => {
      try {
        const accessToken = getAccessToken();
        const decoded: any = jwtDecode(accessToken ?? "");
        setLoggedUserName(decoded.name ?? null);
        setLoggedUserId(decoded.sub ?? null);
      } catch {}
    };

    fetchTest();
    fetchUserMetadata();
  }, [id]);

  const HandleStartTestAttempt = async () => {
    if (test === null) return;

    const dto: AddTestAttemptDto = {
      testId: id ?? "",
      userId: loggedUserId || undefined,
      guestName: guestName === null ? undefined : guestName,
      startedAt: new Date(),
    };

    const attemptAddResultDto = await createAttempt(dto);
    const attemptId = attemptAddResultDto.attemptId;

    navigate(`/pass-test/attempt/${attemptId}`);
  };

  if (testFounded === null)
    return <div className={styles.loader}>Loading...</div>;

  return (
    <div className={styles.passTestPage}>
      <Header />
      <div className={styles.wrapper}>
        <div className={styles.container}>
          {testFounded && test ? (
            <>
              <div
                className={styles.header}
                style={{
                  backgroundImage: test.coverImageUrl
                    ? `url(${test.coverImageUrl})`
                    : "linear-gradient(135deg, #1e3c72, #2a5298)",
                }}
              >
                <div className={styles.overlay}>
                  <h1>{test.title}</h1>
                  {test.description && <p>{test.description}</p>}
                </div>

                <div className={styles.overlayFooter}>
                  <button
                    className={styles.startButton}
                    onClick={HandleStartTestAttempt}
                  >
                    <div className={styles.startButtonContent}>
                      <p>Start test as</p>
                      {""}
                      <p translate="no">
                        {loggedUserName ? loggedUserName : guestName}
                      </p>
                    </div>
                  </button>
                </div>
              </div>
              <div className={styles.stats}>
                {!loggedUserName && (
                  <input
                    type="text"
                    value={guestName}
                    placeholder="Guest name"
                    onChange={(e) => setGuestName(e.target.value)}
                    className={styles.titleInput}
                    required
                  />
                )}
                <p>
                  Time limit:{" "}
                  {test.timeLimitMinutes
                    ? test.timeLimitMinutes
                    : "No time limit"}
                </p>
              </div>
            </>
          ) : (
            <div className={styles.notFoundCard}>
              <h2>Test not available</h2>
              <p>
                This test does not exist or is private. Please check the link or
                contact the author.
              </p>
            </div>
          )}
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default StartTestPage;
