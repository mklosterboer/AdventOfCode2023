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
                var numberString = first.Digit.ToString() + last.Digit.ToString();

                var calibrationValue = int.Parse(numberString);

                PrintLine(value, first, last, calibrationValue);

                result += calibrationValue;
            }

            Console.WriteLine("");

            return result;
        }

        private (FoundNumber first, FoundNumber last) FindDigits(string value)
        {
            var result = Enumerable.Empty<int>();

            var numberMapping = new List<FoundNumber>();

            numberMapping.AddRange(FindIntegerMappings(value));
            numberMapping.AddRange(FindWordMappings(value));

            var orderedList = numberMapping.OrderBy(x => x.Index);

            return new(orderedList.First(), orderedList.Last());
        }

        private List<FoundNumber> FindIntegerMappings(string value)
        {
            var result = new List<FoundNumber>();

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

        private List<FoundNumber> FindWordMappings(string value)
        {
            var result = new List<FoundNumber>();

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

        private static List<FoundNumber> GetRegexMatches(string value, string word, int wordValue)
        {
            var result = new List<FoundNumber>();
            var regex = new Regex($@"(?'{word}'{word})");

            foreach (Match m in regex.Matches(value))
            {
                var group = m.Groups[word];
                if (group.Success)
                {
                    result.Add(new(wordValue, group.Index, group.Length));
                }
            }

            return result;
        }

        private static void PrintLine(string input, FoundNumber first, FoundNumber last, int value)
        {
            Console.WriteLine("");
            for (var i = 0; i < input.Length; i++)
            {
                if (first.Index == i)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    for (var j = 0; j < first.Length; j++)
                    {
                        Console.Write(input[i + j]);
                    }
                    i += first.Length - 1;
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }

                if (last.Index == i)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    for (var j = 0; j < last.Length; j++)
                    {
                        Console.Write(input[i + j]);
                    }
                    i += last.Length - 1;
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }
                Console.Write(input[i]);

                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Write($" - {value}");
        }

        [GeneratedRegex(@"^[^\d]*(?'first'\d{1})")]
        private static partial Regex FirstParser();

        [GeneratedRegex(@"(.*)(?'last'\d{1})(.*)$")]
        private static partial Regex LastParser();

        private record FoundNumber(int Digit, int Index, int Length);
    }
}
