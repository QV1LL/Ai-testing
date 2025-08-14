interface Feature {
  title: string;
  description: string;
}

interface FeaturesSectionProps {
  title: string;
  features: Feature[];
}

const FeaturesSection: React.FC<FeaturesSectionProps> = ({
  title,
  features,
}) => (
  <section className="features">
    <h2>{title}</h2>
    <div className="features-grid">
      {features.map((f, i) => (
        <div className="feature-card" key={i}>
          <h3>{f.title}</h3>
          <p>{f.description}</p>
        </div>
      ))}
    </div>
  </section>
);

export default FeaturesSection;
