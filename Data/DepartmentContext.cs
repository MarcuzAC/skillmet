using Microsoft.EntityFrameworkCore;
using DepartmentalSystemAPI.Models;

namespace DepartmentalSystemAPI.Data
{
    public class DepartmentContext : DbContext
    {
        public DepartmentContext(DbContextOptions<DepartmentContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<ProjectSkill> ProjectSkills { get; set; }
        public DbSet<AssignmentHistory> AssignmentHistories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Employee configuration
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId);

            // Skill configuration
            modelBuilder.Entity<Skill>()
                .HasIndex(s => s.Name)
                .IsUnique();

            // EmployeeSkill configuration
            modelBuilder.Entity<EmployeeSkill>()
                .HasKey(es => es.Id);

            modelBuilder.Entity<EmployeeSkill>()
                .HasIndex(es => new { es.EmployeeId, es.SkillId })
                .IsUnique();

            // ProjectSkill configuration
            modelBuilder.Entity<ProjectSkill>()
                .HasKey(ps => ps.Id);

            modelBuilder.Entity<ProjectSkill>()
                .HasIndex(ps => new { ps.ProjectId, ps.SkillId })
                .IsUnique();

            // Task dependencies
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.PredecessorTask)
                .WithMany(t => t.SuccessorTasks)
                .HasForeignKey(t => t.PredecessorTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // Fix cascade delete issue for StageId
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.Stage)
                .WithMany()
                .HasForeignKey(t => t.StageId)
                .OnDelete(DeleteBehavior.NoAction);

            // Fix cascade delete issue for AssignedToId (optional, but good practice)
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.NoAction);

            // Fix cascade delete issue for ProjectId (optional, but good practice)
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // This should be fine since it's the only cascade path

            // Stage configuration
            modelBuilder.Entity<Stage>()
                .HasIndex(s => new { s.ProjectId, s.Order })
                .IsUnique();

            // AssignmentHistory configuration
            modelBuilder.Entity<AssignmentHistory>()
                .HasKey(ah => ah.Id);

            modelBuilder.Entity<AssignmentHistory>()
                .HasOne(ah => ah.Task)
                .WithMany()
                .HasForeignKey(ah => ah.TaskId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AssignmentHistory>()
                .HasOne(ah => ah.Employee)
                .WithMany()
                .HasForeignKey(ah => ah.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            // AuditLog configuration
            modelBuilder.Entity<AuditLog>()
                .HasKey(al => al.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}