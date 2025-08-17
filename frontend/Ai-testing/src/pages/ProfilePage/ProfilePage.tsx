import React, { useState } from "react";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import styles from "./ProfilePage.module.css";
import ProfileCardSection from "../../components/ProfilePage/ProfileCardSection/ProfileCardSection";
import InteractedTestsSection from "../../components/ProfilePage/InteractedTestsSection/InteractedTestsSection";
import EditUserModal from "../../components/EditUserModal/EditUserModal";

const ProfilePage: React.FC = () => {
  const [showEditModal, setShowEditModal] = useState(false);

  return (
    <div className={styles.profilePage}>
      <Header />
      {showEditModal && (
        <EditUserModal
          isOpen={showEditModal}
          userData={null}
          onClose={() => setShowEditModal(false)}
          onSave={() => console.log()}
        />
      )}
      <ProfileCardSection
        name="Myroslaw"
        email="some@example.com"
        onEditButtonClick={() => setShowEditModal(true)}
      />
      <div className={styles.wrapper}>
        <div className={styles.container}>
          <InteractedTestsSection
            createdTests={[
              { id: "1", title: "React Basics", date: "2025-08-01" },
              { id: "2", title: "TypeScript Advanced", date: "2025-08-10" },
            ]}
            passedTests={[
              {
                id: "3",
                title: "JavaScript Quiz",
                date: "2025-08-05",
                score: 85,
              },
            ]}
          />
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default ProfilePage;
