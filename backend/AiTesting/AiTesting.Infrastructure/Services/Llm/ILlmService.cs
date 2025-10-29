using AiTesting.Domain.Common;
using AiTesting.Domain.Models;

namespace AiTesting.Infrastructure.Services.Llm;

public interface ILlmService
{
    Task<Result<T>> GenerateUpdateQuestionsDto<T>(Test test, string prompt);
    Task<bool> IsCorrect(string userAnswer, string correctAnswer, string questionText);
}
