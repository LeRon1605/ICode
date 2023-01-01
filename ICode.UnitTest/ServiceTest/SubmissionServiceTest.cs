using AutoMapper;
using Data.Repository.Interfaces;
using Data.Repository;
using Moq;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using API.Services;
using Xunit;
using System.Threading.Tasks;
using CodeStudy.Models;
using Data.Entity;
using Models.DTO;
using UnitTest.Common;
using UnitTest.Data;

namespace UnitTest.ServiceTest
{
    public class SubmissionServiceTest
    {
        private readonly ISubmissionService submissionService;

        private readonly Mock<ICodeExecutor> codeExecutorMock;
        private readonly Mock<ISubmissionRepository> submissionRepositoryMock;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITestcaseService> testCaseServiceMock;
        private readonly Mock<IMapper> mapperMock;

        public SubmissionServiceTest()
        {
            codeExecutorMock = new Mock<ICodeExecutor>();
            submissionRepositoryMock = new Mock<ISubmissionRepository>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            testCaseServiceMock = new Mock<ITestcaseService>();
            mapperMock = new Mock<IMapper>();

            unitOfWorkMock.Setup(x => x.CommitAsync());
            submissionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Submission>()));
            mapperMock.Setup(x => x.Map<Submission, SubmissionResult>(It.IsAny<Submission>())).Callback<Submission>(x => new SubmissionResult
            {
                Status = x.State == ICode.Common.SubmitState.Success
            });

            submissionService = new SubmissionService(submissionRepositoryMock.Object, unitOfWorkMock.Object, testCaseServiceMock.Object, codeExecutorMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task GivenCorrectCode_WhenSubmit_ThenShouldError()
        {
            testCaseServiceMock.Setup(x => x.GetTestcaseOfProblem(It.IsAny<string>())).Returns(TestcaseData.GetAll());
            codeExecutorMock.Setup(x => x.ExecuteCode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new ExecutorResult
            {
                cpuTime = "0.0001",
                memory = "500",
                output = "1",
                statusCode = 200
            }));

            Submission submission = SubmissionData.GetSubmission();

            SubmissionResult result = await submissionService.Submit(submission, "Problem");

            Assert.True(submission.State == ICode.Common.SubmitState.Success);

            testCaseServiceMock.Verify(x => x.GetTestcaseOfProblem(It.IsAny<string>()), Times.AtLeastOnce);
            submissionRepositoryMock.Verify(x => x.AddAsync(submission), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);

        }

        [Fact]
        public async Task GivenInvalidCode_WhenSubmit_ThenShouldError()
        {
            // Invalid code return output incorrect
            testCaseServiceMock.Setup(x => x.GetTestcaseOfProblem(It.IsAny<string>())).Returns(TestcaseData.GetAll());
            codeExecutorMock.Setup(x => x.ExecuteCode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new ExecutorResult
            {
                cpuTime = "0.0001",
                memory = "500",
                output = "2",
                statusCode = 200
            }));

            Submission submission = SubmissionData.GetSubmission();

            SubmissionResult result = await submissionService.Submit(submission, "Problem");

            Assert.False(submission.State == ICode.Common.SubmitState.Success);

            testCaseServiceMock.Verify(x => x.GetTestcaseOfProblem(It.IsAny<string>()), Times.AtLeastOnce);
            submissionRepositoryMock.Verify(x => x.AddAsync(submission), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);

        }

        [Fact]
        public async Task GivenLimitResourceCode_WhenSubmit_ThenShouldError()
        {
            // Correct answer but Time limit or memory limit
            testCaseServiceMock.Setup(x => x.GetTestcaseOfProblem(It.IsAny<string>())).Returns(TestcaseData.GetAll());
            codeExecutorMock.Setup(x => x.ExecuteCode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new ExecutorResult
            {
                cpuTime = "0.0001",
                memory = "10000",
                output = "1",
                statusCode = 200
            }));

            Submission submission = SubmissionData.GetSubmission();

            SubmissionResult result = await submissionService.Submit(submission, "Problem");

            Assert.False(submission.State == ICode.Common.SubmitState.Success);

            testCaseServiceMock.Verify(x => x.GetTestcaseOfProblem(It.IsAny<string>()), Times.AtLeastOnce);
            submissionRepositoryMock.Verify(x => x.AddAsync(submission), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GivenInvalidProblemId_WhenSubmit_ThenReturnNull()
        {
            testCaseServiceMock.Setup(x => x.GetTestcaseOfProblem(It.IsAny<string>())).Returns(new List<TestcaseDTO>());

            Submission submission = SubmissionData.GetSubmission();

            SubmissionResult result = await submissionService.Submit(submission, "Invalid_Problem");

            Assert.False(submission.State == ICode.Common.SubmitState.Success);

            testCaseServiceMock.Verify(x => x.GetTestcaseOfProblem(It.IsAny<string>()), Times.AtLeastOnce);
            submissionRepositoryMock.Verify(x => x.AddAsync(submission), Times.Once);
            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}
