using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Text;    

namespace _2whealers.Models
{
    public static class NumberToWordsConverter
    {
        private static readonly string[] UnitsMap = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
            "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

        private static readonly string[] TensMap = { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public static string ConvertAmountToWords(decimal amount)
        {
            // Round to 2 decimals always
            amount = Math.Round(amount, 2);

            long intPortion = (long)amount;
            int fraction = (int)((amount - intPortion) * 100);

            var words = new StringBuilder();

            if (intPortion > 0)
            {
                words.Append(NumberToWords(intPortion) + " Rupees");
            }

            if (fraction > 0)
            {
                if (words.Length > 0)
                    words.Append(" and ");

                words.Append(NumberToWords(fraction) + " Paise");
            }

            if (words.Length == 0)
                words.Append("Zero Rupees");

            words.Append(" Only");

            return words.ToString();
        }

        private static string NumberToWords(long number)
        {
            if (number == 0)
                return UnitsMap[0];

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if (number / 10000000 > 0)
            {
                words += NumberToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if (number / 100000 > 0)
            {
                words += NumberToWords(number / 100000) + " Lakh ";
                number %= 100000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                if (number < 20)
                    words += UnitsMap[number];
                else
                {
                    words += TensMap[number / 10];
                    if (number % 10 > 0)
                        words += "-" + UnitsMap[number % 10];
                }
            }

            return words.Trim();
        }
    }
}
