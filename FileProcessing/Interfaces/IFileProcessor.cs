using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Interfaces
{
    /// <summary>
    /// Defines the contract for a file processor responsible for processing text files,
    /// extracting words, writing output files, and validating file names.
    /// </summary>
    internal interface IFileProcessor
    {
        /// <summary>
        /// Asynchronously processes a text file, calculating word frequencies.
        /// </summary>
        /// <param name="inputFileName">The path to the input text file.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
        /// The result is a <see cref="ConcurrentDictionary{TKey, TValue}"/> containing word frequencies.
        /// </returns>
        Task<ConcurrentDictionary<string, int>> ProcessFileAsync(string inputFileName);

        /// <summary>
        /// Writes word frequencies to an output file.
        /// </summary>
        /// <param name="outputFileName">The path to the output text file.</param>
        /// <param name="wordFrequencies">A <see cref="ConcurrentDictionary{TKey, TValue}"/> containing word frequencies.</param>
        void WriteOutputFile(string outputFileName, ConcurrentDictionary<string, int> wordFrequencies);

        /// <summary>
        /// Validates the input file name, throwing exceptions for invalid cases.
        /// </summary>
        /// <param name="inputFileName">The path to the input text file.</param>
        void ValidateInputFile(string inputFileName);

        /// <summary>
        /// Validates the output file name, directory existence, and write permissions,
        /// throwing exceptions for invalid cases.
        /// </summary>
        /// <param name="outputFileName">The path to the output text file.</param>
        void ValidateOutputFile(string outputFileName);

        /// <summary>
        /// Asynchronously extracts words from a text file using regular expressions.
        /// </summary>
        /// <param name="inputFileName">The path to the input text file.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
        /// The result is an array of strings representing the extracted words.
        /// </returns>
        Task<string[]> ExtractFileWordsAsync(string inputFileName);
    }

}
