using System;

namespace MIS_ELITE
{
    internal class ConvertNumberToWords
    {
        public ConvertNumberToWords()
        {
        }

        string[] ones = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
                             "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                             "Seventeen", "Eighteen", "Nineteen" };

        string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty",
                             "Seventy", "Eighty", "Ninety" };

        public string ConvertNumberToWord(double number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Negative " + ConvertNumberToWord(Math.Abs(number));

            long integerPart = (long)number;
            int decimalPart = (int)((number - integerPart) * 100); // Handling up to 2 decimal places

            string words = ConvertRecursively(integerPart);
            if (decimalPart > 0)
            {
                words += $" Point {ConvertDecimalToWords(decimalPart)}";
            }
            return words.Trim();
        }

        //public string ConvertRecursively(long number)
        //{
        //    if (number < 20)
        //        return ones[number];

        //    if (number < 100)
        //        return tens[number / 10] + (number % 10 != 0 ? " " + ones[number % 10] : "");

        //    if (number < 1000)
        //        return ones[number / 100] + " Hundred" + (number % 100 != 0 ? " " + ConvertRecursively(number % 100) : "");

        //    if (number < 1_000_000)
        //        return ConvertRecursively(number / 1000) + " Thousand" + (number % 1000 != 0 ? ", " + ConvertRecursively(number % 1000) : "");

        //    if (number < 1_000_000_000)
        //        return ConvertRecursively(number / 1_000_000) + " Million" + (number % 1_000_000 != 0 ? ", " + ConvertRecursively(number % 1_000_000) : "");

        //    return ConvertRecursively(number / 1_000_000_000) + " Billion" + (number % 1_000_000_000 != 0 ? ", " + ConvertRecursively(number % 1_000_000_000) : "");
        //}

        public string ConvertRecursively(long number)
        {
            if (number < 20)
                return ones[number];

            if (number < 100)
                return tens[number / 10] + (number % 10 != 0 ? " " + ones[number % 10] : "");

            if (number < 1000)
                return ones[number / 100] + " Hundred" + (number % 100 != 0 ? " " + ConvertRecursively(number % 100) : "");

            if (number < 1_00_000)
                return ConvertRecursively(number / 1000) + " Thousand" + (number % 1000 != 0 ? " " + ConvertRecursively(number % 1000) : "");

            if (number < 1_00_00_000)
                return ConvertRecursively(number / 1_00_000) + " Lac" + (number % 1_00_000 != 0 ? " " + ConvertRecursively(number % 1_00_000) : "");

            return ConvertRecursively(number / 1_00_00_000) + " Crore" + (number % 1_00_00_000 != 0 ? " " + ConvertRecursively(number % 1_00_00_000) : "");
        }


        public string ConvertDecimalToWords(int number)
        {
            string result = "";
            foreach (char digit in number.ToString())
            {
                result += ones[int.Parse(digit.ToString())] + " ";
            }
            return result.Trim();
        }
    }
}
