using AutoMapper;
using Data.Repository.Interfaces;
using Data.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Services.Interfaces;
using Services;
using Xunit;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Data.Entity;

namespace UnitTest.ServiceTest
{
    public class ReportServiceTest
    {
        private readonly IReportService reportService;

        private readonly Mock<IReportRepository> reportRepositoryMock;
        private readonly Mock<IReplyRepository> replyRepositoryMock;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IMapper> mappeMock;
        public ReportServiceTest()
        {
            reportRepositoryMock = new Mock<IReportRepository>();
            replyRepositoryMock = new Mock<IReplyRepository>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            mappeMock = new Mock<IMapper>();

            reportService = new ReportService(reportRepositoryMock.Object, replyRepositoryMock.Object, unitOfWorkMock.Object, mappeMock.Object);
        }

        [Fact]
        public async Task WhenReplyToReport_ThenShouldWork()
        {
            Report report = new Report
            {
                Reply = null
            };
            reportRepositoryMock.Setup(x => x.GetReportsDetailSingle(It.IsAny<Expression<Func<Report, bool>>>())).Returns(report);

            bool result = await reportService.Reply("ReportID", new CodeStudy.Models.ReplyInput
            {
                Content = "ReplyContent",
            });

            Assert.True(result);
            reportRepositoryMock.Verify(x => x.Update(report), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GivenReportAlreadyReplied_WhenReplyToReport_ThenShouldWork()
        {
            Report report = new Report
            {
                Reply = new Reply()
            };
            reportRepositoryMock.Setup(x => x.GetReportsDetailSingle(It.IsAny<Expression<Func<Report, bool>>>())).Returns(report);

            bool result = await reportService.Reply("ReportID", new CodeStudy.Models.ReplyInput
            {
                Content = "ReplyContent",
            });

            Assert.False(result);
            reportRepositoryMock.Verify(x => x.Update(report), Times.Never);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task GivenReportAlreadyReplied_WhenUpdateReply_ThenShouldWork()
        {
            Report report = new Report
            {
                Reply = new Reply()
            };
            reportRepositoryMock.Setup(x => x.GetReportsDetailSingle(It.IsAny<Expression<Func<Report, bool>>>())).Returns(report);

            bool result = await reportService.UpdateReply("ReportID", new CodeStudy.Models.ReplyInput
            {
                Content = "ReplyContent",
            });

            Assert.True(result);
            reportRepositoryMock.Verify(x => x.Update(report), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GivenReportNotReplied_WhenUpdateReply_ThenShouldWork()
        {
            Report report = new Report
            {
                Reply = null
            };
            reportRepositoryMock.Setup(x => x.GetReportsDetailSingle(It.IsAny<Expression<Func<Report, bool>>>())).Returns(report);

            bool result = await reportService.UpdateReply("ReportID", new CodeStudy.Models.ReplyInput
            {
                Content = "ReplyContent",
            });

            Assert.False(result);
            reportRepositoryMock.Verify(x => x.Update(report), Times.Never);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task GivenReportAlreadyReplied_WhenDeleteReply_ThenShouldWork()
        {
            Report report = new Report
            {
                Reply = new Reply()
            };
            reportRepositoryMock.Setup(x => x.GetReportsDetailSingle(It.IsAny<Expression<Func<Report, bool>>>())).Returns(report);

            bool result = await reportService.RemoveReply("ReportID");

            Assert.True(result);
            replyRepositoryMock.Verify(x => x.Remove(report.Reply), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GivenReportNotReplied_WhenDeleteReply_ThenShouldWork()
        {
            Report report = new Report
            {
                Reply = null
            };
            reportRepositoryMock.Setup(x => x.GetReportsDetailSingle(It.IsAny<Expression<Func<Report, bool>>>())).Returns(report);

            bool result = await reportService.RemoveReply("ReportID");

            Assert.False(result);
            replyRepositoryMock.Verify(x => x.Remove(report.Reply), Times.Never);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }
    }
}
