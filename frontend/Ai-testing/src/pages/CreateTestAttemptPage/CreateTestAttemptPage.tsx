// import { useEffect, useState } from "react";
// import { useParams, useNavigate } from "react-router-dom";
// import { getById, createAttempt } from "../../api/testService";
// import styles from "./CreateTestAttemptPage.module.css";
// import type { FullTestDto, QuestionDto } from "../../types/test";
// import Header from "../../components/Header/Header";
// import Footer from "../../components/Footer/Footer";

// const CreateTestAttemptPage: React.FC = () => {
//   const { id } = useParams<{ id: string }>();
//   const navigate = useNavigate();
//   const [test, setTest] = useState<FullTestDto | null>(null);
//   const [answers, setAnswers] = useState<Record<string, any>>({});
//   const [loading, setLoading] = useState(true);

//   useEffect(() => {
//     const fetchTest = async () => {
//       if (!id) return;
//       try {
//         const data = await getById(id);
//         setTest(data);
//       } catch (error) {
//         console.error(error);
//       } finally {
//         setLoading(false);
//       }
//     };
//     fetchTest();
//   }, [id]);

//   const handleSelect = (qId: string, optionId: string, multiple: boolean) => {
//     setAnswers((prev) => {
//       if (multiple) {
//         const prevArr = prev[qId]?.selectedOptionIds || [];
//         return {
//           ...prev,
//           [qId]: {
//             selectedOptionIds: prevArr.includes(optionId)
//               ? prevArr.filter((id: string) => id !== optionId)
//               : [...prevArr, optionId],
//           },
//         };
//       } else {
//         return { ...prev, [qId]: { selectedOptionIds: [optionId] } };
//       }
//     });
//   };

//   const handleText = (qId: string, text: string) => {
//     setAnswers((prev) => ({ ...prev, [qId]: { writtenAnswer: text } }));
//   };

//   const handleSubmit = async () => {
//     if (!id) return;
//     try {
//       const dto = {
//         testId: id,
//         answers: Object.entries(answers).map(([questionId, data]) => ({
//           questionId,
//           selectedOptionIds: data.selectedOptionIds,
//           writtenAnswer: data.writtenAnswer,
//         })),
//       };
//       const result = await createAttempt(dto);
//       navigate(`/tests/${id}/result/${result.id}`);
//     } catch (error) {
//       console.error("Failed to submit attempt:", error);
//     }
//   };

//   if (loading) return <div className={styles.loader}>Loading...</div>;
//   if (!test) return <div className={styles.loader}>Test not found</div>;

//   const sortedQuestions = [...test.questions].sort(
//     (a, b) => (a.order ?? 0) - (b.order ?? 0)
//   );

//   return (
//     <div className={styles.createTestAttemptPage}>
//       <Header />

//       <div className={styles.wrapper}>
//         <div className={styles.container}>
//           <div
//             className={styles.header}
//             style={{
//               backgroundImage: test.coverImageUrl
//                 ? `url(${test.coverImageUrl})`
//                 : "linear-gradient(135deg, #1e3c72, #2a5298)",
//             }}
//           >
//             <div className={styles.overlay}>
//               <h1>{test.title}</h1>
//               {test.description && <p>{test.description}</p>}
//             </div>
//           </div>

//           <div className={styles.content}>
//             {sortedQuestions.map((q: QuestionDto, idx) => (
//               <div key={q.id} className={styles.question}>
//                 <h3>
//                   {idx + 1}. {q.text}
//                 </h3>

//                 {/* Single / Multiple Choice */}
//                 {(q.type === "SingleChoice" || q.type === "MultipleChoice") && (
//                   <ul>
//                     {q.options.map((opt) => (
//                       <li key={opt.id}>
//                         <label>
//                           <input
//                             type={q.type === "MultipleChoice" ? "checkbox" : "radio"}
//                             name={q.id}
//                             value={opt.id}
//                             checked={
//                               q.type === "MultipleChoice"
//                                 ? answers[q.id]?.selectedOptionIds?.includes(opt.id)
//                                 : answers[q.id]?.selectedOptionIds?.[0] === opt.id
//                             }
//                             onChange={() =>
//                               handleSelect(q.id, opt.id, q.type === "MultipleChoice")
//                             }
//                           />
//                           {opt.text}
//                         </label>
//                       </li>
//                     ))}
//                   </ul>
//                 )}

//                 {/* Open-ended */}
//                 {q.type === "OpenEnded" && (
//                   <textarea
//                     value={answers[q.id]?.writtenAnswer || ""}
//                     onChange={(e) => handleText(q.id, e.target.value)}
//                     placeholder="Write your answer..."
//                   />
//                 )}
//               </div>
//             ))}

//             <div style={{ textAlign: "center", marginTop: "2rem" }}>
//               <button className={styles.submitButton} onClick={handleSubmit}>
//                 Submit Answers
//               </button>
//             </div>
//           </div>
//         </div>
//       </div>

//       <Footer />
//     </div>
//   );
// };

// export default CreateTestAttemptPage;
