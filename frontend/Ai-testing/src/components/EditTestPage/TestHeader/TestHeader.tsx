import React, { useRef, useState, useEffect } from "react";
import styles from "./TestHeader.module.css";
import type { UpdateTestMetadataDto } from "../../../types/test";
import DropZone from "../DropZone/DropZone";

interface TestHeaderProps {
  metadata: UpdateTestMetadataDto;
  preview: string | null;
  onSaveTest: (metadata: UpdateTestMetadataDto) => void;
  onCancel: () => void;
}

const TestHeader: React.FC<TestHeaderProps> = ({
  metadata,
  preview: initialPreview,
  onSaveTest,
  onCancel,
}) => {
  const [preview, setPreview] = useState<string | null>(initialPreview);
  const [coverImage, setCoverImage] = useState<File | null>(null);

  const [isPublic, setIsPublic] = useState<boolean>(metadata.isPublic);
  const [timeLimit, setTimeLimit] = useState<number | null>(
    metadata.timeLimitInMinutes ?? null
  );
  const [hasTimeLimit, setHasTimeLimit] = useState<boolean>(
    metadata.timeLimitInMinutes !== null &&
      metadata.timeLimitInMinutes !== undefined
  );

  const titleRef = useRef<HTMLInputElement>(null);
  const descRef = useRef<HTMLTextAreaElement>(null);

  useEffect(() => {
    setPreview(initialPreview);
  }, [initialPreview]);

  const handleFile = (file: File | null) => {
    if (file) {
      setCoverImage(file);
      setPreview(URL.createObjectURL(file));
    }
  };

  const handleSave = () => {
    onSaveTest({
      ...metadata,
      title: titleRef.current?.value ?? "",
      description: descRef.current?.value ?? "",
      isPublic: isPublic,
      timeLimitInMinutes: hasTimeLimit ? timeLimit : null,
      coverImage: coverImage,
    });
  };

  return (
    <div
      className={styles.testHeader}
      style={{
        backgroundImage: preview
          ? `url(${preview})`
          : "linear-gradient(135deg, #1e3c72, #2a5298)",
        backgroundSize: "cover",
        backgroundPosition: "center",
        backgroundRepeat: "no-repeat",
      }}
    >
      <div className={styles.overlay}>
        <input
          ref={titleRef}
          className={styles.titleInput}
          defaultValue={metadata.title}
          placeholder="No title"
        />
        <textarea
          ref={descRef}
          className={styles.descriptionInput}
          defaultValue={metadata.description || ""}
          placeholder="No description"
        />

        <div className={styles.settings}>
          <label className={styles.checkboxLabel}>
            <input
              type="checkbox"
              checked={isPublic}
              onChange={(e) => setIsPublic(e.target.checked)}
            />
            Public test
          </label>

          <div className={styles.timeLimitWrapper}>
            <label className={styles.checkboxLabel}>
              <input
                type="checkbox"
                checked={hasTimeLimit}
                onChange={(e) => {
                  setHasTimeLimit(e.target.checked);
                  if (!e.target.checked) setTimeLimit(null);
                  else setTimeLimit(metadata?.timeLimitInMinutes ?? null);
                }}
              />
              Enable time limit
            </label>

            {hasTimeLimit && (
              <input
                type="number"
                min={1}
                value={timeLimit ?? ""}
                onChange={(e) =>
                  setTimeLimit(e.target.value ? Number(e.target.value) : null)
                }
                className={styles.timeLimitInput}
              />
            )}
          </div>
        </div>

        <DropZone onFileSelect={handleFile} />
      </div>

      <div className={styles.overlayFooter}>
        <button className={styles.saveButton} onClick={handleSave}>
          Save
        </button>
        <button className={styles.cancelButton} onClick={onCancel}>
          Cancel
        </button>
      </div>
    </div>
  );
};

export default TestHeader;
