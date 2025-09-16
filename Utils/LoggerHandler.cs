using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace RepoAnalyzer.Tests.Utils
{
    public class LoggerHandler
    {
        private readonly ILogger<LoggerHandler> _logger;

        public LoggerHandler(ILogger<LoggerHandler> logger)
        {
            _logger = logger;
        }

        public string ProcessFavorite(string repoId, string userId)
        {
            if (string.IsNullOrEmpty(repoId) || string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Invalid input");
                return "Failed";
            }

            // simulate some processing
            _logger.LogInformation($"Processed favorite for {repoId} by {userId}");
            return "Success";
        }
    }
}
