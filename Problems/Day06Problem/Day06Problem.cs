using AdventOfCode2023.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Problems
{
    internal partial class Day06Problem : Problem
    {
        protected override string InputName => "Test";

        private string[] Input;

        public Day06Problem()
        {
            Input = GetInputStringList().ToArray();
        }

        public override object PartOne()
        {
            var times = GetDigits()
                .Matches(Input[0].Split(":")[1])
                .Select(x => int.Parse(x.Value)).ToArray();
            var distances = GetDigits()
                .Matches(Input[1] .Split(":")[1])
                .Select(x => int.Parse(x.Value)).ToArray();

            return Enumerable.Range(0, times.Length)
                .Select(x => GetWinSum(times[x], distances[x]))
                .Aggregate((long)1, (acc, val) => acc * val);
        }

        public override object PartTwo()
        {
            var raceTime = long.Parse(Input[0].Split(":")[1].Replace(" ", ""));
            var recordDistance = long.Parse(Input[1].Split(":")[1].Replace(" ", ""));

            return GetWinSum(raceTime, recordDistance);
        }

        private static long GetWinSum(long raceTime, long recordDistance)
        {
            var sqrt = Math.Sqrt(raceTime * raceTime - 4 * recordDistance);
            var pos = (sqrt - raceTime) / -2;
            var neg = (-sqrt - raceTime) / -2;

            if (neg < pos)
            {
                return GetRange(neg, pos);
            }
            else
            {
                return GetRange(pos, neg);
            }
        }

        private static long GetRange(double min, double max)
        {
            var roundedMin = (int)Math.Ceiling(min);
            if (roundedMin == min)
            {
                roundedMin++;
            }

            var roundedMax = (int)Math.Floor(max);
            if (roundedMax == max)
            {
                roundedMax--;
            }

            return roundedMax - roundedMin + 1;
        }

        [GeneratedRegex(@"\d+")]
        private static partial Regex GetDigits();
    }
}
