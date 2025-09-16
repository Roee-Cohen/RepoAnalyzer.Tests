using RepoAnalyzer.Core.Interfaces;
using RepoAnalyzer.Models;

namespace RepoAnalyzer.Tests.Utils
{
    public class PublisherHandler
    {
        private readonly IRabbitProducer _publisher;

        public PublisherHandler(IRabbitProducer publisher)
        {
            _publisher = publisher;
        }

        public async Task<string> ProcessFavorite(RepoAnalysis analysis)
        {
            await _publisher.PublishAsync(analysis);
            return "Success";
        }
    }
}
