import styles from "./CTASection.module.css";

const CTASection: React.FC = () => (
  <section className={styles.cta}>
    <h2>Ready to Get Started?</h2>
    <p>
      Join hundreds of professionals using{" "}
      <span translate="no">AI Testing</span>
    </p>
    <button
      className="primary-btn"
      onClick={() => (window.location.href = "/create-test")}
    >
      Start Now
    </button>
  </section>
);

export default CTASection;
