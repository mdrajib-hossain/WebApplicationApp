using System.Text;

namespace WebApplicationApp.Repositories.SelesManService
{


    public static class NumberToWordsConverter
    {
        private static readonly string[] Ones =
        {
            "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
            "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
        };

        private static readonly string[] Tens =
        {
            "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
        };

        private static readonly string[] BanglaNumbers =
        {
            "", "এক", "দুই", "তিন", "চার", "পাঁচ", "ছয়", "সাত", "আট", "নয়", "দশ",
            "এগারো", "বারো", "তেরো", "চৌদ্দ", "পনেরো", "ষোলো", "সতেরো", "আঠারো", "উনিশ", "বিশ",
            "একুশ", "বাইশ", "তেইশ", "চব্বিশ", "পঁচিশ", "ছাব্বিশ", "সাতাশ", "আটাশ", "উনত্রিশ", "ত্রিশ",
            "একত্রিশ", "বত্রিশ", "তেত্রিশ", "চৌত্রিশ", "পঁয়ত্রিশ", "ছত্রিশ", "সাঁইত্রিশ", "আটত্রিশ", "উনচল্লিশ", "চল্লিশ",
            "একচল্লিশ", "বিয়াল্লিশ", "তেতাল্লিশ", "চৌয়াল্লিশ", "পঁয়তাল্লিশ", "ছেচল্লিশ", "সাতচল্লিশ", "আটচল্লিশ", "উনপঞ্চাশ", "পঞ্চাশ",
            "একান্ন", "বায়ান্ন", "তিপ্পান্ন", "চৌয়ান্ন", "পঞ্চান্ন", "ছাপ্পান্ন", "সাতান্ন", "আটান্ন", "উনষাট", "ষাট",
            "একষট্টি", "বাষট্টি", "তেষট্টি", "চৌষট্টি", "পঁয়ষট্টি", "ছেছট্টি", "সাতষট্টি", "আটষট্টি", "উনসত্তর", "সত্তর",
            "একাত্তর", "বাহাত্তর", "তেহাত্তর", "চৌহাত্তর", "পঁচাত্তর", "ছিয়াত্তর", "সাতাত্তর", "আটাত্তর", "উনাশি", "আশি",
            "একাশি", "বিরাশি", "তিরাশি", "চৌরাশি", "পঁচাশি", "ছিয়াশি", "সাতাশি", "আটাশি", "উননব্বই", "নব্বই",
            "একানব্বই", "বিয়ানব্বই", "তিরানব্বই", "চৌরানব্বই", "পঁচানব্বই", "ছিয়ানব্বই", "সাতানব্বই", "আটানব্বই", "নিরানব্বই"
        };

        public static string ConvertToWords(decimal amount)
        {
            if (amount == 0) return "Zero Taka Only";
            if (amount < 0) return "Minus " + ConvertToWords(Math.Abs(amount));

            long integerPart = (long)Math.Floor(amount);
            int decimalPart = (int)Math.Round((amount - integerPart) * 100);

            string words = "";

            if (integerPart > 0)
            {
                words += ConvertIntegerToWords(integerPart) + " Taka";
            }

            if (decimalPart > 0)
            {
                if (!string.IsNullOrEmpty(words))
                    words += " and ";

                words += ConvertIntegerToWords(decimalPart) + " Paisa";
            }

            return words.Trim() + " Only";
        }

        private static string ConvertIntegerToWords(long number)
        {
            if (number == 0) return "";
            if (number < 20) return Ones[number];
            if (number < 100) return Tens[number / 10] + (number % 10 > 0 ? " " + Ones[number % 10] : "");
            if (number < 1000) return Ones[number / 100] + " Hundred" + (number % 100 > 0 ? " " + ConvertIntegerToWords(number % 100) : "");
            if (number < 100000) return ConvertIntegerToWords(number / 1000) + " Thousand" + (number % 1000 > 0 ? " " + ConvertIntegerToWords(number % 1000) : "");
            if (number < 10000000) return ConvertIntegerToWords(number / 100000) + " Lakh" + (number % 100000 > 0 ? " " + ConvertIntegerToWords(number % 100000) : "");

            return ConvertIntegerToWords(number / 10000000) + " Crore" + (number % 10000000 > 0 ? " " + ConvertIntegerToWords(number % 10000000) : "");
        }

        public static string ConvertToBanglaWords(decimal amount)
        {
            if (amount == 0) return "শূন্য টাকা মাত্র";
            if (amount < 0) return "মাইনাস " + ConvertToBanglaWords(Math.Abs(amount));

            long integerPart = (long)Math.Floor(amount);
            int decimalPart = (int)Math.Round((amount - integerPart) * 100);

            string words = "";

            if (integerPart > 0)
            {
                words += ConvertIntegerToBangla(integerPart) + " টাকা";
            }

            if (decimalPart > 0)
            {
                if (!string.IsNullOrEmpty(words))
                    words += " এবং ";

                words += ConvertIntegerToBangla(decimalPart) + " পয়সা";
            }

            return words.Trim() + " মাত্র";
        }

        private static string ConvertIntegerToBangla(long number)
        {
            if (number == 0) return "";
            if (number < 100) return BanglaNumbers[number];
            if (number < 1000) return BanglaNumbers[number / 100] + " শত" + (number % 100 > 0 ? " " + ConvertIntegerToBangla(number % 100) : "");
            if (number < 100000) return ConvertIntegerToBangla(number / 1000) + " হাজার" + (number % 1000 > 0 ? " " + ConvertIntegerToBangla(number % 1000) : "");
            if (number < 10000000) return ConvertIntegerToBangla(number / 100000) + " লক্ষ" + (number % 100000 > 0 ? " " + ConvertIntegerToBangla(number % 100000) : "");

            return ConvertIntegerToBangla(number / 10000000) + " কোটি" + (number % 10000000 > 0 ? " " + ConvertIntegerToBangla(number % 10000000) : "");
        }
    









    public static string FixForPdf(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Split compound vowels into visual components
            text = text.Replace("ো", "\u09C7\u09BE"); // o-kar -> e-kar + a-kar
            text = text.Replace("ৌ", "\u09C7\u09CC"); // ou-kar -> e-kar + ou-mark

            StringBuilder sb = new StringBuilder();
            char[] chars = text.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];

                // Reorder left-vowel marks: e-kar (ে), oi-kar (ৈ), i-kar (ি)
                if (c == '\u09C7' || c == '\u09C8' || c == '\u09BF')
                {
                    int pos = sb.Length - 1;
                    while (pos >= 0 && (IsBanglaConsonant(sb[pos]) || sb[pos] == '\u09CD'))
                    {
                        pos--;
                    }
                    pos++;

                    if (pos < sb.Length)
                    {
                        sb.Insert(pos, c);
                        continue;
                    }
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static bool IsBanglaConsonant(char c)
        {
            return (c >= '\u0985' && c <= '\u09B9');
        }





    } 



    }
