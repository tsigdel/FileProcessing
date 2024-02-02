using FileProcessing.Interfaces;
using FileProcessing.Services;
using Moq;
using Serilog;
using System.Collections.Concurrent;

namespace FileProcessing.Tests
{
    [TestFixture]
    public class FileProcessorTests
    {
        private ILogger _logger;
        private IWordProcessor _wordProcessor;

        [SetUp]
        public void SetUp()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            _wordProcessor = new Mock<IWordProcessor>().Object;
        }

        [Test]
        public async Task ExtractFileWords_ValidInputFile_ReturnsCorrectWords()
        {
            // Arrange
            var fileProcessor = new FileProcessor(_logger, _wordProcessor);
            var expectedWords = new[] { "About", "the", "job", "At", "Broadridge", "we", "ve", "built", "a", "culture", "where", "the",
                "highest", "goal", "is", "to", "empower", "others", "to", "accomplish", "more", "If", "you", "re",
                "passionate", "about", "developing", "your", "career"  };

            // Act

            var inputFileName = @"..\..\..\Assets\Input.txt";
            var result = await fileProcessor.ExtractFileWordsAsync(inputFileName);

            // Assert
            CollectionAssert.AreEqual(expectedWords, result);
            Assert.That(result, Is.EqualTo(expectedWords));
        }

        [Test]
        public async Task ProcessFileAsync_ValidInputFile_ReturnsCorrectWordFrequencies()
        {
            // Arrange
            var wordProcessorMock = new Mock<IWordProcessor>();
            var fileProcessor = new FileProcessor(_logger, wordProcessorMock.Object);

            var inputFileName = @"..\..\..\Assets\Input.txt";
            var expectedWordFrequencies = new ConcurrentDictionary<string, int>
            {
                ["and"] = 3,
                ["to"] = 2,
                ["the"] = 1,
                ["a"] = 3,
                ["team"] = 2,
                ["$"] = 1
            };

            wordProcessorMock.Setup(wp => wp.ProcessWords(It.IsAny<string[]>())).Returns(expectedWordFrequencies);

            // Act
            var result = await fileProcessor.ProcessFileAsync(inputFileName);

            // Assert
            wordProcessorMock.Verify(wp => wp.ProcessWords(It.IsAny<string[]>()), Times.Once);
            CollectionAssert.AreEquivalent(expectedWordFrequencies, result);
        }

        [Test]
        public void WriteOutputFile_ValidOutputFile_WritesCorrectlySortedWordFrequencies()
        {
            // Arrange
            var fileProcessor = new FileProcessor(_logger, _wordProcessor);

            var outputFileName = @"..\..\..\Assets\Output.txt";

            var wordFrequencies = new ConcurrentDictionary<string, int>
            {
                ["and"] = 12,
                ["to"] = 10,
                ["the"] = 7,
                ["a"] = 5,
                ["team"] = 5,
                ["$"] = 4
            };

            // Act
            fileProcessor.WriteOutputFile(outputFileName, wordFrequencies);

            // Assert
            Assert.That(File.Exists(outputFileName), Is.True);

            var lines = File.ReadAllLines(outputFileName);
            Assert.AreEqual(6, lines.Length);
            Assert.AreEqual("and,12", lines[0]);
            Assert.AreEqual("to,10", lines[1]);
            Assert.AreEqual("the,7", lines[2]);
            Assert.AreEqual("a,5", lines[3]);
            Assert.AreEqual("team,5", lines[4]);
            Assert.AreEqual("$,4", lines[5]);
        }

        [Test]
        public void ValidateInputFile_ValidInputFile_DoesNotThrowException()
        {
            // Arrange
            var fileProcessor = new FileProcessor(_logger, _wordProcessor);
            var inputFileName = @"..\..\..\Assets\Input.txt";

            // Act and Assert
            Assert.DoesNotThrow(() => fileProcessor.ValidateInputFile(inputFileName));
        }

        [Test]
        public void ValidateOutputFile_ValidOutputFile_DoesNotThrowException()
        {
            // Arrange
            var fileProcessor = new FileProcessor(_logger, _wordProcessor);
            var outputFileName = @"..\..\..\Assets\Output.txt";

            // Act and Assert
            Assert.DoesNotThrow(() => fileProcessor.ValidateOutputFile(outputFileName));
        }

        [Test]
        public void ValidateInputFile_NullInputFileName_ThrowsArgumentException()
        {
            // Arrange
            var fileProcessor = new FileProcessor(_logger, _wordProcessor);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => fileProcessor.ValidateInputFile(null));
        }

        [Test]
        public void ValidateInputFile_NonexistentInputFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var fileProcessor = new FileProcessor(_logger, _wordProcessor);

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => fileProcessor.ValidateInputFile("nonexistent_input.txt"));
        }

        [Test]
        public void ValidateOutputFile_NullOutputFileName_ThrowsArgumentException()
        {
            // Arrange
            var fileProcessor = new FileProcessor(_logger, _wordProcessor);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => fileProcessor.ValidateOutputFile(null));
        }

        [Test]
        public void ValidateOutputFile_NonexistentOutputDirectory_ThrowsDirectoryNotFoundException()
        {
            // Arrange
            var fileProcessor = new FileProcessor(_logger, _wordProcessor);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => fileProcessor.ValidateOutputFile("nonexistent_directory/nonexistent_output.txt"));
        }
    }

}
