using System.Text.RegularExpressions;

namespace Journal_Service;

public static class UtilityExt
{
    public static string NormalizeTitle(this string input)
    {
        //string noWhitespace = Regex.Replace(input, "[^a-zA-Z]", "");
        string noWhitespace = Regex.Replace(input, "[^a-zA-Z0-9\u0600-\u06FF\u0750-\u077F]", "");

        return noWhitespace.Trim().ToLower();
    }

    public static string CleanIssn(this string issn)
    {
        return string.IsNullOrWhiteSpace(issn) == false
            ? issn.Trim().Replace("-", string.Empty).Replace(" ",string.Empty).ToUpper()
            : string.Empty;
    }

    public static string ToTitleCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    private static readonly Dictionary<char, char> arabicToPersianMap = new Dictionary<char, char>
    {
        { 'ي', 'ی' },
        { 'ك', 'ک' },
        { 'ه', 'ه' },
    };

    public static string ConvertArabicToPersian(this string input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        char[] result = input.ToCharArray();
        for (int i = 0; i < result.Length; i++)
        {
            if (arabicToPersianMap.TryGetValue(result[i], out char persianChar))
            {
                result[i] = persianChar;
            }
        }

        return new string(result);
    }
}