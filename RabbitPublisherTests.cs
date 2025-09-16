using Moq;
using RepoAnalyzer.Core.Interfaces;
using RepoAnalyzer.Models;
using RepoAnalyzer.Tests.Utils;

namespace RepoAnalyzer.Tests
{
    public class RabbitPublisherTests
    {
        [Fact]
        public async Task ProcessFavorite_ValidInput_CallsPublisher()
        {
            var publisherMock = new Mock<IRabbitProducer>();
            var handler = new PublisherHandler(publisherMock.Object);

            var analysis = new RepoAnalysis()
            {
                ActivityDays = 1,
                AnalyzedAt = DateTime.Now,
                DefaultBranch = "master",
                Forks = 1,
                RepoId = "11",
                Name = "test"
            };
            var result = await handler.ProcessFavorite(analysis);

            Assert.Equal("Success", result);
            publisherMock.Verify(p => p.PublishAsync(It.IsAny<RepoAnalysis>()), Times.Once);
        }
    }
}
