using AdventOfCode2023.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Problems
{
    internal partial class Day08Problem : Problem
    {
        protected override string InputName => "Actual";

        public override object PartOne()
        {
            var input = GetInputStringList().ToArray();
            var map = new Map(input);

            var startingNode = map.Nodes["AAA"];

            return map.GetTicksToEnd(startingNode, nodeValue => nodeValue == "ZZZ");
        }

        public override object PartTwo()
        {
            var input = GetInputStringList().ToArray();
            var map = new Map(input);

            // Each starting node will always cycle in the same amount of ticks.
            // So, they will sync up on the least common multiple of all ticks.
            // Therefore, don't interate them together:
            // Get the number of ticks for each and find the least common multiple. 
            var ticksPerStartingNode = map.Nodes
                .Where(x => x.Value.Value[2] == 'A')
                .Select(x => map.GetTicksToEnd(x.Value, nodeValue => nodeValue[2] == 'Z'))
                .ToArray();

            return LCMofSet(ticksPerStartingNode);
        }

        // Least Common Multiple of a set of numbers
        // Taken from: https://stackoverflow.com/a/29717490
        private static long LCMofSet(IEnumerable<long> numbers)
        {
            return numbers.Aggregate(LCM);
        }

        private static long LCM(long a, long b)
        {
            return Math.Abs(a * b) / GCD(a, b);
        }

        private static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}
