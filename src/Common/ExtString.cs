namespace HXE.Common;

public static class ExtString
{
    public static bool ContainsAny(this string haystack, bool caseInsensitive, params string[] needles)
    {
        foreach (string needle in needles)
        {
            if (caseInsensitive)
            {
                if (haystack.Contains(needle.ToLower()))
                    return true;
            }
            else
            {
                if (haystack.Contains(needle))
                    return true;
            }
        }

        return false;
    }
}
