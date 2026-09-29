using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace try2.Models;

public partial class StudentPlannerDbContext : DbContext
{
    public StudentPlannerDbContext()
    {
    }

    public StudentPlannerDbContext(DbContextOptions<StudentPlannerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Deadline> Deadlines { get; set; }

    public virtual DbSet<LecturesNote> LecturesNotes { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=StudentPlannerDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Deadline>(entity =>
        {
            entity.HasKey(e => e.DeadlineId).HasName("PK__Deadline__CAF057F7CBD63F03");

            entity.Property(e => e.DeadlineId).HasColumnName("DeadlineID");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.IsCompleted).HasDefaultValue(false, "DF_Deadlines_IsCompleted");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TaskDescription).HasMaxLength(1000);

            entity.HasOne(d => d.Subject).WithMany(p => p.Deadlines)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_Deadlines_Subjects");
        });

        modelBuilder.Entity<LecturesNote>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PK__Lectures__EACE357FF3A38088");

            entity.Property(e => e.NoteId).HasColumnName("NoteID");
            entity.Property(e => e.FileLink).HasMaxLength(2083);
            entity.Property(e => e.LectureTitle).HasMaxLength(250);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.Subject).WithMany(p => p.LecturesNotes)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_LecturesNotes_Subjects");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.LessonId).HasName("PK__Schedule__B084ACB07AF858AB");

            entity.ToTable("Schedule");

            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.Classroom).HasMaxLength(50);
            entity.Property(e => e.LessonType).HasMaxLength(50);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.Subject).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_Schedule_Subjects");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA38892AA056A");

            entity.HasIndex(e => e.SubjectName, "UQ__Subjects__4C5A7D55EF302128").IsUnique();

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.SubjectName).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
