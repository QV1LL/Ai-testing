const HeroSection: React.FC = () => (
  <section className="hero">
    <div className="hero-content">
      <h1>
        Welcome to <span translate="no">AI Testing</span>
      </h1>
      <p>
        Create, manage, and take AI-powered tests. Smart. Reliable. Accessible.
      </p>
      <div className="quick-actions">
        <button
          className="primary-btn"
          onClick={() => (window.location.href = "/create-test")}
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
