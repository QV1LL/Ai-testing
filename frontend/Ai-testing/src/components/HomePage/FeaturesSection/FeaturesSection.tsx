import styles from "./FeaturesSection.module.css";

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
  <section className={styles.features}>
    <h2>{title}</h2>
    <div className={styles.featuresGrid}>
      {features.map((f, i) => (
        <div className={styles.featureCard} key={i}>
          <h3>{f.title}</h3>
          <p>{f.description}</p>
        </div>
      ))}
    </div>
  </section>
);

export default FeaturesSection;
