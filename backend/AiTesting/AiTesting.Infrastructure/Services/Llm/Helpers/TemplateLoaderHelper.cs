namespace AiTesting.Infrastructure.Services.Llm.Helpers;

internal static class TemplateLoaderHelper
{
    private const string GENERATE_QUESTIONS_TEMPLATE_PATH = "generate_questions_prompt.txt";
    private const string CHECK_ANSWERS_TEMPLATE_PATH = "check_answer_prompt.txt";

    public static string LoadGenerateTemplate()
    {
        var templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GENERATE_QUESTIONS_TEMPLATE_PATH);
        if (!File.Exists(templateFilePath))
            throw new FileNotFoundException("Prompt template file not found.", templateFilePath);
        return File.ReadAllText(templateFilePath);
    }

    public static string LoadCheckAnswerTemplate()
    {
        var templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CHECK_ANSWERS_TEMPLATE_PATH);
        if (!File.Exists(templateFilePath))
            throw new FileNotFoundException("Prompt template file not found.", templateFilePath);
        return File.ReadAllText(templateFilePath);
    }
}
