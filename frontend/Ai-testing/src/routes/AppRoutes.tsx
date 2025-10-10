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
import EditTestPage from "../pages/EditTestPage/EditTestPage";
import ViewTestPage from "../pages/ViewTestPage/ViewTestPage";
import StartTestPage from "../pages/StartTestPage/StartTestPage";
import PassTestAttemptPage from "../pages/PassTestAttemptPage/PassTestAttemptPage";
import AttemptByIdPage from "../pages/AttemptByIdPage/AttemptByIdPage";
import ViewTestAttemptResults from "../pages/ViewTestAttemptResults/ViewTestAttemptsResults";

const AppRoutes: React.FC = () => {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/auth" element={<AuthPage />} />
      <Route path="/about" element={<AboutPage />} />
      <Route path="/docs" element={<DocumentationPage />} />
      <Route path="/privacy" element={<PrivacyPolicyPage />} />
      <Route path="/attempt" element={<AttemptByIdPage />} />
      <Route path="/test-attempt/result" element={<ViewTestAttemptResults />} />
      <Route path="/pass-test/:id" element={<StartTestPage />} />
      <Route path="/pass-test/attempt/:id" element={<PassTestAttemptPage />} />

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
      <Route
        path="/tests/edit/:id"
        element={
          <PrivateRoute>
            <EditTestPage />
          </PrivateRoute>
        }
      />
      <Route
        path="/tests/view/:id"
        element={
          <PrivateRoute>
            <ViewTestPage />
          </PrivateRoute>
        }
      />

      {/* Not found page */}
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
};

export default AppRoutes;
