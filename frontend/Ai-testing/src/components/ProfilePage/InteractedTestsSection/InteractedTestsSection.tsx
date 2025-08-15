import React from "react";
import styles from "./InteractedTestsSection.module.css";

interface Test {
  id: string;
  title: string;
  date: string;
  score?: number;
}

interface InteractedTestsSectionProps {
  createdTests?: Test[];
  passedTests?: Test[];
}

const InteractedTestsSection: React.FC<InteractedTestsSectionProps> = ({
  createdTests = [],
  passedTests = [],
}) => {
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
                  <td>{test.date}</td>
                  <td>
                    <button className="secondary-btn">Edit</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No tests created yet.</p>
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
                  <td>{test.date}</td>
                  <td>{test.score ?? "-"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No tests passed yet.</p>
        )}
      </div>
    </div>
  );
};

export default InteractedTestsSection;
