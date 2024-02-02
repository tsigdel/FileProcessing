using System.Collections.Concurrent;

namespace FileProcessing.Interfaces
{
    /// <summary>
    /// Defines the contract for a word processor that calculates the frequency of each unique word in a collection.
    /// </summary>
    public interface IWordProcessor
    {
        /// <summary>
        /// Processes a collection of words, calculating the frequency of each unique word.
        /// </summary>
        /// <param name="words">An enumerable collection of words to be processed.</param>
        /// <returns>
        /// A <see cref="ConcurrentDictionary{TKey, TValue}"/> containing word frequencies.
        /// The keys represent unique words, and the values represent their respective frequencies.
        /// </returns>
        ConcurrentDictionary<string, int> ProcessWords(IEnumerable<string> words);
    }
}