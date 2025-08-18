import { useState } from "react";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import HeroSection from "../../components/HomePage/HeroSection/HeroSection";
import JoinTestSection from "../../components/HomePage/JoinTestSection/JoinTestSection";
import FeaturesSection from "../../components/HomePage/FeaturesSection/FeaturesSection";
import CTASection from "../../components/HomePage/CTASection/CTASection";
import styles from "./HomePage.module.css";

const HomePage: React.FC = () => {
  const [testId, setTestId] = useState<string>("");

  const handleJoinTest = () => {
    if (testId.trim()) {
      window.location.href = `/test/${testId}`;
    }
  };

  return (
    <>
      <Header />

      <HeroSection />
      <div className={styles.wrapper}>
        <div className={styles.homeContainer}>
          <JoinTestSection
            testId={testId}
            onChange={setTestId}
            onJoin={handleJoinTest}
          />
          <FeaturesSection
            title="Why Choose us?"
            features={[
              {
                title: "Test analysis",
                description:
                  "Get instant insights and performance analytics after every test.",
              },
              {
                title: "Secure & Reliable",
                description:
                  "Your data is safe, and tests are protected from unauthorized access.",
              },
              {
                title: "Easy to Use",
                description:
                  "Create and share tests in minutes with an intuitive interface.",
              },
            ]}
          />
          <FeaturesSection
            title="How It Works"
            features={[
              {
                title: "1. Create or Select",
                description:
                  "Choose from existing tests or create a new one in minutes.",
              },
              {
                title: "2. Share",
                description:
                  "Send the test link or ID to participants securely.",
              },
              {
                title: "3. Analyze Results",
                description: "Get detailed insights to evaluate performance.",
              },
            ]}
          />
        </div>
      </div>
      <CTASection />
      <Footer />
    </>
  );
};

export default HomePage;
