import React from "react";
import { Routes, Route } from "react-router-dom";
import HomePage from "../pages/HomePage/HomePage";
import AboutPage from "../pages/AboutPage/AboutPage";
import DocumentationPage from "../pages/DocumentationPage/DocumentationPage";
import PrivacyPolicyPage from "../pages/PrivacyPolicyPage/PrivacyPolicyPage";
import ProfilePage from "../pages/ProfilePage/ProfilePage";
import MyTestsPage from "../pages/MyTestsPage/MyTestsPage";
import NotFoundPage from "../pages/NotFoundPage/NotFoundPage";
import PrivateRoute from "../routes/PrivateRoute";
import AuthPage from "../pages/AuthPage/AuthPage";
import CreateTestPage from "../pages/CreateTestPage/CreateTestPage";

const AppRoutes: React.FC = () => {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/auth" element={<AuthPage />} />
      <Route path="/about" element={<AboutPage />} />
      <Route path="/docs" element={<DocumentationPage />} />
      <Route path="/privacy" element={<PrivacyPolicyPage />} />

      {/* Захищені сторінки */}
      <Route
        path="/profile"
        element={
          <PrivateRoute>
            <ProfilePage />
          </PrivateRoute>
        }
      />
      <Route
        path="/tests"
        element={
          <PrivateRoute>
            <MyTestsPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/tests/create"
        element={
          <PrivateRoute>
            <CreateTestPage />
          </PrivateRoute>
        }
      />

      {/* Not found page */}
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
};

export default AppRoutes;
