using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    /// <summary>
    /// Reads student records from a comma-separated text file and writes a
    /// formatted grade report to an output file.
    /// </summary>
    public class StudentResultProcessor
    {
        /// <summary>
        /// Reads and validates student records from a text file.
        /// Each line must contain exactly 3 comma-separated fields:
        /// ID, FullName, Score.
        /// </summary>
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    // Skip blank lines rather than treating them as bad records.
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    string[] fields = line.Split(',');

                    if (fields.Length != 3)
                    {
                        throw new MissingFieldException(
                            $"Line {lineNumber}: expected 3 fields (ID, Name, Score) but found {fields.Length}. " +
                            $"Line content: \"{line}\"");
                    }

                    string idText = fields[0].Trim();
                    string fullName = fields[1].Trim();
                    string scoreText = fields[2].Trim();

                    if (idText.Length == 0 || fullName.Length == 0 || scoreText.Length == 0)
                    {
                        throw new MissingFieldException(
                            $"Line {lineNumber}: one or more required fields is empty. Line content: \"{line}\"");
                    }

                    if (!int.TryParse(idText, out int id))
                    {
                        throw new MissingFieldException(
                            $"Line {lineNumber}: student ID \"{idText}\" is not a valid whole number.");
                    }

                    if (!int.TryParse(scoreText, out int score))
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: score \"{scoreText}\" is not a valid integer.");
                    }

                    students.Add(new Student(id, fullName, score));
                }
            }

            return students;
        }

        /// <summary>
        /// Writes a formatted grade summary for each student to the given output file.
        /// </summary>
        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine("STUDENT GRADE REPORT");
                writer.WriteLine("=====================");
                writer.WriteLine();

                foreach (Student student in students)
                {
                    writer.WriteLine(
                        $"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }

                writer.WriteLine();
                writer.WriteLine($"Total students processed: {students.Count}");
            }
        }
    }
}
