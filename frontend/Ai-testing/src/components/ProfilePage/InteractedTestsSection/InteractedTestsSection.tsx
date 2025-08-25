import React from "react";
import styles from "./InteractedTestsSection.module.css";
import type { TestAttemptsInfo, TestInfo } from "../../../types/profile";
import { useNavigate } from "react-router-dom";

interface InteractedTestsSectionProps {
  createdTests?: TestInfo[];
  passedTests?: TestAttemptsInfo[];
}

const InteractedTestsSection: React.FC<InteractedTestsSectionProps> = ({
  createdTests = [],
  passedTests = [],
}) => {
  const navigate = useNavigate();

  const formatLocalDateTime = (dateInput: string | Date) => {
    const date =
      typeof dateInput === "string" ? new Date(dateInput) : dateInput;
    return `${String(date.getDate()).padStart(2, "0")}.${String(
      date.getMonth() + 1
    ).padStart(2, "0")}.${date.getFullYear()} ${String(
      date.getHours()
    ).padStart(2, "0")}:${String(date.getMinutes()).padStart(2, "0")}`;
  };

  return (
    <div className={styles.testsSection}>
      {/* Created Tests */}
      <div className={styles.testTableWrapper}>
        <h3>Created Tests</h3>
        {createdTests.length > 0 ? (
          <table className={styles.testTable}>
            <thead>
              <tr>
                <th>Title</th>
                <th>Date Created</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {createdTests.map((test) => (
                <tr key={test.id}>
                  <td>{test.title}</td>
                  <td>{formatLocalDateTime(test.createdAt)}</td>
                  <td>
                    <button
                      className="secondary-btn"
                      onClick={() => navigate(`/tests/view/${test.id}`)}
                    >
                      View
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p className={styles.testsPlaceholder}>No tests created yet.</p>
        )}
      </div>

      {/* Passed Tests */}
      <div className={styles.testTableWrapper}>
        <h3>Passed Tests</h3>
        {passedTests.length > 0 ? (
          <table className={styles.testTable}>
            <thead>
              <tr>
                <th>Title</th>
                <th>Date Taken</th>
                <th>Score</th>
              </tr>
            </thead>
            <tbody>
              {passedTests.map((test) => (
                <tr key={test.id}>
                  <td>{test.title}</td>
                  <td>{formatLocalDateTime(test.startedAt)}</td>
                  <td>{test.score ?? "-"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p className={styles.testsPlaceholder}>No tests passed yet.</p>
        )}
      </div>
    </div>
  );
};

export default InteractedTestsSection;
