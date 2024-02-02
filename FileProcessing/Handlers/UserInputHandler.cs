using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.Handlers
{
    /// <summary>
    /// Handles user input related to file names for input and output.
    /// </summary>
    public class UserInputHandler
    {
        /// <summary>
        /// Retrieves the input file name either from user input or a predefined default value.
        /// </summary>
        /// <returns>
        /// A string representing the input file name.
        /// If user input is requested, it is obtained from the console using <see cref="Console.ReadLine"/>.
        /// Otherwise, a predefined default value is returned.
        /// </returns>
        public string GetInputFileName()
        {
            // Uncomment the following lines for user input
            // Console.Write("Enter the input file name: ");
            // return Console.ReadLine();

            // Return a predefined default value from project file
            return @"..\..\..\Assets\Input.txt";
        }

        /// <summary>
        /// Retrieves the output file name either from user input or a predefined default value.
        /// </summary>
        /// <returns>
        /// A string representing the output file name.
        /// If user input is requested, it is obtained from the console using <see cref="Console.ReadLine"/>.
        /// Otherwise, a predefined default value is returned.
        /// </returns>
        public string GetOutputFileName()
        {
            // Uncomment the following lines for user input
            // Console.Write("Enter the output file name: ");
            // return Console.ReadLine();

            // Return a predefined default value from project file
            return @"..\..\..\Assets\Output.txt";
        }
    }


}
