import React, { useState } from "react";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import styles from "./MyTestsPage.module.css";

interface Test {
  id: string;
  title: string;
  description: string;
}

const MyTestsPage: React.FC = () => {
  const [search, setSearch] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const testsPerPage = 9;

  const allTests: Test[] = [
    { id: "1", title: "React Basics", description: "Learn React fundamentals" },
    { id: "2", title: "TypeScript Advanced", description: "Deep dive into TS" },
    { id: "3", title: "JavaScript Quiz", description: "Test JS knowledge" },
    { id: "4", title: "HTML & CSS", description: "Build static pages" },
    { id: "5", title: "Node.js Intro", description: "Backend basics" },
    { id: "6", title: "Redux", description: "State management" },
    { id: "7", title: "Unit Testing", description: "Learn testing in JS" },
  ];

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
            {currentTests.map((t) => (
              <div key={t.id} className={styles.testCard}>
                <h3>{t.title}</h3>
                <p>{t.description}</p>
              </div>
            ))}
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
