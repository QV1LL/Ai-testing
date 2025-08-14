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
  <section className="join-test">
    <h2>Join a Test</h2>
    <div className="join-form">
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
