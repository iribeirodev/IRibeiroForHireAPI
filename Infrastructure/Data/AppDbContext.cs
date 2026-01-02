using Microsoft.EntityFrameworkCore;
using IRibeiroForHireAPI.Domain.Entities;

namespace IRibeiroForHire.Infrastructure.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options){}

    public DbSet<QaInteraction> QaInteractions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QaInteraction>(entity =>
        {
            entity.ToTable("qa_interactions");

            entity.Property(e => e.AnswerText)
                  .HasColumnName("answer")
                  .HasColumnType("text");

            entity.Property(e => e.QuestionText)
                  .HasColumnName("question")
                  .HasColumnType("text");

            entity.Property(e => e.InteractionTime)
                  .HasColumnName("interaction_time")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
