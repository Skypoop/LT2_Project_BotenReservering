using System.Text.RegularExpressions;

namespace ProjectBotenReservering.Core.Helpers;

public static class ValidationHelper
{
    public static bool IsValidName(string name)
    {
        string trimmedName = name?.Trim() ?? string.Empty;
        string[] parts = trimmedName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Regex nameRegex = new Regex(@"^[\p{L}\s\-\']+$");

        if (string.IsNullOrWhiteSpace(trimmedName) || parts.Length < 2 || !nameRegex.IsMatch(trimmedName))
        {
            return false;
        }
        return true;
    }

    public static bool IsValidEmail(string email)
    {
        string trimmedEmail = email?.Trim() ?? string.Empty;
        Regex regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        
        if (string.IsNullOrWhiteSpace(trimmedEmail) || !regex.IsMatch(trimmedEmail))
        {
            return false;
        }
        return true;
    }

    public static bool IsValidLevel(string level)
    {
        if (string.IsNullOrWhiteSpace(level))
        {
            return true;
        }
        
        bool isInt = int.TryParse(level, out int val);
        if (!isInt || val < 0 || val > 3)
        {
            return false;
        }
        return true;
    }
}