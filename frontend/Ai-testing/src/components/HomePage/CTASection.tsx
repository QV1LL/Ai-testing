const CTASection: React.FC = () => (
  <section className="cta">
    <h2>Ready to Get Started?</h2>
    <p>
      Join hundreds of professionals using AI Testing to evaluate skills faster
      and smarter.
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
