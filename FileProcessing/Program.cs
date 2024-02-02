// See https://aka.ms/new-console-template for more information
using FileProcessing.Handlers;
using FileProcessing.Services;
using Serilog;

Console.WriteLine("Hello, World!");

try
{
    // Initialize Serilog logger
    var logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

    var userInputHandler = new UserInputHandler();
    var wordProcessor = new WordProcessor(logger);
    var fileProcessor = new FileProcessor(logger, wordProcessor);

    string inputFileName = userInputHandler.GetInputFileName();
    string outputFileName = userInputHandler.GetOutputFileName();

    var wordFrequencies = await fileProcessor.ProcessFileAsync(inputFileName);

    fileProcessor.WriteOutputFile(outputFileName, wordFrequencies);

    Console.WriteLine("File process completed successfully. Please check the output at \\Assets\\Output.txt in project folder.");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}