import React from "react";
import styles from "./LoaderModal.module.css";

interface LoaderModalProps {
  isLoading: boolean;
  message?: string;
}

const LoaderModal: React.FC<LoaderModalProps> = ({
  isLoading,
  message = "Завантаження...",
}) => {
  if (!isLoading) return null;

  return (
    <div className={styles.loaderModalOverlay}>
      <div className={styles.loaderModal}>
        <div className={styles.loaderSpinner}></div>
        <p>{message}</p>
      </div>
    </div>
  );
};

export default LoaderModal;
