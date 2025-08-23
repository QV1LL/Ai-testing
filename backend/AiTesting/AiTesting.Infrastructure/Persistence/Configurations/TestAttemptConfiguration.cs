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
            .IsRequired()
            .HasColumnType("timestamptz")
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v.DateTime, DateTimeKind.Utc)
            );

        builder.Property(a => a.FinishedAt)
            .HasColumnType("timestamptz")
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => DateTime.SpecifyKind(v.HasValue ? v.Value.DateTime : DateTime.Now, DateTimeKind.Utc)
            );

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
