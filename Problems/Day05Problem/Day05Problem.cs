using AdventOfCode2023.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Problems
{
    internal class Day05Problem : Problem
    {
        protected override string InputName => "Actual";

        public override object PartOne()
        {
            var seedIds = new Regex(@"\d+").Matches(GetFirstRow().Split(":")[1]).Select(x => long.Parse(x.Value));

            var input = GetInputStringList().ToList();

            var seedFactory = new SeedFactory(input);

            var seeds = seedIds.Select(x => seedFactory.Create(x));

            return seeds.Min(x => x.Location);
        }

        // BRUTE FORCE - DON'T DO THIS - TOOK 38 MINUTES
        public override object PartTwo()
        {
            var input = GetInputStringList().ToList();

            var seedFactory = new SeedFactory(input);

            var seedPairs = new Regex(@"\d+ \d+")
                .Matches(GetFirstRow().Split(":")[1])
                .Select(x => x.Value.Split(" ").Select(y => long.Parse(y)).ToArray());
            var seedRanges = seedPairs.Select(x => new SeedRange(x[0], x[1]));

            object lockMinLocation = new();
            var minLocation = long.MaxValue;

            Parallel.ForEach(seedRanges, seedRange =>
            {
                var localMinLocation = long.MaxValue;
                var end = seedRange.Start + seedRange.Length;
                for (var i = seedRange.Start; i < end; i++)
                {
                    var seed = seedFactory.Create(i);

                    if (seed.Location < minLocation)
                    {
                        minLocation = seed.Location;
                    }
                    if (i % 1000000 == 0)
                    {
                        Console.WriteLine($"{seedRange.Start} - Remaining {end - i}");
                    }
                }
                lock (lockMinLocation)
                {
                    if (localMinLocation < minLocation)
                    {
                        minLocation = localMinLocation;
                    }

                }
                Console.WriteLine($"Done {seedRange.Start} {seedRange.Length}");
            });

            return minLocation;
        }

        public class Mapping
        {
            private List<MapRange> Ranges { get; init; }

            private static Regex MapRegex = new(@"\d+");

            public Mapping(List<string> rows, string sourceHeader)
            {
                Ranges = [];

                var startIndex = rows.FindIndex(x => x == sourceHeader);
                var endIndex = rows.FindIndex(startIndex, x => string.IsNullOrEmpty(x));

                if (endIndex == -1)
                {
                    endIndex = rows.Count;
                }

                for (int i = startIndex + 1; i < endIndex; i++)
                {
                    var value = rows[i];

                    var matches = MapRegex.Matches(value).Select(x => long.Parse(x.Value)).ToArray();

                    var destinationStart = matches[0];
                    var sourceStart = matches[1];
                    var length = matches[2];

                    Ranges.Add(new MapRange(sourceStart, destinationStart, length));
                }
            }

            public long GetMappedValue(long value)
            {
                foreach (var range in Ranges)
                {
                    if (range.TryGetDestination(value, out var destination))
                    {
                        return destination;
                    }
                }

                return value;
            }
        }

        private record Seed(long Id, long Soil, long Fert, long Water, long Light, long Temp, long Humidity, long Location);

        private class SeedFactory(List<string> input)
        {
            private Mapping SeedToSoilMap { get; init; } = new Mapping(input, "seed-to-soil map:");
            private Mapping SoilToFertMap { get; init; } = new Mapping(input, "soil-to-fertilizer map:");
            private Mapping FertToWaterMap { get; init; } = new Mapping(input, "fertilizer-to-water map:");
            private Mapping WaterToLightMap { get; init; } = new Mapping(input, "water-to-light map:");
            private Mapping LightToTempMap { get; init; } = new Mapping(input, "light-to-temperature map:");
            private Mapping TempToHumidityMap { get; init; } = new Mapping(input, "temperature-to-humidity map:");
            private Mapping HumidityToLocationMap { get; init; } = new Mapping(input, "humidity-to-location map:");

            public Seed Create(long seedId)
            {
                var soil = SeedToSoilMap.GetMappedValue(seedId);
                var fert = SoilToFertMap.GetMappedValue(soil);
                var water = FertToWaterMap.GetMappedValue(fert);
                var light = WaterToLightMap.GetMappedValue(water);
                var temp = LightToTempMap.GetMappedValue(light);
                var humidity = TempToHumidityMap.GetMappedValue(temp);
                var location = HumidityToLocationMap.GetMappedValue(humidity);

                return new Seed(seedId, soil, fert, water, light, temp, humidity, location);
            }
        }

        private class MapRange(long sourceStart, long destinationStart, long length)
        {
            public long SourceStart { get; init; } = sourceStart;

            public long SourceEnd { get; init; } = sourceStart + length;

            public long DestinationStart { get; init; } = destinationStart;

            public bool TryGetDestination(long source, out long destination)
            {
                if (Contains(source))
                {
                    var offset = source - SourceStart;

                    destination = DestinationStart + offset;
                    return true;
                }

                destination = 0;
                return false;
            }

            private bool Contains(long value)
            {
                return value >= SourceStart && value <= SourceEnd;
            }
        }

        private record SeedRange(long Start, long Length);
    }
}
