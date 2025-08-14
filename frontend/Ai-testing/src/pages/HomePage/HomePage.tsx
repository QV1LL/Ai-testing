import { useState } from "react";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import HeroSection from "../../components/HomePage/HeroSection";
import JoinTestSection from "../../components/HomePage/JoinTestSection";
import FeaturesSection from "../../components/HomePage/FeaturesSection";
import CTASection from "../../components/HomePage/CTASection";
import "./HomePage.css";

const HomePage: React.FC = () => {
  const [testId, setTestId] = useState<string>("");

  const handleJoinTest = () => {
    if (testId.trim()) {
      window.location.href = `/test/${testId}`;
    }
  };

  return (
    <div className="home-container">
      <Header />
      <HeroSection />
      <JoinTestSection
        testId={testId}
        onChange={setTestId}
        onJoin={handleJoinTest}
      />
      <FeaturesSection
        title="Why Choose AI Testing?"
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
            description: "Send the test link or ID to participants securely.",
          },
          {
            title: "3. Analyze Results",
            description: "Get detailed insights to evaluate performance.",
          },
        ]}
      />
      <CTASection />
      <Footer />
    </div>
  );
};

export default HomePage;
