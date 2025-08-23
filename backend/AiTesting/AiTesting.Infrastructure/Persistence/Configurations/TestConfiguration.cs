using AiTesting.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiTesting.Infrastructure.Persistence.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.ToTable("Tests");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(t => t.IsPublic)
            .IsRequired();

        builder.Property(t => t.TimeLimitMinutes);

        builder.Property(t => t.CreatedAt)
            .HasColumnType("timestamptz")
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v.DateTime, DateTimeKind.Utc)
            );

        builder.Property<DateTime>("UpdatedAt")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasOne(t => t.CreatedBy)
            .WithMany(u => u.Tests)
            .HasForeignKey(t => t.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Questions)
            .WithOne(q => q.Test)
            .HasForeignKey(q => q.TestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
