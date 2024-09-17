using System.Globalization;
using System.Text.RegularExpressions;
using GajinoAgencies.Settings;

namespace GajinoAgencies.Utilities;

public static class DateManagerExtension
{

    public static string ToPersianDate(this string gregorianDate)
    {
        if (!DateTime.TryParse(gregorianDate, out var dateTime))
        {
            throw new ArgumentException("The input date is not in a valid format.");
        }

        var persianCalendar = new PersianCalendar();

        var year = persianCalendar.GetYear(dateTime);
        var month = persianCalendar.GetMonth(dateTime);
        var day = persianCalendar.GetDayOfMonth(dateTime);
        var hour = dateTime.Hour;
        var minute = dateTime.Minute;
        var second = dateTime.Second;

        return $"{year}/{month:D2}/{day:D2} {hour:D2}:{minute:D2}:{second:D2}";
    }

    public static DateTime ToGregorianDate(this string persianDate, string delimiter = "-")
    {
        //var regex = new Regex(AppConstants.PersianDateRegex);
        var regex = CreatePersianDateRegex(delimiter);

        var match = regex.Match(persianDate);
        if (!match.Success)
        {
            throw new ArgumentException("The input date is not in a valid Persian date format (yyyy/MM/dd hh:mm:ss).");
        }

        var year = int.Parse(match.Groups["year"].Value);
        var month = int.Parse(match.Groups["month"].Value);
        var day = int.Parse(match.Groups["day"].Value);

        var hour = match.Groups["hour"].Success ? int.Parse(match.Groups["hour"].Value) : 0;
        var minute = match.Groups["minute"].Success ? int.Parse(match.Groups["minute"].Value) : 0;
        var second = match.Groups["second"].Success ? int.Parse(match.Groups["second"].Value) : 0;

        var persianCalendar = new PersianCalendar();
        
        if ((year is < 1 or > 9999) || (month is < 1 or > 12) || (day is < 1 or > 31))
        {
            throw new ArgumentException("The provided Persian date components are out of range.");
        }

        switch (month)
        {
            case 12 when day > 29 && !persianCalendar.IsLeapYear(year):
                throw new ArgumentException("Invalid day for the given month in the Persian calendar.");
            case > 6 when day > 30:
                throw new ArgumentException("Invalid day for the given month in the Persian calendar.");
            default:
            {
                var gregorianDate = persianCalendar.ToDateTime(year, month, day, hour, minute, second, 0);

                return gregorianDate;
            }
        }
    }

    public static Regex CreatePersianDateRegex(string delimiter)
    {
        // Escape the delimiter for regex
        var escapedDelimiter = Regex.Escape(delimiter);

        // Replace the placeholder with the escaped delimiter
        var pattern = AppConstants.PersianDateRegexTemplate.Replace("DELIMITER", escapedDelimiter);

        return new Regex(pattern);
    }
}
