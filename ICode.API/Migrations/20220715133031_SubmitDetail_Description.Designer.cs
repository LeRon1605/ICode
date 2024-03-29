﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Migrations
{
    [DbContext(typeof(ICodeDbContext))]
    [Migration("20220715133031_SubmitDetail_Description")]
    partial class SubmitDetail_Description
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("API.Models.Entity.Problem", b =>
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

            modelBuilder.Entity("API.Models.Entity.Reply", b =>
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

            modelBuilder.Entity("API.Models.Entity.Report", b =>
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

            modelBuilder.Entity("API.Models.Entity.Role", b =>
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

            modelBuilder.Entity("API.Models.Entity.Submission", b =>
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

                    b.Property<bool>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Submissions");
                });

            modelBuilder.Entity("API.Models.Entity.SubmissionDetail", b =>
                {
                    b.Property<string>("SubmitID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TestCaseID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Memory")
                        .HasColumnType("real");

                    b.Property<bool>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<float>("Time")
                        .HasColumnType("real");

                    b.HasKey("SubmitID", "TestCaseID");

                    b.HasIndex("TestCaseID");

                    b.ToTable("SubmissionDetails");
                });

            modelBuilder.Entity("API.Models.Entity.Tag", b =>
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

            modelBuilder.Entity("API.Models.Entity.TestCase", b =>
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

            modelBuilder.Entity("API.Models.Entity.User", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("RoleID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

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

            modelBuilder.Entity("API.Models.Entity.Problem", b =>
                {
                    b.HasOne("API.Models.Entity.User", "Article")
                        .WithMany("Problems")
                        .HasForeignKey("ArticleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");
                });

            modelBuilder.Entity("API.Models.Entity.Reply", b =>
                {
                    b.HasOne("API.Models.Entity.Report", "Report")
                        .WithOne("Reply")
                        .HasForeignKey("API.Models.Entity.Reply", "ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Report");
                });

            modelBuilder.Entity("API.Models.Entity.Report", b =>
                {
                    b.HasOne("API.Models.Entity.Problem", "Problem")
                        .WithMany("Reports")
                        .HasForeignKey("ProblemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Models.Entity.User", "User")
                        .WithMany("Reports")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Problem");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Models.Entity.Submission", b =>
                {
                    b.HasOne("API.Models.Entity.User", "User")
                        .WithMany("Submissions")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Models.Entity.SubmissionDetail", b =>
                {
                    b.HasOne("API.Models.Entity.Submission", "Submission")
                        .WithMany("SubmissionDetails")
                        .HasForeignKey("SubmitID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("API.Models.Entity.TestCase", "TestCase")
                        .WithMany("SubmissionDetails")
                        .HasForeignKey("TestCaseID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Submission");

                    b.Navigation("TestCase");
                });

            modelBuilder.Entity("API.Models.Entity.TestCase", b =>
                {
                    b.HasOne("API.Models.Entity.Problem", "Problem")
                        .WithMany("TestCases")
                        .HasForeignKey("ProblemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Problem");
                });

            modelBuilder.Entity("API.Models.Entity.User", b =>
                {
                    b.HasOne("API.Models.Entity.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("ProblemTag", b =>
                {
                    b.HasOne("API.Models.Entity.Problem", null)
                        .WithMany()
                        .HasForeignKey("ProblemsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Models.Entity.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("API.Models.Entity.Problem", b =>
                {
                    b.Navigation("Reports");

                    b.Navigation("TestCases");
                });

            modelBuilder.Entity("API.Models.Entity.Report", b =>
                {
                    b.Navigation("Reply");
                });

            modelBuilder.Entity("API.Models.Entity.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("API.Models.Entity.Submission", b =>
                {
                    b.Navigation("SubmissionDetails");
                });

            modelBuilder.Entity("API.Models.Entity.TestCase", b =>
                {
                    b.Navigation("SubmissionDetails");
                });

            modelBuilder.Entity("API.Models.Entity.User", b =>
                {
                    b.Navigation("Problems");

                    b.Navigation("Reports");

                    b.Navigation("Submissions");
                });
#pragma warning restore 612, 618
        }
    }
}
