import React from "react";
import "./DocumantationPage.css";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";

const DocumentationPage: React.FC = () => {
  return (
    <div>
      <Header />
      <div className="page-container">
        <div className="content-container">
          <h2>Getting Started</h2>
          <p>1. Log in or register for an account.</p>
          <p>2. Click "Create Test" to design a new AI-powered test.</p>
          <p>3. Share the test link or ID with participants.</p>
          <h2>Managing Tests</h2>
          <p>
            View, edit, or delete your tests from the dashboard. Analyze results
            instantly after each test.
          </p>
          <h2>API & Advanced Features</h2>
          <p>
            For developers, API documentation and integration examples are
            provided here.
          </p>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default DocumentationPage;
