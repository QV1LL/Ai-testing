import styles from "./HeroSection.module.css";

const HeroSection: React.FC = () => (
  <section className={styles.hero}>
    <div className={styles.heroContent}>
      <h1>
        Welcome to <span translate="no"> AI Testing</span>
      </h1>
      <p>
        Create, manage, and take AI-powered tests. Smart. Reliable. Accessible.
      </p>
      <div className={styles.quickActions}>
        <button
          className="primary-btn"
          onClick={() => (window.location.href = "/tests/create")}
        >
          Create Test
        </button>
        <button
          className="secondary-btn"
          onClick={() => (window.location.href = "/tests")}
        >
          View Tests
        </button>
      </div>
    </div>
  </section>
);

export default HeroSection;
