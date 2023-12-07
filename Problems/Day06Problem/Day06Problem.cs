using AdventOfCode2023.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Problems
{
    internal partial class Day06Problem : Problem
    {
        protected override string InputName => "Actual";

        public override object PartOne()
        {
            var input = GetInputStringList().ToArray();

            var times = GetDigits().Matches(input[0].Split(":")[1]).Select(x => int.Parse(x.Value)).ToArray();
            var distances = GetDigits().Matches(input[1].Split(":")[1]).Select(x => int.Parse(x.Value)).ToArray();

            var races = Enumerable.Range(0, times.Length).Select(x => new { time = times[x], recordDistance = distances[x] }).ToList();

            return races
                .Select(race => GetWinSum(race.time, race.recordDistance))
                .Aggregate((long)1, (acc, val) => acc * val);
        }

        public override object PartTwo()
        {
            var input = GetInputStringList().ToArray();

            var raceTime = GetDigits().Matches(input[0].Split(":")[1].Replace(" ", "")).Select(x => long.Parse(x.Value)).First();
            var recordDistance = GetDigits().Matches(input[1].Split(":")[1].Replace(" ", "")).Select(x => long.Parse(x.Value)).First();

            return GetWinSum(raceTime, recordDistance);
        }

        private static long GetWinSum(long raceTime, long recordDistance)
        {
            var winSum = 0;

            for (var i = 1; i < raceTime - 1; i++)
            {
                var holdTime = i;
                var moveTime = raceTime - holdTime;

                var distance = moveTime * holdTime;

                if (distance > recordDistance)
                {
                    winSum++;
                }
            }

            return winSum;
        }

        [GeneratedRegex(@"\d+")]
        private static partial Regex GetDigits();
    }
}
