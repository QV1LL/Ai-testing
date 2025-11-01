import React, { useState, useEffect } from "react";
import styles from "./LoaderModal.module.css";

interface LoaderModalProps {
  isLoading: boolean;
  isFailed?: boolean;
  message?: string;
  errorMessage?: string;
  onClose?: () => void;
}

const LoaderModal: React.FC<LoaderModalProps> = ({
  isLoading,
  isFailed = false,
  message = "Завантаження...",
  errorMessage = "Сталася помилка. Спробуйте ще раз.",
  onClose,
}) => {
  const [visible, setVisible] = useState(isLoading || isFailed);

  useEffect(() => {
    setVisible(isLoading || isFailed);
  }, [isLoading, isFailed]);

  if (!visible) return null;

  return (
    <div className={styles.loaderModalOverlay}>
      <div
        className={`${styles.loaderModal} ${
          isFailed ? styles.loaderModalError : ""
        }`}
      >
        {isFailed ? (
          <>
            <div className={styles.errorIcon}>⚠️</div>
            <p className={styles.errorMessage}>{errorMessage}</p>
            <button
              className={styles.closeButton}
              onClick={() => {
                setVisible(false);
                onClose?.();
              }}
            >
              OK
            </button>
          </>
        ) : (
          <>
            <div className={styles.loaderSpinner}></div>
            <p>{message}</p>
          </>
        )}
      </div>
    </div>
  );
};

export default LoaderModal;
