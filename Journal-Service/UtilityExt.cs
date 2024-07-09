﻿using System.Text.RegularExpressions;

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
        return issn.Trim().Replace("-", String.Empty).ToUpper();
    }
    
    private static readonly Dictionary<char, char> arabicToPersianMap = new Dictionary<char, char>
    {
        {'ي', 'ی'},
        {'ك', 'ک'},
        {'ه', 'ه'},
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