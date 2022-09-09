using Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class ICodeDbContext: DbContext
    {
        public ICodeDbContext(DbContextOptions options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(role => role.ID);
                entity.Property(role => role.Name)
                      .HasMaxLength(8)
                      .IsRequired();
                entity.Property(role => role.Priority)
                      .IsRequired();
                entity.Property(role => role.CreatedAt)
                      .IsRequired();
                entity.Property(role => role.UpdatedAt)
                      .IsRequired(false)
                      .HasDefaultValue(null);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(user => user.ID);
                entity.HasOne(user => user.Role)
                      .WithMany(role => role.Users)
                      .HasForeignKey(user => user.RoleID);
                entity.Property(user => user.Username)
                      .HasMaxLength(16)
                      .IsRequired();
                entity.Property(user => user.Password)
                      .IsRequired();
                entity.Property(user => user.Email)
                      .IsRequired();
                entity.Property(user => user.Avatar)
                      .IsRequired();
                entity.Property(user => user.Gender)
                      .IsRequired();
                entity.Property(user => user.CreatedAt)
                      .IsRequired();
                entity.Property(user => user.UpdatedAt)
                      .IsRequired(false)
                      .HasDefaultValue(null);
                entity.Property(user => user.RoleID)
                      .IsRequired();
                entity.Property(user => user.ForgotPasswordToken)
                      .IsRequired(false);
                entity.Property(user => user.ForgotPasswordTokenCreatedAt)
                      .IsRequired(false);
                entity.Property(user => user.ForgotPasswordTokenExpireAt)
                      .IsRequired(false);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(tag => tag.ID);
                entity.Property(tag => tag.Name)
                      .HasMaxLength(16)
                      .IsRequired();
                entity.Property(tag => tag.CreatedAt)
                      .IsRequired();
                entity.Property(tag => tag.UpdatedAt)
                      .IsRequired(false)
                      .HasDefaultValue(null);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(report => report.ID);
                entity.Property(report => report.Title)
                      .HasMaxLength(255)
                      .IsRequired();
                entity.Property(report => report.Content)
                      .IsRequired();
                entity.Property(report => report.CreatedAt)
                      .IsRequired();
                entity.Property(report => report.UpdatedAt)
                      .IsRequired(false)
                      .HasDefaultValue(null);
                entity.Property(report => report.UserID)
                      .IsRequired();
                entity.Property(report => report.ProblemID)
                      .IsRequired();
                entity.HasOne(report => report.User)
                      .WithMany(user => user.Reports)
                      .HasForeignKey(report => report.UserID)
                      .OnDelete(DeleteBehavior.NoAction);
                //entity.HasOne(report => report.Problem)
                //      .WithMany(problem => problem.Reports)
                //      .HasForeignKey(report => report.ProblemID);
            });

            modelBuilder.Entity<Reply>(entity =>
            {
                entity.HasKey(reply => reply.ID);
                entity.Property(reply => reply.Content)
                      .IsRequired();
                entity.Property(reply => reply.CreatedAt)
                      .IsRequired();
                entity.Property(reply => reply.UpdatedAt)
                      .IsRequired(false)
                      .HasDefaultValue(null);
                entity.HasOne(reply => reply.Report)
                      .WithOne(report => report.Reply)
                      .HasForeignKey<Reply>(reply => reply.ID);
            });

            modelBuilder.Entity<Problem>(entity =>
            {
                entity.HasKey(problem => problem.ID);
                entity.Property(problem => problem.Name)
                      .IsRequired();
                entity.Property(problem => problem.Description)
                      .IsRequired();
                entity.Property(problem => problem.ArticleID)
                      .IsRequired();
                entity.Property(problem => problem.Status)
                      .IsRequired()
                      .HasDefaultValue(false);
                entity.Property(problem => problem.CreatedAt)
                      .IsRequired();
                entity.Property(problem => problem.UpdatedAt)
                      .IsRequired(false)
                      .HasDefaultValue(null);
                entity.HasOne(problem => problem.Article)
                      .WithMany(user => user.Problems)
                      .HasForeignKey(problem => problem.ArticleID);
                entity.HasMany(problem => problem.Reports)
                      .WithOne(report => report.Problem)
                      .HasForeignKey(report => report.ProblemID);
            });

            modelBuilder.Entity<TestCase>(entity =>
            {
                entity.HasKey(testcase => testcase.ID);
                entity.Property(testcase => testcase.Input)
                      .IsRequired();
                entity.Property(testcase => testcase.Output)
                      .IsRequired();
                entity.Property(testcase => testcase.MemoryLimit)
                      .HasColumnType("float")
                      .IsRequired();
                entity.Property(testcase => testcase.TimeLimit)
                      .HasColumnType("float")
                      .IsRequired();
                entity.Property(testcase => testcase.CreatedAt)
                      .IsRequired();
                entity.Property(testcase => testcase.UpdatedAt)
                      .IsRequired(false)
                      .HasDefaultValue(null);
                entity.Property(testcase => testcase.ProblemID)
                      .IsRequired();
                entity.HasOne(testcase => testcase.Problem)
                      .WithMany(problem => problem.TestCases)
                      .HasForeignKey(testcase => testcase.ProblemID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(submit => submit.ID);
                entity.Property(submit => submit.Code)
                      .IsRequired();
                entity.Property(submit => submit.Language)
                      .IsRequired();
                entity.Property(submit => submit.CreatedAt)
                      .IsRequired();
                entity.Property(submit => submit.Status)
                      .IsRequired()
                      .HasDefaultValue(false);
                entity.Property(submit => submit.UserID)
                      .IsRequired();
                entity.Property(submit => submit.Description)
                      .IsRequired();
                entity.HasOne(submit => submit.User)
                      .WithMany(user => user.Submissions)
                      .HasForeignKey(submit => submit.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SubmissionDetail>(entity =>
            {
                entity.HasKey(detail => new { detail.SubmitID, detail.TestCaseID });
                entity.Property(detail => detail.Status)
                      .IsRequired();
                entity.Property(detail => detail.Memory)
                      .IsRequired();
                entity.Property(detail => detail.Time)
                      .IsRequired();
                entity.HasOne(detail => detail.Submission)
                      .WithMany(detail => detail.SubmissionDetails)
                      .HasForeignKey(detail => detail.SubmitID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(detail => detail.TestCase)
                      .WithMany(testcase => testcase.SubmissionDetails)
                      .HasForeignKey(detail => detail.TestCaseID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(token => token.ID);
                entity.Property(token => token.Token)
                      .IsRequired();
                entity.Property(token => token.State)
                      .HasDefaultValue(false)
                      .IsRequired();
                entity.Property(token => token.JwtID)
                      .IsRequired();
                entity.Property(token => token.UserID)
                      .IsRequired();
                entity.Property(token => token.JwtID)
                      .IsRequired();
                entity.HasOne(token => token.User)
                      .WithMany(user => user.Tokens)
                      .HasForeignKey(token => token.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Reply> Replies { get; set; }
        public virtual DbSet<Problem> Problems { get; set; }
        public virtual DbSet<Submission> Submissions { get; set; }
        public virtual DbSet<SubmissionDetail> SubmissionDetails { get; set; }
        public virtual DbSet<TestCase> TestCases { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
