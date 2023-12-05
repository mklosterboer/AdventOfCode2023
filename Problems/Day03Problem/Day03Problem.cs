using AdventOfCode2023.DataStructures;
using AdventOfCode2023.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Problems
{
    internal partial class Day03Problem : Problem
    {
        protected override string InputName => "Actual";

        private List<string> Rows { get; init; }

        public Day03Problem()
        {
            Rows = GetInputStringList().ToList();
        }

        public override object PartOne()
        {
            var sum = 0;

            var map = new Map(Rows[0].Length, Rows.Count);

            for (var i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];

                foreach (Match match in PartNumberRegex().Matches(row))
                {
                    var partNumber = new PartNumber(match, i);
                    if (partNumber.HasPartNear(map, Rows))
                    {
                        sum += partNumber.Value;
                    }
                }
            }

            return sum;
        }

        public override object PartTwo()
        {
            var allGears = new List<Gear>();

            var map = new Map(Rows[0].Length, Rows.Count);

            for (var i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];

                foreach (Match match in PartNumberRegex().Matches(row))
                {
                    var partNumber = new PartNumber(match, i);

                    allGears.AddRange(partNumber.GetNearbyGears(map, Rows));
                }
            }

            var workingGears = allGears
                .GroupBy(x => x.Location)
                .Select(x => new
                {
                    Location = x.Key,
                    PartNumbers = x.Select(gear => gear.PartNumber).ToList()
                })
                .Where(x => x.PartNumbers.Count == 2);

            return workingGears
                .Select(g => g.PartNumbers[0] * g.PartNumbers[1])
                .Sum();
        }

        private class PartNumber
        {
            public int Value { get; init; }

            private int StartIndex { get; init; }

            private int EndIndex { get; init; }

            private int RowIndex { get; init; }

            public PartNumber(Match partNumberMatch, int rowIndex)
            {
                Value = int.Parse(partNumberMatch.Value);
                StartIndex = partNumberMatch.Index;
                EndIndex = partNumberMatch.Index + partNumberMatch.Length - 1;
                RowIndex = rowIndex;
            }

            public bool HasPartNear(Map map, List<string> rows)
            {
                foreach (var searchPoint in GetSearchPoints(map))
                {
                    var point = rows[searchPoint.Y][searchPoint.X];

                    if (!char.IsNumber(point) && point != '.')
                    {
                        return true;
                    }
                }

                return false;
            }

            public List<Gear> GetNearbyGears(Map map, List<string> rows)
            {
                var gears = new List<Gear>();

                foreach (var searchPoint in GetSearchPoints(map))
                {
                    var point = rows[searchPoint.Y][searchPoint.X];

                    if (point == '*')
                    {
                        gears.Add(new Gear(searchPoint, Value));
                    }
                }

                return gears;
            }

            private List<IntVector2> GetSearchPoints(Map map)
            {
                var searchPoints = new List<IntVector2>();

                for (var y = RowIndex - 1; y <= RowIndex + 1; y++)
                {
                    for (var x = StartIndex - 1; x <= EndIndex + 1; x++)
                    {
                        if (x < 0 || x > map.Width - 1 || y < 0 || y > map.Height - 1)
                        {
                            continue;
                        }

                        searchPoints.Add(new IntVector2(x, y));
                    }
                }

                return searchPoints;
            }
        }

        private record Map(int Width, int Height);

        private record Gear(IntVector2 Location, int PartNumber);

        [GeneratedRegex(@"\d+")]
        private static partial Regex PartNumberRegex();
    }
}
