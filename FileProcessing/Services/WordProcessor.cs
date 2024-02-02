using FileProcessing.Interfaces;
using Serilog;
using System.Collections.Concurrent;

namespace FileProcessing.Services
{
    /// <summary>
    /// Implementation of the <see cref="IWordProcessor"/> interface for processing collections of words.
    /// </summary>
    public class WordProcessor : IWordProcessor
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordProcessor"/> class.
        /// </summary>
        /// <param name="logger">The logger instance used for logging.</param>
        public WordProcessor(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Processes a collection of words, calculating the frequency of each unique word in parallel.
        /// </summary>
        /// <param name="words">An enumerable collection of words to be processed.</param>
        /// <returns>
        /// A <see cref="ConcurrentDictionary{TKey, TValue}"/> containing word frequencies.
        /// The keys represent unique words, and the values represent their respective frequencies.
        /// </returns>
        public ConcurrentDictionary<string, int> ProcessWords(IEnumerable<string> words)
        {
            try
            {
                _logger.Information("Processing words...");
                return new ConcurrentDictionary<string, int>(
                    words.AsParallel()
                         .WithDegreeOfParallelism(Environment.ProcessorCount)
                         .GroupBy(word => word, StringComparer.OrdinalIgnoreCase)
                         .ToDictionary(group => group.Key, group => group.Count())
                );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing words");
                throw; // Rethrow the exception after logging
            }
        }
    }
}
