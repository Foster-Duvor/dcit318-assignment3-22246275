using System;

namespace SchoolGradingSystem
{
    /// <summary>
    /// Thrown when a student's score cannot be converted to an integer.
    /// </summary>
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message)
        {
        }
    }
}