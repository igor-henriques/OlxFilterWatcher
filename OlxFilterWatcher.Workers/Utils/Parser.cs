namespace OlxFilterWatcher.Workers.Utils;

public static class Parser
{
    private static readonly Dictionary<string, int> monthKeyValues = new()
    {
        { "jan", 1 },
        { "fev", 2 },
        { "mar", 3 },
        { "abr", 4 },
        { "mai", 5 },
        { "jun", 6 },
        { "jul", 7 },
        { "ago", 8 },
        { "set", 9 },
        { "out", 10 },
        { "nov", 11 },
        { "dez", 12 },
    };

    public static int ToInt(this string text)
    {
        _ = int.TryParse(new string(text?.Where(c => char.IsDigit(c))?.ToArray()), out int result);

        return result;
    }

    public static decimal ToDecimal(this string text)
    {
        _ = decimal.TryParse(new string(text?.Where(c => char.IsDigit(c))?.ToArray()), out decimal result);

        return result;
    }

    public static string FilterCharactersOnly(this string text)
    {
        return new string(text.Where(c => char.IsLetter(c)).ToArray());
    }

    public static string FilterDigitsOnly(this string text)
    {
        return new string(text.Where(c => char.IsDigit(c)).ToArray());
    }

    public static DateTime ToDateTime(this string text)
    {
        var date = text switch
        {
            var dateText when dateText.Contains("Hoje") => DateTime.Today,
            var dateText when dateText.Contains("Ontem") => DateTime.Today.AddDays(-1),
            _ => GetDate(text)
        };

        var time = TimeOnly.Parse(text.Split(',')[1]);

        return new DateTime(date.Year, date.Month, date.Day, hour: time.Hour, minute: time.Minute, 0);

        DateTime GetDate(string dateTime)
        {
            var date = dateTime.Split(',')[0];

            var month = monthKeyValues.Where(x => date.FilterCharactersOnly().Contains(x.Key)).Select(x => x.Value).FirstOrDefault();

            var day = date.FilterDigitsOnly().ToInt();

            return new DateTime(year: DateTime.Now.Year, month: month, day: day);
        }
    }

    public static string ToJson<T>(this T type)
    {
        return JsonSerializer.Serialize(type);
    }

    public static OlxGeneralPost ToOlxPost(this string serializedText)
    {
        if (string.IsNullOrEmpty(serializedText))
            return null;

        return JsonSerializer.Deserialize<OlxGeneralPost>(serializedText);
    }

    public static bool IsNullOrEmpty(this string postUrl, out bool isNullOrEmpty)
    {
        isNullOrEmpty = string.IsNullOrEmpty(postUrl);

        return isNullOrEmpty;
    }
}
