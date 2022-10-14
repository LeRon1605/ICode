using API.Controllers;
using AutoMapper;
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
    public class TestcaseControllerTest
    {
        private readonly TestcaseController testcaseController;

        private readonly Mock<ITestcaseService> testcaseServiceMock;
        private readonly Mock<IProblemService> problemServiceMock;
        private readonly Mock<IMapper> mapperMock;

        public TestcaseControllerTest()
        {
            testcaseServiceMock = new Mock<ITestcaseService>();
            problemServiceMock = new Mock<IProblemService>();
            mapperMock = new Mock<IMapper>();

            testcaseController = new TestcaseController(testcaseServiceMock.Object, problemServiceMock.Object, mapperMock.Object);
        }

        [Fact]
        public void GivenNotExistTestcaseId_WhenGetById_ThenShouldReturnNotFound()
        {
            SetupFindTestcase(true);

            var result = testcaseController.GetTestcaseByID("testcase_id");
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultObj = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public void GivenNotExistProblem_WhenGetById_ThenShouldReturnNotFound()
        {
            SetupFindTestcase(false);
            SetupFindProblem(false, true);

            var result = testcaseController.GetTestcaseByID("testcase_id");
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultObj = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public void GivenNotAuthorAccess_WhenGetById_ThenShouldReturnForbid()
        {
            // Login as User role and not a author
            SetupClaim(false, false);
            
            SetupFindTestcase(false);
            SetupFindProblem(false, false);

            var result = testcaseController.GetTestcaseByID("testcase_id");
            var forbidResult = Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void GivenAuthorAccess_WhenGetById_ThenShouldReturnOk()
        {
            // Login as User role and author
            SetupClaim(true, false);

            SetupFindTestcase(false);
            SetupFindProblem(true, false);

            var result = testcaseController.GetTestcaseByID("testcase_id");
            var okResult = Assert.IsType<OkObjectResult>(result);
            //var returnObj = Assert.IsType<TestcaseDTO>(okResult.Value);
        }

        [Fact]
        public void GivenAdminAccess_WhenGetById_ThenShouldReturnOk()
        {
            // Login as Admin role and not author
            SetupClaim(false, true);

            SetupFindTestcase(false);
            SetupFindProblem(false, false);

            var result = testcaseController.GetTestcaseByID("testcase_id");
            var okResult = Assert.IsType<OkObjectResult>(result);
            //var returnObj = Assert.IsType<TestcaseDTO>(okResult.Value);
        }

        [Fact]
        public async Task GivenNotExistTestcaseId_WhenUpdate_ThenShouldReturnNotFound()
        {
            SetupFindTestcase(true);

            var result = await testcaseController.UpdateTestcase("testcase_id", new TestcaseInput());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultObj = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public async Task GivenNotExistProblem_WhenUpdate_ThenShouldReturnNotFound()
        {
            SetupFindTestcase(false);
            SetupFindProblem(false, true);

            var result = await testcaseController.UpdateTestcase("testcase_id", new TestcaseInput());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultObj = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public async Task GivenNotAuthorAccess_WhenUpdate_ThenShouldReturnForbid()
        {
            // Login as User role and not a author
            SetupClaim(false, false);

            SetupFindTestcase(false);
            SetupFindProblem(false, false);

            var result = await testcaseController.UpdateTestcase("testcase_id", new TestcaseInput());
            var forbidResult = Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GivenAuthorAccess_WhenUpdate_ThenShouldReturnNoContent()
        {
            // Login as User role and author
            SetupClaim(true, false);

            SetupFindTestcase(false);
            SetupFindProblem(true, false);

            var result = await testcaseController.UpdateTestcase("testcase_id", new TestcaseInput());
            var okResult = Assert.IsType<NoContentResult>(result);
            //var returnObj = Assert.IsType<TestcaseDTO>(okResult.Value);
            testcaseServiceMock.Verify(x => x.Update("testcase_id", It.IsAny<TestcaseInput>()), Times.Once);
        }

        [Fact]
        public async Task GivenAdminAccess_WhenUpdate_ThenShouldReturnNoContent()
        {
            // Login as Admin role and not author
            SetupClaim(false, true);

            SetupFindTestcase(false);
            SetupFindProblem(false, false);

            var result = await testcaseController.UpdateTestcase("testcase_id", new TestcaseInput());
            var okResult = Assert.IsType<NoContentResult>(result);
            //var returnObj = Assert.IsType<TestcaseDTO>(okResult.Value);

            testcaseServiceMock.Verify(x => x.Update("testcase_id", It.IsAny<TestcaseInput>()), Times.Once);
        }

        [Fact]
        public async Task GivenNotExistTestcaseId_WhenDelete_ThenShouldReturnNotFound()
        {
            SetupFindTestcase(true);

            var result = await testcaseController.DeleteTestcase("testcase_id");
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultObj = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public async Task GivenNotExistProblem_WhenDelete_ThenShouldReturnNotFound()
        {
            SetupFindTestcase(false);
            SetupFindProblem(false, true);

            var result = await testcaseController.DeleteTestcase("testcase_id");
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultObj = Assert.IsType<ErrorResponse>(notFoundResult.Value);
        }

        [Fact]
        public async Task GivenNotAuthorAccess_WhenDelete_ThenShouldReturnForbid()
        {
            // Login as User role and not a author
            SetupClaim(false, false);

            SetupFindTestcase(false);
            SetupFindProblem(false, false);

            var result = await testcaseController.DeleteTestcase("testcase_id");
            var forbidResult = Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GivenAuthorAccess_WhenDelete_ThenShouldReturnNoContent()
        {
            // Login as User role and author
            SetupClaim(true, false);

            SetupFindTestcase(false);
            SetupFindProblem(true, false);

            var result = await testcaseController.DeleteTestcase("testcase_id");
            var okResult = Assert.IsType<NoContentResult>(result);
            //var returnObj = Assert.IsType<TestcaseDTO>(okResult.Value);
            testcaseServiceMock.Verify(x => x.Remove("testcase_id"), Times.Once);
        }

        [Fact]
        public async Task GivenAdminAccess_WhenDelete_ThenShouldReturnNoContent()
        {
            // Login as Admin role and not author
            SetupClaim(false, true);

            SetupFindTestcase(false);
            SetupFindProblem(false, false);

            var result = await testcaseController.DeleteTestcase("testcase_id");
            var okResult = Assert.IsType<NoContentResult>(result);
            //var returnObj = Assert.IsType<TestcaseDTO>(okResult.Value);

            testcaseServiceMock.Verify(x => x.Remove("testcase_id"), Times.Once);
        }

        private void SetupClaim(bool isAuthor, bool isAdmin)
        {
            testcaseController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(Constant.ID, isAuthor ? "author_id" : "not_author_id"),
                        new Claim(Constant.ROLE, isAdmin ? "Admin" : "User")
                    }))
                }
            };
        }

        private void SetupFindTestcase(bool isReturnNull)
        {
            testcaseServiceMock.Setup(x => x.FindByID(It.IsAny<string>())).Returns(value: isReturnNull ? null : new TestCase
            {
                ID = "testcase_id",
                ProblemID = "problem_id"
            });
        }

        private void SetupFindProblem(bool isAuthor, bool isReturnNull)
        {
            problemServiceMock.Setup(x => x.FindByID(It.IsAny<string>())).Returns(isReturnNull ? null : new Problem
            {
                ID = "problem_id",
                ArticleID = isAuthor ? "author_id" : "not_another_id" // compare when read from claim
            });
        }
    }
}
