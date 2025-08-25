import type { FullTestDto } from "../../../types/test";

interface TestInfoFormProps {
  test: FullTestDto;
  coverFile: File | null;
  setCoverFile: (file: File | null) => void;
  onChangeTitle: (val: string) => void;
  onChangeDescription: (val: string) => void;
  onSave: () => void;
}

export const TestInfoForm: React.FC<TestInfoFormProps> = ({
  test,
  setCoverFile,
  onChangeTitle,
  onChangeDescription,
  onSave,
}) => (
  <div>
    <input
      type="text"
      value={test.title}
      onChange={(e) => onChangeTitle(e.target.value)}
      placeholder="Test title"
    />
    <textarea
      value={test.description ?? ""}
      onChange={(e) => onChangeDescription(e.target.value)}
      placeholder="Description"
    />
    <input
      type="file"
      accept="image/*"
      onChange={(e) => setCoverFile(e.target.files ? e.target.files[0] : null)}
    />
    <button onClick={onSave}>Save Test Info</button>
  </div>
);
