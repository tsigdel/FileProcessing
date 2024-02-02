using FileProcessing.Interfaces;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileProcessing.Services
{
    /// <summary>
    /// Implementation of the <see cref="IFileProcessor"/> interface for processing text files.
    /// </summary>
    public class FileProcessor : IFileProcessor
    {
        private readonly IWordProcessor _wordProcessor;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProcessor"/> class.
        /// </summary>
        /// <param name="logger">An instance of the <see cref="ILogger"/> used for logging.</param>
        /// <param name="wordProcessor">An instance of the <see cref="IWordProcessor"/> used for word processing.</param>
        public FileProcessor(ILogger logger, IWordProcessor wordProcessor)
        {
            _wordProcessor = wordProcessor ?? throw new ArgumentNullException(nameof(wordProcessor));

            // Validate logger
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Log the initialization
            _logger.Information("FileProcessor initialized.");
        }

        /// <summary>
        /// Processes a text file asynchronously, extracting words and calculating word frequencies.
        /// </summary>
        /// <param name="inputFilePath">The path to the input text file.</param>
        /// <returns>A <see cref="ConcurrentDictionary{TKey, TValue}"/> containing word frequencies.</returns>
        public async Task<ConcurrentDictionary<string, int>> ProcessFileAsync(string inputFilePath)
        {
            try
            {
                _logger.Information($"Processing file: {inputFilePath}");
                string[] words = await ExtractFileWordsAsync(inputFilePath);

                return _wordProcessor.ProcessWords(words);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error processing file: {inputFilePath}");
                throw; // Rethrow the exception after logging
            }
        }

        /// <summary>
        /// Extracts words from a text file asynchronously using regular expressions.
        /// </summary>
        /// <param name="inputFilePath">The path to the input text file.</param>
        /// <returns>An array of strings representing the extracted words.</returns>
        public async Task<string[]> ExtractFileWordsAsync(string inputFilePath)
        {
            try
            {
                ValidateInputFile(inputFilePath);

                _logger.Information($"Extracting words from file: {inputFilePath}");

                var text = await File.ReadAllTextAsync(inputFilePath);

                var words = Regex.Split(text, @"[\s\p{P}]")
                                 .Where(word => !string.IsNullOrWhiteSpace(word))
                                 .ToArray();

                return words;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error extracting words from file: {inputFilePath}");
                throw; // Rethrow the exception after logging
            }
        }

        /// <summary>
        /// Writes word frequencies to an output file, sorted by frequency and then alphabetically.
        /// </summary>
        /// <param name="outputFilePath">The path to the output text file.</param>
        /// <param name="wordFrequencies">A <see cref="ConcurrentDictionary{TKey, TValue}"/> containing word frequencies.</param>
        public void WriteOutputFile(string outputFilePath, ConcurrentDictionary<string, int> wordFrequencies)
        {
            try
            {
                ValidateOutputFile(outputFilePath);
                _logger.Information($"Writing output to file: {outputFilePath}");

                var sortedWords = wordFrequencies.OrderByDescending(kvPair => kvPair.Value)
                                    .ThenBy(kvPair => kvPair.Key, StringComparer.OrdinalIgnoreCase)
                                    .Select(kvPair => $"{kvPair.Key},{kvPair.Value}");

                File.WriteAllLines(outputFilePath, sortedWords);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error writing output to file: {outputFilePath}");
                throw; // Rethrow the exception after logging
            }
        }

        /// <summary>
        /// Validates the input file, checking if it exists and has a valid name.
        /// </summary>
        /// <param name="inputFilePath">The path to the input text file.</param>
        public void ValidateInputFile(string inputFilePath)
        {
            try
            {
                _logger.Information($"Validating input file: {inputFilePath}");
                if (string.IsNullOrWhiteSpace(inputFilePath))
                {
                    throw new ArgumentException("Input file name cannot be empty or whitespace.");
                }

                if (!File.Exists(inputFilePath))
                {
                    throw new FileNotFoundException($"Input file '{inputFilePath}' not found.");
                }

                _logger.Debug($"Validation completed for input file: {inputFilePath}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error validating input file: {inputFilePath}");
                throw; // Rethrow the exception after logging
            }
        }

        /// <summary>
        /// Validates the output file, checking if it has a valid name and write permissions.
        /// </summary>
        /// <param name="outputFilePath">The path to the output text file.</param>
        public void ValidateOutputFile(string outputFilePath)
        {
            try
            {
                _logger.Information($"Validating output file: {outputFilePath}");
                if (string.IsNullOrWhiteSpace(outputFilePath))
                {
                    throw new ArgumentException("Output file name cannot be empty or whitespace.");
                }

                string directory = Path.GetDirectoryName(outputFilePath);
                if (!Directory.Exists(directory))
                {
                    throw new DirectoryNotFoundException($"Output directory '{directory}' not found.");
                }

                try
                {
                    File.Create(outputFilePath).Close();
                    File.Delete(outputFilePath);
                }
                catch (UnauthorizedAccessException)
                {
                    throw new UnauthorizedAccessException($"No write permission for the output file '{outputFilePath}'.");
                }
                catch
                {
                    throw;
                }

                _logger.Debug($"Validation completed for output file: {outputFilePath}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error validating output file: {outputFilePath}");
                throw; // Rethrow the exception after logging
            }
        }
    }
}
