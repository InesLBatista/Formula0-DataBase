using System;
using System.Text.RegularExpressions;

namespace ProjetoFBD
{
    // Funções de validação usadas nos formulários
    public static class ValidationHelper
    {
        // Campo obrigatório
        public static bool IsNotEmpty(string value, string fieldName, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                errorMessage = $"{fieldName} não pode estar vazio.";
                return false;
            }
            return true;
        }

        // Posição válida (1-100)
        public static bool IsValidPosition(int position, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (position < 1 || position > 100)
            {
                errorMessage = "A posição deve estar entre 1 e 100.";
                return false;
            }
            return true;
        }

        // Pontos >= 0
        public static bool IsValidPoints(decimal points, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (points < 0)
            {
                errorMessage = "Os pontos não podem ser negativos.";
                return false;
            }
            return true;
        }

        // Ano razoável
        public static bool IsValidYear(int year, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (year < 1950 || year > DateTime.Now.Year + 1)
            {
                errorMessage = $"O ano deve estar entre 1950 e {DateTime.Now.Year + 1}.";
                return false;
            }
            return true;
        }

        // Tempo no formato HH:MM:SS(.mmm)
        public static bool IsValidTimeFormat(string time, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(time))
            {
                return true; // Allow empty times
            }

            var regex = new Regex(@"^([0-9]{1,2}):([0-5][0-9]):([0-5][0-9])(\.[0-9]{1,3})?$");
            if (!regex.IsMatch(time))
            {
                errorMessage = "O tempo deve estar no formato HH:MM:SS ou HH:MM:SS.mmm";
                return false;
            }
            return true;
        }

        // Converter para inteiro
        public static bool TryParseInt(string value, string fieldName, out int result, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!int.TryParse(value, out result))
            {
                errorMessage = $"{fieldName} deve ser um número válido.";
                return false;
            }
            return true;
        }

        // Converter para decimal
        public static bool TryParseDecimal(string value, string fieldName, out decimal result, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!decimal.TryParse(value, out result))
            {
                errorMessage = $"{fieldName} deve ser um número decimal válido.";
                return false;
            }
            return true;
        }

        // Nacionalidade preenchida (2-50 chars)
        public static bool IsValidNationality(string nationality, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(nationality))
            {
                errorMessage = "A nacionalidade não pode estar vazia.";
                return false;
            }
            if (nationality.Length < 2 || nationality.Length > 50)
            {
                errorMessage = "A nacionalidade deve ter entre 2 e 50 caracteres.";
                return false;
            }
            return true;
        }

        // Código do piloto (3 letras maiúsculas)
        public static bool IsValidDriverCode(string code, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(code))
            {
                errorMessage = "O código do piloto não pode estar vazio.";
                return false;
            }
            if (code.Length != 3)
            {
                errorMessage = "O código do piloto deve ter exatamente 3 caracteres (ex: HAM, VER, LEC).";
                return false;
            }
            if (!Regex.IsMatch(code, "^[A-Z]{3}$"))
            {
                errorMessage = "O código do piloto deve conter apenas letras maiúsculas.";
                return false;
            }
            return true;
        }

        // Gênero: M ou F
        public static bool IsValidGender(string gender, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(gender))
            {
                errorMessage = "O gênero não pode estar vazio.";
                return false;
            }
            gender = gender.ToUpper();
            if (gender != "M" && gender != "F")
            {
                errorMessage = "O gênero deve ser 'M' ou 'F'.";
                return false;
            }
            return true;
        }
    }
}
