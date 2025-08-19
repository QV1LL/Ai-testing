import React, { useState, useEffect } from "react";
import styles from "./EditUserModal.module.css";
import { updateProfile } from "../../api/profileService";

interface EditUserModalProps {
  isOpen: boolean;
  userData: { name: string; email: string } | null;
  onClose: () => void;
  onSave: (updatedUser: { name: string; email: string }) => void;
}

const EditUserModal: React.FC<EditUserModalProps> = ({
  isOpen,
  userData,
  onClose,
  onSave,
}) => {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    if (userData) {
      setName(userData.name);
      setEmail(userData.email);
    }
  }, [userData]);

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      await updateProfile({ name, email });
      onSave({ name, email });
      onClose();
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div className={styles.overlay}>
      <div className={styles.modal}>
        <button className={styles.closeBtn} onClick={onClose}>
          &times;
        </button>
        <h2 className={styles.modalHeader}>Edit User</h2>
        <form onSubmit={handleSubmit} className={styles.form}>
          <div className={styles.formGroup}>
            <label>Name</label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className={error ? styles.errorInput : ""}
              onFocus={() => setError("")}
              required
            />
          </div>
          <div className={styles.formGroup}>
            <label>Email</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className={error ? styles.errorInput : ""}
              onFocus={() => setError("")}
              required
            />
          </div>
          <button type="submit" className={styles.primaryBtn}>
            Save Changes
          </button>
          {error && <p className={styles.errorText}>{error}</p>}
        </form>
      </div>
    </div>
  );
};

export default EditUserModal;
