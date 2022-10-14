using API.Controllers;
using CodeStudy.Models;
using Data.Common;
using Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Moq;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ControllerTest
{
    public class ReportControllerTest
    {
        private readonly Mock<IProblemService> problemServiceMock;
        private readonly Mock<IReportService> reportServiceMock;

        private readonly ReportController reportController;

        public ReportControllerTest()
        {
            problemServiceMock = new Mock<IProblemService>();
            reportServiceMock = new Mock<IReportService>();

            reportController = new ReportController(reportServiceMock.Object, problemServiceMock.Object);
        }

        #region GetByIdTest
        [Fact]
        public void GivenInvalidReport_WhenGetById_ThenShouldReturnNotFound()
        {
            SetUpGetDetailReportById(isReturnNull: true);

            var result = reportController.GetByID("invalid_report_id");
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact] 
        public void GivenAnonymousAccess_WhenGetById_ThenShouldReturnForbid()
        {
            // Not author, not admin
            SetupClaim(false, false);
            SetUpGetDetailReportById();

            var result = reportController.GetByID("report_id");
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void GivenAuthorAccess_WhenGetById_ThenShouldReturnOk()
        {
            // author, not admin
            SetupClaim(true, false);
            SetUpGetDetailReportById();

            var result = reportController.GetByID("report_id");
            var notFoundResult = Assert.IsType<OkObjectResult>(result);
            var returnObj = Assert.IsType<ReportDTO>(notFoundResult.Value);

            Assert.NotNull(returnObj);
            Assert.True(returnObj.ID == "report_id");
        }

        [Fact]
        public void GivenAdminAccess_WhenGetById_ThenShouldReturnOk()
        {
            // not author, not admin
            SetupClaim(false, true);
            SetUpGetDetailReportById();

            var result = reportController.GetByID("report_id");
            var notFoundResult = Assert.IsType<OkObjectResult>(result);
            var returnObj = Assert.IsType<ReportDTO>(notFoundResult.Value);

            Assert.NotNull(returnObj);
            Assert.True(returnObj.ID == "report_id");
        }
        #endregion

        #region UpdateTest
        [Fact]
        public async Task GivenInvalidReport_WhenUpdate_ThenShouldReturnNotFound()
        {
            SetupFindByReportId(isReturnNull: true);

            var result = await reportController.Update("invalid_report_id", new ReportInput());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public async Task GivenAnonymousAccess_WhenUpdate_ThenShouldReturnForbid()
        {
            // Not author, not admin
            SetupClaim(false, false);
            SetupFindByReportId();

            var result = await reportController.Update("report_id", new ReportInput());
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GivenAuthorAccess_WhenUpdate_ThenShouldReturnOk()
        {
            // author, not admin
            SetupClaim(true, false);
            SetupFindByReportId();

            var result = await reportController.Update("report_id", new ReportInput());
            var okResult = Assert.IsType<NoContentResult>(result);

            reportServiceMock.Verify(x => x.Update(It.IsAny<string>(), It.IsAny<ReportInput>()), Times.Once);
        }
        #endregion

        #region DeleteTest
        [Fact]
        public async Task GivenInvalidReport_WhenDelete_ThenShouldReturnNotFound()
        {
            SetupFindByReportId(isReturnNull: true);

            var result = await reportController.Delete("invalid_report_id");
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public async Task GivenAnonymousAccess_WhenDelete_ThenShouldReturnForbid()
        {
            // not author, not admin
            SetupClaim(false, false);
            SetupFindByReportId();

            var result = await reportController.Delete("report_id");
            var forbidResult = Assert.IsType<ForbidResult>(result);

            reportServiceMock.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GivenAuthorAccess_WhenDelete_ThenShouldReturnOk()
        {
            // author, not admin
            SetupClaim(true, false);
            SetupFindByReportId();

            var result = await reportController.Delete("report_id");
            var nocontentResult = Assert.IsType<NoContentResult>(result);

            reportServiceMock.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GivenAdminAccess_WhenDelete_ThenShouldReturnOk()
        {
            // not author, admin
            SetupClaim(false, true);
            SetupFindByReportId();

            var result = await reportController.Delete("report_id");
            var nocontentResult = Assert.IsType<NoContentResult>(result);

            reportServiceMock.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
        }
        #endregion

        #region ReplyTest
        [Fact]
        public async Task GivenInvalidReport_WhenReply_ThenShouldReturnNotFound()
        {
            SetupFindByReportId(isReturnNull: true);

            var result = await reportController.Reply("report_id", new ReplyInput());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
             Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public async Task GivenAnonymousAccess_WhenReply_ThenShouldReturnForbid()
        {
            // not admin, not author
            SetupClaim(false, false);
            SetupFindByReportId();
            SetUpFindProblemById();

            var result = await reportController.Reply("report_id", new ReplyInput());
            Assert.IsType<ForbidResult>(result); 
        }

        [Fact]
        public async Task GivenAuthorizedAccess_WhenReply_ThenShouldReturnOk()
        {
            // admin, author
            SetupClaim(true, true);
            SetupFindByReportId();
            SetUpFindProblemById();

            // Reply success, report hasn't been replied yet.
            reportServiceMock.Setup(x => x.Reply(It.IsAny<string>(), It.IsAny<ReplyInput>())).Returns(Task.FromResult(true));

            var result = await reportController.Reply("report_id", new ReplyInput());
            var noContentResult = Assert.IsType<NoContentResult>(result);

            reportServiceMock.Verify(x => x.Reply(It.IsAny<string>(), It.IsAny<ReplyInput>()), Times.Once);
        }

        [Fact]
        public async Task GivenReportAlreadyReply_WhenReply_ThenShouldRetunConflict()
        {
            // admin, author
            SetupClaim(true, true);
            SetupFindByReportId();
            SetUpFindProblemById();

            // Reply failure, report has already been replied.
            reportServiceMock.Setup(x => x.Reply(It.IsAny<string>(), It.IsAny<ReplyInput>())).Returns(Task.FromResult(false));

            var result = await reportController.Reply("report_id", new ReplyInput());
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);

            reportServiceMock.Verify(x => x.Reply(It.IsAny<string>(), It.IsAny<ReplyInput>()), Times.Once);
        }
        #endregion

        #region UpdateReplyTest
        [Fact]
        public async Task GivenInvalidReport_WhenUpdateReply_ThenShouldReturnNotFound()
        {
            SetupFindByReportId(isReturnNull: true);

            var result = await reportController.UpdateReply("report_id", new ReplyInput());
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GivenAnonymousAccess_WhenUpteReply_ThenShouldReturnForbid()
        {
            // not admin, not author
            SetupClaim(false, false);
            SetupFindByReportId();
            SetUpFindProblemById();

            var result = await reportController.UpdateReply("report_id", new ReplyInput());
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GivenAuthorizedAccess_WhenUpdateReply_ThenShouldReturnOk()
        {
            // admin, author
            SetupClaim(true, true);
            SetupFindByReportId();
            SetUpFindProblemById();

            // Update Reply success
            reportServiceMock.Setup(x => x.UpdateReply(It.IsAny<string>(), It.IsAny<ReplyInput>())).Returns(Task.FromResult(true));

            var result = await reportController.UpdateReply("report_id", new ReplyInput());
            var noContentResult = Assert.IsType<NoContentResult>(result);

            reportServiceMock.Verify(x => x.UpdateReply(It.IsAny<string>(), It.IsAny<ReplyInput>()), Times.Once);
        }

        [Fact]
        public async Task GivenReportNotBeenReplied_WhenUpdateReply_ThenShouldReturnNotFound()
        {
            // admin, author
            SetupClaim(true, true);
            SetupFindByReportId();
            SetUpFindProblemById();

            // Update Reply failure, report hasn't been reply yet
            reportServiceMock.Setup(x => x.UpdateReply(It.IsAny<string>(), It.IsAny<ReplyInput>())).Returns(Task.FromResult(false));

            var result = await reportController.UpdateReply("report_id", new ReplyInput());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            reportServiceMock.Verify(x => x.UpdateReply(It.IsAny<string>(), It.IsAny<ReplyInput>()), Times.Once);
        }
        #endregion

        #region DeleteReplyTest
        [Fact]
        public async Task GivenInvalidReport_WhenDeleteReply_ThenShouldReturnNotFound()
        {
            SetupFindByReportId(isReturnNull: true);

            var result = await reportController.DeleteReply("report_id");
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GivenAnonymousAccess_WhenDeleteReply_ThenShouldReturnForbid()
        {
            // not admin, not author
            SetupClaim(false, false);
            SetupFindByReportId();
            SetUpFindProblemById();

            var result = await reportController.DeleteReply("report_id");
            var forbidResult = Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GivenAuthorizedAccess_WhenDeleteReply_ThenShouldReturnOk()
        {
            // admin, author
            SetupClaim(true, true);
            SetupFindByReportId();
            SetUpFindProblemById();

            // Remove Reply success
            reportServiceMock.Setup(x => x.RemoveReply(It.IsAny<string>())).Returns(Task.FromResult(true));

            var result = await reportController.DeleteReply("report_id");
            var noContentResult = Assert.IsType<NoContentResult>(result);

            reportServiceMock.Verify(x => x.RemoveReply(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GivenReportNotBeenReplied_WhenDeleteReply_ThenShouldReturnNotFound()
        {
            // admin, author
            SetupClaim(true, true);
            SetupFindByReportId();
            SetUpFindProblemById();

            // Remove Reply failure, report hasn't been reply yet
            reportServiceMock.Setup(x => x.RemoveReply(It.IsAny<string>())).Returns(Task.FromResult(false));

            var result = await reportController.DeleteReply("report_id");
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            reportServiceMock.Verify(x => x.RemoveReply(It.IsAny<string>()), Times.Once);
        }
        #endregion

        #region MockSetup
        private void SetupClaim(bool isAuthor, bool isAdmin)
        {
            reportController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(Constant.ID, isAuthor ? "author_id" : "anonymous_id"),
                        new Claim(Constant.ROLE, isAdmin ? "Admin" : "User")
                    }))
                }
            };
        }

        private void SetupFindByReportId(bool isReturnNull = false)
        {
            reportServiceMock.Setup(x => x.FindByID(It.IsAny<string>())).Returns(isReturnNull ? null : new Report
            {
                ID = "report_id",
                UserID = "author_id"
            });
        }

        private void SetUpGetDetailReportById(bool isReturnNull = false)
        {
            reportServiceMock.Setup(x => x.GetDetailById(It.IsAny<string>())).Returns(isReturnNull ? null : new ReportDTO
            {
                ID = "report_id",
                User = new UserDTO
                {
                    ID = "author_id"
                }
            });
        }

        private void SetUpFindProblemById()
        {
            problemServiceMock.Setup(x => x.FindByID(It.IsAny<string>())).Returns(new Problem
            {
                ID = "problem_id",
                ArticleID = "author_id"
            });
        }
        #endregion
    }
}
