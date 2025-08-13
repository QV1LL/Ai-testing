using AiTesting.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiTesting.Infrastructure.Persistence.Configurations;

public class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttempt>
{
    public void Configure(EntityTypeBuilder<TestAttempt> builder)
    {
        builder.ToTable("TestAttempts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.StartedAt)
            .IsRequired();

        builder.Property(a => a.FinishedAt);

        builder.Property(a => a.Score)
            .IsRequired();

        builder.HasOne(a => a.User)
            .WithMany(u => u.TestAttempts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Guest)
            .WithMany()
            .HasForeignKey(a => a.GuestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Test)
            .WithMany(t => t.TestAttempts)
            .HasForeignKey(a => a.TestId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Answers)
            .WithOne(aa => aa.Attempt)
            .HasForeignKey(aa => aa.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
