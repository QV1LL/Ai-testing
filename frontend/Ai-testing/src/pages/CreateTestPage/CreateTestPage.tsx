import React, { useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import styles from "./CreateTestPage.module.css";
import * as testService from "../../api/testService";
import type { CreateTestDto } from "../../types/test";

const CreateTestPage: React.FC = () => {
  const navigate = useNavigate();

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [coverImage, setCoverImage] = useState<File | null>(null);
  const [preview, setPreview] = useState<string>("");
  const [isPublic, setIsPublic] = useState(false);
  const [timeLimit, setTimeLimit] = useState<number | "">("");
  const [error, setError] = useState("");

  const fileInputRef = useRef<HTMLInputElement>(null);

  const resetForm = () => {
    setTitle("");
    setDescription("");
    setCoverImage(null);
    setPreview("");
    setIsPublic(false);
    setTimeLimit("");
    setError("");
  };

  const handleFileChange = (file: File) => {
    setCoverImage(file);
    setPreview(URL.createObjectURL(file));
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      handleFileChange(e.dataTransfer.files[0]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      const data: CreateTestDto = {
        title,
        description,
        isPublic,
        timeLimitMinutes: Number(timeLimit === "" ? 0 : timeLimit),
      };

      await testService.create(data, coverImage); // 👈 оновлений сервіс приймає файл

      resetForm();
      navigate("/tests");
    } catch (err: any) {
      setError(err.response?.data?.message || "Failed to create test");
    }
  };

  return (
    <div className={styles.createTestPage}>
      <Header />
      <div className={styles.pageWrapper}>
        <div className={styles.formContainer}>
          <form onSubmit={handleSubmit} className={styles.testForm}>
            <div className={styles.formGroup}>
              <label htmlFor="title">Title</label>
              <input
                id="title"
                type="text"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="Enter test title"
                required
                className={error ? styles.errorInput : ""}
                onFocus={() => setError("")}
              />
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="description">Description</label>
              <textarea
                id="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Enter test description"
                rows={4}
                className={error ? styles.errorInput : ""}
                onFocus={() => setError("")}
              />
            </div>

            {/* Upload / Drag area */}
            <div className={styles.formGroup}>
              <label htmlFor="dropzone">Cover image</label>
              <div
                id="dropzone"
                className={styles.dropZone}
                onDrop={handleDrop}
                onDragOver={(e) => e.preventDefault()}
                onClick={() => fileInputRef.current?.click()}
              >
                {preview ? (
                  <img
                    src={preview}
                    alt="Preview"
                    className={styles.previewImage}
                  />
                ) : (
                  <p>Drag & drop an image here or click to upload</p>
                )}
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/*"
                  style={{ display: "none" }}
                  onChange={(e) => {
                    if (e.target.files && e.target.files[0]) {
                      handleFileChange(e.target.files[0]);
                    }
                  }}
                />
              </div>
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="timeLimit">Time Limit (minutes)</label>
              <input
                id="timeLimit"
                type="number"
                value={timeLimit}
                onChange={(e) =>
                  setTimeLimit(
                    e.target.value === "" ? "" : Number(e.target.value)
                  )
                }
                placeholder="Optional"
                min={1}
                className={error ? styles.errorInput : ""}
              />
            </div>

            <div className={styles.formGroupInline}>
              <label htmlFor="isPublic">Public Test</label>
              <input
                id="isPublic"
                type="checkbox"
                checked={isPublic}
                onChange={(e) => setIsPublic(e.target.checked)}
              />
            </div>

            <button type="submit" className={styles.primaryBtn}>
              Create Test
            </button>

            {error && <p className={styles.errorText}>{error}</p>}
          </form>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default CreateTestPage;
