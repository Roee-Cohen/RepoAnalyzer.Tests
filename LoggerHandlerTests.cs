using Microsoft.Extensions.Logging;
using Moq;
using RepoAnalyzer.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoAnalyzer.Tests
{
    public class LoggerHandlerTests
    {
        [Fact]
        public void ProcessFavorite_ValidInput_ReturnsSuccess()
        {
            Debug.WriteLine("Here");
            // Arrange
            var loggerMock = new Mock<ILogger<LoggerHandler>>();
            var handler = new LoggerHandler(loggerMock.Object);

            // Act
            var result = handler.ProcessFavorite("repo123", "user456");

            // Assert
            Assert.Equal("Success", result);
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Processed favorite")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void ProcessFavorite_InvalidInput_ReturnsFailed()
        {
            var loggerMock = new Mock<ILogger<LoggerHandler>>();
            var handler = new LoggerHandler(loggerMock.Object);

            var result = handler.ProcessFavorite(null, "user456");

            Assert.Equal("Failed", result);
            loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Invalid input")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
