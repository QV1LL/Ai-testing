import React from "react";
import { Routes, Route } from "react-router-dom";
import HomePage from "../pages/HomePage/HomePage";
import AboutPage from "../pages/AboutPage/AboutPage";
import DocumentationPage from "../pages/DocumantationPage/DocumantationPage";
import PrivacyPolicyPage from "../pages/PrivacyPolicyPage/PrivacyPolicyPage";
import ProfilePage from "../pages/ProfilePage/ProfilePage";
import MyTestsPage from "../pages/MyTestsPage/MyTestsPage";
import NotFoundPage from "../pages/NotFoundPage/NotFoundPage";

const AppRoutes: React.FC = () => {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/about" element={<AboutPage />} />
      <Route path="/docs" element={<DocumentationPage />} />
      <Route path="/privacy" element={<PrivacyPolicyPage />} />
      <Route path="/profile" element={<ProfilePage />} />
      <Route path="/tests" element={<MyTestsPage />} />
      {/* Not found page*/}
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
};

export default AppRoutes;
