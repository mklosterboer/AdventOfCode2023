using AdventOfCode2023.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Problems
{
    internal partial class Day02Problem : Problem
    {
        protected override string InputName => "Actual";

        private IEnumerable<Game> Games { get; init; }

        public Day02Problem()
        {
            Games = GetInputStringList().Select(x => new Game(x));
        }

        public override object PartOne()
        {
            var sumPossible = 0;

            foreach (Game game in Games)
            {
                if (!game.ExceedsLimits(12, 13, 14))
                {
                    sumPossible += game.ID;
                }
            }

            return sumPossible;
        }

        public override object PartTwo()
        {
            return Games.Sum(x => x.Power());
        }

        private partial class Game
        {
            public int ID { get; init; }

            public List<Draw> Draws { get; init; }

            [GeneratedRegex(@"^Game (?'id'\d*)")]
            private static partial Regex GameIdRegex();

            public Game(string input)
            {
                var splitString = input.Split(':');
                var val = GameIdRegex().Match(splitString[0]).Groups["id"].Value;
                ID = int.Parse(val);

                var drawsString = splitString[1].Split(';');

                Draws = drawsString.Select(x => new Draw(x)).ToList();
            }

            public bool ExceedsLimits(int red, int green, int blue)
            {
                foreach (var draw in Draws)
                {
                    if (draw.ExceedsLimits(red, green, blue))
                    {
                        return true;
                    }
                }

                return false;
            }

            public int Power()
            {
                var maxRed = Draws.Select(x => x.Red).Max();
                var maxGreen = Draws.Select(x => x.Green).Max();
                var maxBlue = Draws.Select(x => x.Blue).Max();

                return maxRed * maxGreen * maxBlue;
            }
        }

        private partial class Draw
        {
            public int Red { get; init; }

            public int Green { get; init; }

            public int Blue { get; init; }

            [GeneratedRegex(@"(?'count'\d*) (?'color'(red|blue|green))")]
            private static partial Regex DrawRegex();

            public Draw(string input)
            {
                foreach (Match match in DrawRegex().Matches(input))
                {
                    var count = int.Parse(match.Groups["count"].Value);
                    var color = match.Groups["color"].Value;

                    switch (color)
                    {
                        case "red":
                            Red = count;
                            break;
                        case "green":
                            Green = count;
                            break;
                        case "blue":
                            Blue = count;
                            break;
                    }
                }
            }

            public bool ExceedsLimits(int red, int green, int blue) =>
                Red > red || Green > green || Blue > blue;
        }
    }
}
