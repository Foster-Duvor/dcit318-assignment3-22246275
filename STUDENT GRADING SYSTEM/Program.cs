using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("       SCHOOL GRADING SYSTEM");
            Console.WriteLine("=========================================\n");

            bool keepRunning = true;

            while (keepRunning)
            {
                ProcessOneFile();
                Console.WriteLine();
                keepRunning = AskContinueOrQuit();
                Console.WriteLine();
            }

            Console.WriteLine("Goodbye!");
        }

        /// <summary>
        /// Runs one full read -> process -> write cycle, wrapped in the
        /// exception handling required by the assignment.
        /// </summary>
        private static void ProcessOneFile()
        {
            var processor = new StudentResultProcessor();

            Console.Write("Enter path to input file (e.g. students.txt): ");
            string? inputPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(inputPath))
            {
                Console.WriteLine("No file path entered. Skipping this run.");
                return;
            }

            Console.Write("Enter path for the output report (e.g. report.txt): ");
            string? outputPath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                outputPath = "report.txt";
                Console.WriteLine($"No output path entered. Defaulting to \"{outputPath}\".");
            }

            try
            {
                List<Student> students = processor.ReadStudentsFromFile(inputPath);
                processor.WriteReportToFile(students, outputPath);

                Console.WriteLine($"\nSuccess! Processed {students.Count} student record(s).");
                Console.WriteLine($"Report written to: {Path.GetFullPath(outputPath)}\n");

                Console.WriteLine("Preview:");
                Console.WriteLine("--------");
                foreach (Student student in students)
                {
                    Console.WriteLine(
                        $"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"\nError: the file \"{inputPath}\" could not be found. Check the path and try again.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"\nError: invalid score format.\n{ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"\nError: missing field in student record.\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Asks the user whether to process another file or quit the program.
        /// </summary>
        private static bool AskContinueOrQuit()
        {
            while (true)
            {
                Console.Write("Type 'C' to process another file, or 'Q' to quit: ");
                string? input = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (input == "c" || input == "continue")
                {
                    return true;
                }

                if (input == "q" || input == "quit")
                {
                    return false;
                }

                Console.WriteLine("Please enter 'C' or 'Q'.");
            }
        }
    }
}
