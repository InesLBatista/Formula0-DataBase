using System;
using System.Text.RegularExpressions;

namespace ProjetoFBD
{
    /// <summary>
    /// Provides validation utilities for form inputs
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates if a string is not null or empty
        /// </summary>
        public static bool IsNotEmpty(string value, string fieldName, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                errorMessage = $"{fieldName} cannot be empty.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if a position number is valid (positive integer)
        /// </summary>
        public static bool IsValidPosition(int position, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (position < 1 || position > 100)
            {
                errorMessage = "Position must be between 1 and 100.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if points value is non-negative
        /// </summary>
        public static bool IsValidPoints(decimal points, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (points < 0)
            {
                errorMessage = "Points cannot be negative.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if a year is within reasonable range
        /// </summary>
        public static bool IsValidYear(int year, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (year < 1950 || year > DateTime.Now.Year + 1)
            {
                errorMessage = $"Year must be between 1950 and {DateTime.Now.Year + 1}.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates time string format (HH:MM:SS.mmm)
        /// </summary>
        public static bool IsValidTimeFormat(string time, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(time))
            {
                return true; // Allow empty times
            }

            // Regex for time format: HH:MM:SS or HH:MM:SS.mmm
            var regex = new Regex(@"^([0-9]{1,2}):([0-5][0-9]):([0-5][0-9])(\.[0-9]{1,3})?$");
            if (!regex.IsMatch(time))
            {
                errorMessage = "Time must be in format HH:MM:SS or HH:MM:SS.mmm";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if a value can be parsed as integer
        /// </summary>
        public static bool TryParseInt(string value, string fieldName, out int result, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!int.TryParse(value, out result))
            {
                errorMessage = $"{fieldName} must be a valid number.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates if a value can be parsed as decimal
        /// </summary>
        public static bool TryParseDecimal(string value, string fieldName, out decimal result, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!decimal.TryParse(value, out result))
            {
                errorMessage = $"{fieldName} must be a valid decimal number.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates nationality format (2-3 letter country code or full name)
        /// </summary>
        public static bool IsValidNationality(string nationality, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(nationality))
            {
                errorMessage = "Nationality cannot be empty.";
                return false;
            }
            if (nationality.Length < 2 || nationality.Length > 50)
            {
                errorMessage = "Nationality must be between 2 and 50 characters.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates driver abbreviation (3 letters)
        /// </summary>
        public static bool IsValidDriverCode(string code, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(code))
            {
                errorMessage = "Driver code cannot be empty.";
                return false;
            }
            if (code.Length != 3)
            {
                errorMessage = "Driver code must be exactly 3 characters (e.g., HAM, VER, LEC).";
                return false;
            }
            if (!Regex.IsMatch(code, "^[A-Z]{3}$"))
            {
                errorMessage = "Driver code must contain only uppercase letters.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates gender (M/F)
        /// </summary>
        public static bool IsValidGender(string gender, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(gender))
            {
                errorMessage = "Gender cannot be empty.";
                return false;
            }
            gender = gender.ToUpper();
            if (gender != "M" && gender != "F")
            {
                errorMessage = "Gender must be 'M' or 'F'.";
                return false;
            }
            return true;
        }
    }
}
