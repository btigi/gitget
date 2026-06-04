namespace GitGet.Utilities;

public static class WildcardMatcher
{
    public static bool IsMatch(string input, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return false;
        }

        if (!pattern.Contains('*'))
        {
            return string.Equals(input, pattern, StringComparison.OrdinalIgnoreCase);
        }

        var parts = pattern.Split('*', StringSplitOptions.None);
        if (parts.Length == 0)
        {
            return true;
        }

        var index = 0;
        var first = true;

        foreach (var part in parts)
        {
            if (part.Length == 0)
            {
                first = false;
                continue;
            }

            var found = input.IndexOf(part, index, StringComparison.OrdinalIgnoreCase);
            if (found < 0)
            {
                return false;
            }

            if (first && found != 0)
            {
                return false;
            }

            index = found + part.Length;
            first = false;
        }

        var endsWithWildcard = pattern.EndsWith('*');
        return endsWithWildcard || index == input.Length;
    }
}
