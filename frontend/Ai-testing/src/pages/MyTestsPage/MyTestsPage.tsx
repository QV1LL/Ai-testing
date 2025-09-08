import React, { useEffect, useState } from "react";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import styles from "./MyTestsPage.module.css";
import { getUserTests } from "../../api/testService";
import type { TestMetadataDto } from "../../types/test";
import { useNavigate } from "react-router-dom";

const MyTestsPage: React.FC = () => {
  const [search, setSearch] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [allTests, setAllTests] = useState<TestMetadataDto[]>([]);
  const testsPerPage = 9;
  const navigate = useNavigate();

  useEffect(() => {
    const fetchTests = async () => {
      const data = await getUserTests();
      console.log("allTests:", data);
      setAllTests(data);
    };

    fetchTests();
  }, []);

  const filteredTests = allTests.filter((t) =>
    t.title.toLowerCase().includes(search.toLowerCase())
  );

  const indexOfLast = currentPage * testsPerPage;
  const indexOfFirst = indexOfLast - testsPerPage;
  const currentTests = filteredTests.slice(indexOfFirst, indexOfLast);

  const totalPages = Math.ceil(filteredTests.length / testsPerPage);

  return (
    <div className={styles.myTestsPage}>
      <Header />
      <div className={`${styles.filterWrapper} ${styles.coloredBackground}`}>
        <div className={styles.filterContainer}>
          <div className={styles.filters}>
            <input
              type="text"
              placeholder="Search tests..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
        </div>
      </div>
      <div className={styles.wrapper}>
        <div className={styles.container}>
          <div className={styles.testsList}>
            {currentTests.length > 0 ? (
              currentTests.map((t) => (
                <div
                  key={t.id}
                  className={styles.testCard}
                  onClick={() => navigate(`/tests/view/${t.id}`)}
                  style={{ cursor: "pointer" }}
                >
                  <h3>{t.title}</h3>
                  <p>
                    {t.description === null ? "no description" : t.description}
                  </p>
                </div>
              ))
            ) : (
              <p className={styles.testsPlaceholder}>No tests available yet.</p>
            )}
          </div>

          <div className={styles.pagination}>
            {Array.from({ length: totalPages }, (_, i) => (
              <button
                key={i}
                className={`${styles.pageBtn} ${
                  currentPage === i + 1 ? styles.active : ""
                }`}
                onClick={() => setCurrentPage(i + 1)}
              >
                {i + 1}
              </button>
            ))}
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default MyTestsPage;
