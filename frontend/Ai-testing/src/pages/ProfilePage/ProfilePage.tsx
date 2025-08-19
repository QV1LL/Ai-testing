import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import styles from "./ProfilePage.module.css";
import ProfileCardSection from "../../components/ProfilePage/ProfileCardSection/ProfileCardSection";
import InteractedTestsSection from "../../components/ProfilePage/InteractedTestsSection/InteractedTestsSection";
import EditUserModal from "../../components/EditUserModal/EditUserModal";
import { deleteProfile, getProfile } from "../../api/profileService";
import { logout } from "../../api/profileService";
import type { ProfileDto } from "../../types/profile";

const mockProfile: ProfileDto = {
  name: "Loading...",
  email: "loading@example.com",
};

const ProfilePage: React.FC = () => {
  const navigate = useNavigate();
  const [showEditModal, setShowEditModal] = useState(false);
  const [profile, setProfile] = useState<ProfileDto>(mockProfile);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchProfile = async () => {
      setError("");
      try {
        const data = await getProfile();
        setProfile(data);
      } catch (err: any) {
        console.error(err);
      }
    };

    fetchProfile();
  }, []);

  const handleLogout = () => {
    logout();
    navigate("/");
  };

  const handleDelete = () => {
    try {
      deleteProfile();
      logout();
      navigate("/");
    } catch {}
  };

  if (error) return <p>{error}</p>;

  return (
    <div className={styles.profilePage}>
      <Header />
      {showEditModal && (
        <EditUserModal
          isOpen={showEditModal}
          userData={profile}
          onClose={() => setShowEditModal(false)}
          onSave={(updatedUser) =>
            setProfile({ name: updatedUser.name, email: updatedUser.email })
          }
        />
      )}
      <ProfileCardSection
        name={profile.name}
        email={profile.email}
        onEditButtonClick={() => setShowEditModal(true)}
        onLogOutButtonClick={handleLogout}
        onDeleteButtonClick={handleDelete}
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
