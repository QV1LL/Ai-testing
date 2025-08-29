import React, { useState, useRef } from "react";
import styles from "./DropZone.module.css";

interface DropZoneProps {
  onFileSelect: (file: File) => void;
  preview?: string | null;
}

const DropZone: React.FC<DropZoneProps> = ({ onFileSelect, preview }) => {
  const [dragActive, setDragActive] = useState(false);
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  const handleClick = () => {
    fileInputRef.current?.click();
  };

  const handleDrag = (e: React.DragEvent, active: boolean) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(active);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);

    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      const file = e.dataTransfer.files[0];
      onFileSelect(file);
    }
  };

  const handleFile = (file: File) => {
    onFileSelect(file);
  };

  return (
    <div
      className={`${styles.dropZone} ${dragActive ? styles.active : ""}`}
      onClick={handleClick}
      onDragEnter={(e) => handleDrag(e, true)}
      onDragOver={(e) => handleDrag(e, true)}
      onDragLeave={(e) => handleDrag(e, false)}
      onDrop={handleDrop}
    >
      {preview ? (
        <img src={preview} alt="preview" style={{ maxHeight: "100px" }} />
      ) : dragActive ? (
        <p>Drop image here...</p>
      ) : (
        <p>Click or drag to upload</p>
      )}

      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        style={{ display: "none" }}
        onChange={(e) => {
          if (e.target.files && e.target.files[0]) {
            handleFile(e.target.files[0]);
          }
        }}
      />
    </div>
  );
};

export default DropZone;
