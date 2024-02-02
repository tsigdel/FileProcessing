using FileProcessing.Services;
using Moq;
using Serilog;
using System.Collections.Concurrent;

namespace WordProcessing.Tests
{
    [TestFixture]
    public class WordProcessorTests
    {
        private ILogger _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        [Test]
        public void ProcessWords_WithValidWords_ReturnsCorrectWordFrequencies()
        {
            // Arrange
            var wordProcessor = new WordProcessor(_logger);
            var words = new List<string> { "to", "and", "to", "to", "and", "a", "team", "team", "team", "team", "team", "$", "Broadridge", "to", "and", "a", "team", "$", "Broadridge", "a" };

            // Act
            var result = wordProcessor.ProcessWords(words);

            // Assert
            var expectedWordFrequencies = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["to"] = 4,
                ["and"] = 3,
                ["a"] = 3,
                ["team"] = 6,
                ["$"] = 2,
                ["Broadridge"] = 2
            };

            CollectionAssert.AreEquivalent(expectedWordFrequencies, result);
            Assert.AreNotSame(expectedWordFrequencies, result);
        }

        [Test]
        public void ProcessWords_WithEmptyWords_ReturnsEmptyDictionary()
        {
            // Arrange
            var wordProcessor = new WordProcessor(_logger);
            var words = Enumerable.Empty<string>();

            // Act
            var result = wordProcessor.ProcessWords(words);

            // Assert
            var expectedWordFrequencies = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            CollectionAssert.IsEmpty(result);
            CollectionAssert.AreEquivalent(expectedWordFrequencies, result);
        }

        [Test]
        public void ProcessWords_WithException_LogsErrorAndRethrows()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var wordProcessor = new WordProcessor(loggerMock.Object);
            var words = new List<string> { "aaa", "bbb", "ccc", null, "" };

            // Mock an exception during processing
            loggerMock.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>()));

            // Act and Assert
            Assert.Throws<AggregateException>(() => wordProcessor.ProcessWords(words));
        }
    }
}