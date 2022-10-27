﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Migrations
{
    [DbContext(typeof(ICodeDbContext))]
    partial class ICodeDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Data.Entity.Contest", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("PlayerLimit")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Contests");
                });

            modelBuilder.Entity("Data.Entity.ContestDetail", b =>
                {
                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ContestID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("RegisteredAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Score")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("UserID", "ContestID");

                    b.HasIndex("ContestID");

                    b.ToTable("ContestDetails");
                });

            modelBuilder.Entity("Data.Entity.ContestSubmission", b =>
                {
                    b.Property<string>("SubmitID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ContestID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("SubmitID");

                    b.HasIndex("ContestID");

                    b.ToTable("ContestSubmissions");
                });

            modelBuilder.Entity("Data.Entity.Problem", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ArticleID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("ArticleID");

                    b.ToTable("Problems");
                });

            modelBuilder.Entity("Data.Entity.ProblemContestDetail", b =>
                {
                    b.Property<string>("ContestID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProblemID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.HasKey("ContestID", "ProblemID");

                    b.HasIndex("ProblemID");

                    b.ToTable("ProblemContestDetails");
                });

            modelBuilder.Entity("Data.Entity.RefreshToken", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("ExpiredAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("IssuedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("JwtID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("State")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("Data.Entity.Reply", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Replies");
                });

            modelBuilder.Entity("Data.Entity.Report", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ProblemID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("ProblemID");

                    b.HasIndex("UserID");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Data.Entity.Role", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Data.Entity.Submission", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Submissions");
                });

            modelBuilder.Entity("Data.Entity.SubmissionDetail", b =>
                {
                    b.Property<string>("SubmitID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TestCaseID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("Memory")
                        .HasColumnType("real");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<float>("Time")
                        .HasColumnType("real");

                    b.HasKey("SubmitID", "TestCaseID");

                    b.HasIndex("TestCaseID");

                    b.ToTable("SubmissionDetails");
                });

            modelBuilder.Entity("Data.Entity.Tag", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Data.Entity.TestCase", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Input")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("MemoryLimit")
                        .HasColumnType("float");

                    b.Property<string>("Output")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProblemID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("TimeLimit")
                        .HasColumnType("float");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("ProblemID");

                    b.ToTable("TestCases");
                });

            modelBuilder.Entity("Data.Entity.User", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("AllowNotification")
                        .HasColumnType("bit");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ForgotPasswordToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ForgotPasswordTokenCreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ForgotPasswordTokenExpireAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Gender")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLogInAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RemindAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("RoleID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.HasKey("ID");

                    b.HasIndex("RoleID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProblemTag", b =>
                {
                    b.Property<string>("ProblemsID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TagsID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ProblemsID", "TagsID");

                    b.HasIndex("TagsID");

                    b.ToTable("ProblemTag");
                });

            modelBuilder.Entity("Data.Entity.ContestDetail", b =>
                {
                    b.HasOne("Data.Entity.Contest", "Contest")
                        .WithMany("ContestDetails")
                        .HasForeignKey("ContestID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.Entity.User", "User")
                        .WithMany("ContestDetails")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contest");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Entity.ContestSubmission", b =>
                {
                    b.HasOne("Data.Entity.Contest", "Contest")
                        .WithMany("Submissions")
                        .HasForeignKey("ContestID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Data.Entity.Submission", "Submission")
                        .WithOne("ContestSubmission")
                        .HasForeignKey("Data.Entity.ContestSubmission", "SubmitID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contest");

                    b.Navigation("Submission");
                });

            modelBuilder.Entity("Data.Entity.Problem", b =>
                {
                    b.HasOne("Data.Entity.User", "Article")
                        .WithMany("Problems")
                        .HasForeignKey("ArticleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");
                });

            modelBuilder.Entity("Data.Entity.ProblemContestDetail", b =>
                {
                    b.HasOne("Data.Entity.Contest", "Contest")
                        .WithMany("ProblemContestDetails")
                        .HasForeignKey("ContestID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.Entity.Problem", "Problem")
                        .WithMany("ProblemContestDetails")
                        .HasForeignKey("ProblemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contest");

                    b.Navigation("Problem");
                });

            modelBuilder.Entity("Data.Entity.RefreshToken", b =>
                {
                    b.HasOne("Data.Entity.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Entity.Reply", b =>
                {
                    b.HasOne("Data.Entity.Report", "Report")
                        .WithOne("Reply")
                        .HasForeignKey("Data.Entity.Reply", "ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Report");
                });

            modelBuilder.Entity("Data.Entity.Report", b =>
                {
                    b.HasOne("Data.Entity.Problem", "Problem")
                        .WithMany("Reports")
                        .HasForeignKey("ProblemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.Entity.User", "User")
                        .WithMany("Reports")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Problem");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Entity.Submission", b =>
                {
                    b.HasOne("Data.Entity.User", "User")
                        .WithMany("Submissions")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Data.Entity.SubmissionDetail", b =>
                {
                    b.HasOne("Data.Entity.Submission", "Submission")
                        .WithMany("SubmissionDetails")
                        .HasForeignKey("SubmitID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.Entity.TestCase", "TestCase")
                        .WithMany("SubmissionDetails")
                        .HasForeignKey("TestCaseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Submission");

                    b.Navigation("TestCase");
                });

            modelBuilder.Entity("Data.Entity.TestCase", b =>
                {
                    b.HasOne("Data.Entity.Problem", "Problem")
                        .WithMany("TestCases")
                        .HasForeignKey("ProblemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Problem");
                });

            modelBuilder.Entity("Data.Entity.User", b =>
                {
                    b.HasOne("Data.Entity.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("ProblemTag", b =>
                {
                    b.HasOne("Data.Entity.Problem", null)
                        .WithMany()
                        .HasForeignKey("ProblemsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Data.Entity.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Data.Entity.Contest", b =>
                {
                    b.Navigation("ContestDetails");

                    b.Navigation("ProblemContestDetails");

                    b.Navigation("Submissions");
                });

            modelBuilder.Entity("Data.Entity.Problem", b =>
                {
                    b.Navigation("ProblemContestDetails");

                    b.Navigation("Reports");

                    b.Navigation("TestCases");
                });

            modelBuilder.Entity("Data.Entity.Report", b =>
                {
                    b.Navigation("Reply");
                });

            modelBuilder.Entity("Data.Entity.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Data.Entity.Submission", b =>
                {
                    b.Navigation("ContestSubmission");

                    b.Navigation("SubmissionDetails");
                });

            modelBuilder.Entity("Data.Entity.TestCase", b =>
                {
                    b.Navigation("SubmissionDetails");
                });

            modelBuilder.Entity("Data.Entity.User", b =>
                {
                    b.Navigation("ContestDetails");

                    b.Navigation("Problems");

                    b.Navigation("Reports");

                    b.Navigation("Submissions");

                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}