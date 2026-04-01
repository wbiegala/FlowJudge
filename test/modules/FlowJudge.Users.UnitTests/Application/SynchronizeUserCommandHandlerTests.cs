using Moq;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Users.Application.Commands;
using FlowJudge.Users.Domain.Model;
using FlowJudge.Users.Infrastructure;

namespace FlowJudge.Users.UnitTests.Application
{
    public class SynchronizeUserCommandHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenNoUserIdDb_ThenAddUserAsync()
        {
            // Arrange
            _userRepositoryMock.Setup(ur => ur.GetUserByIdentityIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var command = new SynchronizeUserCommand
            {
                UserId = UserIdentityId,
                Username = Username,
                Email = Email
            };
            var handler = new SynchronizeUserCommandHandler(_userRepositoryMock.Object, _uowMock.Object);

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.True(result.IsSuccess);
            _userRepositoryMock.Verify(ur => ur.GetUserByIdentityIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(ur => ur.AddUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenUserExistsIdDb_ThenSkip()
        {
            // Arrange
            _userRepositoryMock.Setup(ur => ur.GetUserByIdentityIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Fixture.CreateUser(Guid.Parse(UserId), Guid.Parse(UserIdentityId), Username, Email));

            var command = new SynchronizeUserCommand
            {
                UserId = UserIdentityId,
                Username = Username,
                Email = Email
            };
            var handler = new SynchronizeUserCommandHandler(_userRepositoryMock.Object, _uowMock.Object);

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.True(result.IsSuccess);
            _userRepositoryMock.Verify(ur => ur.GetUserByIdentityIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(ur => ur.AddUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();

        private const string UserId = "954abcb9-5819-4ad8-9625-43fd5161a17e";
        private const string UserIdentityId = "a990c46f-2b7b-4f3c-9bb5-aa522591e587";
        private const string Username = "testuser";
        private const string Email = "testuser@test.test";
    }
}
