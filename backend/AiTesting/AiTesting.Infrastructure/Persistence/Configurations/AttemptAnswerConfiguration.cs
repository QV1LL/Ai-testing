using AiTesting.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiTesting.Infrastructure.Persistence.Configurations;

public class AttemptAnswerConfiguration : IEntityTypeConfiguration<AttemptAnswer>
{
    public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
    {
        builder.ToTable("AttemptAnswers");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.WrittenAnswer)
            .HasMaxLength(2000);

        builder.HasOne(a => a.Attempt)
            .WithMany(at => at.Answers)
            .HasForeignKey(a => a.AttemptId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(a => a.SelectedOptions)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "AttemptAnswerSelectedOption",
                j => j
                    .HasOne<AnswerOption>()
                    .WithMany()
                    .HasForeignKey("AnswerOptionId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<AttemptAnswer>()
                    .WithMany()
                    .HasForeignKey("AttemptAnswerId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("AttemptAnswerId", "AnswerOptionId");
                    j.ToTable("AttemptAnswerSelectedOptions");
                });
    }
}
