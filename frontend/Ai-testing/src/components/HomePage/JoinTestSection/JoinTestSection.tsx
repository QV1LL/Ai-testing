import styles from "./JoinTestSection.module.css";

interface JoinTestSectionProps {
  testId: string;
  onChange: (value: string) => void;
  onJoin: () => void;
}

const JoinTestSection: React.FC<JoinTestSectionProps> = ({
  testId,
  onChange,
  onJoin,
}) => (
  <section className={styles.joinTest}>
    <h2>Join a Test</h2>
    <div className={styles.joinForm}>
      <input
        type="text"
        placeholder="Enter Test ID"
        value={testId}
        onChange={(e) => onChange(e.target.value)}
      />
      <button className="primary-btn" onClick={onJoin}>
        Start
      </button>
    </div>
  </section>
);

export default JoinTestSection;
