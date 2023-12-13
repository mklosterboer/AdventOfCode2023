using AdventOfCode2023.Utilities;

namespace AdventOfCode2023.Problems
{
    internal class Day09Problem : Problem
    {
        protected override string InputName => "Actual";

        public override object PartOne()
        {
            var input = GetInputStringList().ToArray();

            var lines = input.Select(line =>
                line.Split(" ")
                    .Select(x => int.Parse(x))
                    .ToArray()
                )
                .ToArray();

            return lines.Sum(GetExtrapolatedValue);
        }

        public override object PartTwo()
        {
            var input = GetInputStringList().ToArray();

            var lines = input.Select(line =>
                line.Split(" ")
                    .Select(x => int.Parse(x))
                    .Reverse()
                    .ToArray()
                )
                .ToArray();

            return lines.Sum(GetExtrapolatedValue);
        }

        private int GetExtrapolatedValue(int[] line)
        {
            var sequences = new List<int[]>() { line };

            while (sequences[^1].Any(x => x != 0))
            {
                var currentSeq = sequences[^1];
                List<int> newSeq = [];

                // Skipping the first number in a sequence,
                // subtract the previous value from the current value
                for(var i = 1; i < currentSeq.Length; i++)
                {
                    newSeq.Add(currentSeq[i] - currentSeq[i - 1]);
                }

                sequences.Add([.. newSeq]);
            }

            var result = 0;

            // Starting from the first non-zero row,
            // add the end of each sequence
            for(var i = sequences.Count - 2; i >= 0; i--)
            {
                result += sequences[i][^1];
            }

            return result;
        }
    }
}
