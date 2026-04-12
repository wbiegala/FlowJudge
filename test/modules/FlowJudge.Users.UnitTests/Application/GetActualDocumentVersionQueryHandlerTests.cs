using Moq;
using FlowJudge.Users.Application.Models;
using FlowJudge.Users.Application.Queries;
using FlowJudge.Users.Domain.Model;
using FlowJudge.Users.Infrastructure;

namespace FlowJudge.Users.UnitTests.Application
{
    public class GetActualDocumentVersionQueryHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WhenTermsAndConditionsVersionExists_ThenReturnSuccessWithTextContent()
        {
            // Arrange
            var documentVersion = Fixture.CreateTermsAndConditionsVersion(
                VersionId,
                VersionNumber,
                TextContent,
                HtmlContent,
                CreationTimestamp,
                isAcceptable: true);

            _documentVersionRepositoryMock.Setup(dvr => dvr.GetActualDocumentVersionAsync<TermsAndConditionsVersion>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(documentVersion);

            var query = new GetActualDocumentVersionQuery
            {
                Kind = DocumentKind.TermsAndConditions,
                ContentType = DocumentContentType.Text
            };
            var handler = new GetActualDocumentVersionQueryHandler(_documentVersionRepositoryMock.Object);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(DocumentKind.TermsAndConditions, result.Data.Kind);
            Assert.Equal(VersionId, result.Data.VersionId);
            Assert.Equal(VersionNumber, result.Data.VersionNumber);
            Assert.Equal(TextContent, result.Data.Content);
            _documentVersionRepositoryMock.Verify(dvr => dvr.GetActualDocumentVersionAsync<TermsAndConditionsVersion>(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenTermsAndConditionsVersionExists_ThenReturnSuccessWithHtmlContent()
        {
            // Arrange
            var documentVersion = Fixture.CreateTermsAndConditionsVersion(
                VersionId,
                VersionNumber,
                TextContent,
                HtmlContent,
                CreationTimestamp,
                isAcceptable: true);

            _documentVersionRepositoryMock.Setup(dvr => dvr.GetActualDocumentVersionAsync<TermsAndConditionsVersion>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(documentVersion);

            var query = new GetActualDocumentVersionQuery
            {
                Kind = DocumentKind.TermsAndConditions,
                ContentType = DocumentContentType.Html
            };
            var handler = new GetActualDocumentVersionQueryHandler(_documentVersionRepositoryMock.Object);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(DocumentKind.TermsAndConditions, result.Data.Kind);
            Assert.Equal(VersionId, result.Data.VersionId);
            Assert.Equal(VersionNumber, result.Data.VersionNumber);
            Assert.Equal(HtmlContent, result.Data.Content);
            _documentVersionRepositoryMock.Verify(dvr => dvr.GetActualDocumentVersionAsync<TermsAndConditionsVersion>(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenPrivacyPolicyVersionExists_ThenReturnSuccessWithTextContent()
        {
            // Arrange
            var documentVersion = Fixture.CreatePrivacyPolicyVersion(
                VersionId,
                VersionNumber,
                TextContent,
                HtmlContent,
                CreationTimestamp,
                isAcceptable: true);

            _documentVersionRepositoryMock.Setup(dvr => dvr.GetActualDocumentVersionAsync<PrivacyPolicyVersion>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(documentVersion);

            var query = new GetActualDocumentVersionQuery
            {
                Kind = DocumentKind.PrivacyPolicy,
                ContentType = DocumentContentType.Text
            };
            var handler = new GetActualDocumentVersionQueryHandler(_documentVersionRepositoryMock.Object);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(DocumentKind.PrivacyPolicy, result.Data.Kind);
            Assert.Equal(VersionId, result.Data.VersionId);
            Assert.Equal(VersionNumber, result.Data.VersionNumber);
            Assert.Equal(TextContent, result.Data.Content);
            _documentVersionRepositoryMock.Verify(dvr => dvr.GetActualDocumentVersionAsync<PrivacyPolicyVersion>(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenPrivacyPolicyVersionExists_ThenReturnSuccessWithHtmlContent()
        {
            // Arrange
            var documentVersion = Fixture.CreatePrivacyPolicyVersion(
                VersionId,
                VersionNumber,
                TextContent,
                HtmlContent,
                CreationTimestamp,
                isAcceptable: true);

            _documentVersionRepositoryMock.Setup(dvr => dvr.GetActualDocumentVersionAsync<PrivacyPolicyVersion>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(documentVersion);

            var query = new GetActualDocumentVersionQuery
            {
                Kind = DocumentKind.PrivacyPolicy,
                ContentType = DocumentContentType.Html
            };
            var handler = new GetActualDocumentVersionQueryHandler(_documentVersionRepositoryMock.Object);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(DocumentKind.PrivacyPolicy, result.Data.Kind);
            Assert.Equal(VersionId, result.Data.VersionId);
            Assert.Equal(VersionNumber, result.Data.VersionNumber);
            Assert.Equal(HtmlContent, result.Data.Content);
            _documentVersionRepositoryMock.Verify(dvr => dvr.GetActualDocumentVersionAsync<PrivacyPolicyVersion>(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenDocumentVersionDoesNotExist_ThenReturnFailure()
        {
            // Arrange
            _documentVersionRepositoryMock.Setup(dvr => dvr.GetActualDocumentVersionAsync<TermsAndConditionsVersion>(It.IsAny<CancellationToken>()))
                .ReturnsAsync((TermsAndConditionsVersion)null);

            var query = new GetActualDocumentVersionQuery
            {
                Kind = DocumentKind.TermsAndConditions,
                ContentType = DocumentContentType.Text
            };
            var handler = new GetActualDocumentVersionQueryHandler(_documentVersionRepositoryMock.Object);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            _documentVersionRepositoryMock.Verify(dvr => dvr.GetActualDocumentVersionAsync<TermsAndConditionsVersion>(It.IsAny<CancellationToken>()), Times.Once);
        }

        private readonly Mock<IDocumentVersionRepository> _documentVersionRepositoryMock = new();

        private static readonly Guid VersionId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        private const int VersionNumber = 1;
        private const string TextContent = "Test text content";
        private const string HtmlContent = "<p>Test HTML content</p>";
        private static readonly DateTimeOffset CreationTimestamp = DateTimeOffset.UtcNow;
    }
}
