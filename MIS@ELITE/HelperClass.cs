using System;

namespace MIS_ELITE
{
    internal class HelperClass
    {
        public string getFullMonthName(int Month, int Year)
        {
            DateTime date = new DateTime(Year, Month, 1);
            return string.Concat(date.ToString("MMMM").ToUpper(), " ", date.ToString("yyyy"));
        }
    }
}
