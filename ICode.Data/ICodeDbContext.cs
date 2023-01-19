using Data.Entity;
using Data.Entity.Config;
using ICode.Data.Entity;
using ICode.Data.Entity.Config;
using Microsoft.EntityFrameworkCore;

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

            new RoleEntityConfig().Configure(modelBuilder.Entity<Role>());
            new UserEntityConfig().Configure(modelBuilder.Entity<User>());
            new TagEntityConfig().Configure(modelBuilder.Entity<Tag>());
            new ReportEntityConfig().Configure(modelBuilder.Entity<Report>());
            new ReplyEntityConfig().Configure(modelBuilder.Entity<Reply>());
            new ProblemEntityConfig().Configure(modelBuilder.Entity<Problem>());
            new TestcaseEntityConfig().Configure(modelBuilder.Entity<TestCase>());
            new SubmissionEntityConfig().Configure(modelBuilder.Entity<Submission>());
            new SubmissionDetailEntityConfig().Configure(modelBuilder.Entity<SubmissionDetail>());
            new RefreshTokenEntityConfig().Configure(modelBuilder.Entity<RefreshToken>());
            new ContestEntityConfig().Configure(modelBuilder.Entity<Contest>());
            new ContestDetailEntityConfig().Configure(modelBuilder.Entity<ContestDetail>());
            new ProblemContestDetailEntityConfig().Configure(modelBuilder.Entity<ProblemContestDetail>());
            new ContestSubmissionEntityConfig().Configure(modelBuilder.Entity<ContestSubmission>());
            new CommentEntityConfig().Configure(modelBuilder.Entity<Comment>());
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Reply> Replies { get; set; }
        public virtual DbSet<Problem> Problems { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Submission> Submissions { get; set; }
        public virtual DbSet<SubmissionDetail> SubmissionDetails { get; set; }
        public virtual DbSet<TestCase> TestCases { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Contest> Contests { get; set; }
        public virtual DbSet<ContestDetail> ContestDetails { get; set; }
        public virtual DbSet<ProblemContestDetail> ProblemContestDetails { get; set; }
        public virtual DbSet<ContestSubmission> ContestSubmissions { get; set; }
    }
}
