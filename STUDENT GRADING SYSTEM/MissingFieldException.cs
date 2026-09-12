using System;

namespace SchoolGradingSystem
{
    /// <summary>
    /// Thrown when a line in the input file does not contain all required fields
    /// (student ID, full name, and score).
    /// </summary>
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message)
        {
        }
    }
}