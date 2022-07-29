namespace RedCounterSoftware.Common.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static IEnumerable<DateTimeOffset> GetMonthsByRange(this DateTimeOffset from, DateTimeOffset to)
        {
            if (to < from)
            {
                throw new ArgumentException("from must represent a date older than to.");
            }

            var monthsDifference = ((to.Year - from.Year) * 12) + to.Month - from.Month;
            var year = from.Year;
            var month = from.Month;
            for (var i = 0; i <= monthsDifference; i++)
            {
                var output = new DateTimeOffset(year, month, 1, 0, 0, 0, new TimeSpan(0, 0, 0));
                if (month == 12)
                {
                    year++;
                    month = 1;
                }
                else
                {
                    month++;
                }

                yield return output;
            }
        }
    }
}
