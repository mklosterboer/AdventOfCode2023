using AdventOfCode2023.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Problems
{
    internal partial class Day01Problem : Problem
    {
        protected override string InputName => "Actual";

        private IEnumerable<string> CalibrationValues { get; set; }

        public Day01Problem()
        {
            CalibrationValues = GetInputStringList();
        }

        public override object PartOne()
        {
            var result = 0;

            foreach (var value in CalibrationValues)
            {
                var firstNumberString = FirstParser().Match(value).Groups["first"].Value;
                var lastNumberString = LastParser().Match(value).Groups["last"].Value;

                var calibrationValue = int.Parse(firstNumberString + lastNumberString);

                result += calibrationValue;
            }

            return result;
        }

        public override object PartTwo()
        {
            var result = 0;

            foreach (var value in CalibrationValues)
            {
                var (first, last) = FindDigits(value);
                var numberString = first.digit.ToString() + last.digit.ToString();

                var calibrationValue = int.Parse(numberString);

                Console.WriteLine("");
                for (var i = 0; i < value.Length; i++)
                {
                    if (first.index == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        for (var j = 0; j < first.length; j++)
                        {
                            Console.Write(value[i + j]);
                        }
                        i += first.length - 1;
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }

                    if (last.index == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        for (var j = 0; j < last.length; j++)
                        {
                            Console.Write(value[i + j]);
                        }
                        i += last.length - 1;
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }
                    Console.Write(value[i]);

                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.Write($" - {calibrationValue}");

                result += calibrationValue;
            }

            Console.WriteLine("");

            return result;
        }

        private ((int digit, int index, int length) first, (int digit, int index, int length) last) FindDigits(string value)
        {
            var result = Enumerable.Empty<int>();

            var numberMapping = new List<(int digit, int index, int length)>();

            numberMapping.AddRange(FindIntegerMappings(value));
            numberMapping.AddRange(FindWordMappings(value));

            var orderedList = numberMapping.OrderBy(x => x.index);

            return new(orderedList.First(), orderedList.Last());
        }

        private List<(int digit, int index, int length)> FindIntegerMappings(string value)
        {
            var result = new List<(int digit, int index, int length)>();

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i].ToString();

                if (int.TryParse(c, out var digit))
                {
                    result.Add(new(digit, i, 1));
                }
            }

            return result;
        }

        private List<(int digit, int index, int length)> FindWordMappings(string value)
        {
            var result = new List<(int digit, int index, int length)>();

            result.AddRange(GetRegexMatches(value, "one", 1));
            result.AddRange(GetRegexMatches(value, "two", 2));
            result.AddRange(GetRegexMatches(value, "three", 3));
            result.AddRange(GetRegexMatches(value, "four", 4));
            result.AddRange(GetRegexMatches(value, "five", 5));
            result.AddRange(GetRegexMatches(value, "six", 6));
            result.AddRange(GetRegexMatches(value, "seven", 7));
            result.AddRange(GetRegexMatches(value, "eight", 8));
            result.AddRange(GetRegexMatches(value, "nine", 9));

            return result;
        }

        private static List<(int digit, int index, int length)> GetRegexMatches(
            string value, string word, int wordValue)
        {
            var result = new List<(int digit, int index, int length)> ();
            var regex = new Regex($@"(?'{word}'{word})");

            foreach(Match m in regex.Matches(value))
            {
                var group = m.Groups[word];
                if (group.Success)
                {
                    result.Add(new (wordValue, group.Index, group.Length));
                }
            }

            return result;
        }

        [GeneratedRegex(@"^[^\d]*(?'first'\d{1})")]
        private static partial Regex FirstParser();

        [GeneratedRegex(@"(.*)(?'last'\d{1})(.*)$")]
        private static partial Regex LastParser();
    }
}
