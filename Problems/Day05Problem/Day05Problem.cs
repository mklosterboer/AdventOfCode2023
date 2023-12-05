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

        // BRUTE FORCE - DON'T DO THIS - TOOK ~3 MINUTES with 8 cores
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

                long endCurrentRange = 1;

                for (var i = seedRange.Start; i < end; i += endCurrentRange)
                {
                    var seed = seedFactory.Create(i);
                    endCurrentRange = seed.RangeEnd;

                    if (seed.Location < minLocation)
                    {
                        minLocation = seed.Location;
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

        private class Mapping
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

            public (long value, long endOfRange) GetMappedValue(long value)
            {
                foreach (var range in Ranges)
                {
                    if (TryGetDestination(range, value, out var destination))
                    {
                        return (destination, range.SourceEnd);
                    }
                }

                return (value, 1);
            }

            public static bool TryGetDestination(MapRange range, long source, out long destination)
            {
                if (source >= range.SourceStart && source <= range.SourceEnd)
                {
                    destination = range.DestinationStart + (source - range.SourceStart);
                    return true;
                }

                destination = 0;
                return false;
            }
        }

        private record Seed(long Id, long Soil, long Fert, long Water, long Light, long Temp, long Humidity, long Location, long RangeEnd);

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
                var fert = SoilToFertMap.GetMappedValue(soil.value);
                var water = FertToWaterMap.GetMappedValue(fert.value);
                var light = WaterToLightMap.GetMappedValue(water.value);
                var temp = LightToTempMap.GetMappedValue(light.value);
                var humidity = TempToHumidityMap.GetMappedValue(temp.value);
                var location = HumidityToLocationMap.GetMappedValue(humidity.value);

                // We can skip forward by the minimum value of the rest of any given the range.
                // EX 1: If fert.endOfRange = 1, it was not in a supplied range, so we cannot skip anything
                // EX 2: If fert.endOfRange = 10 and water.endOfRange = 20 and the rest are >20, we can skip the next 10 seeds because
                //       the next 10 ferts will produce a larger water number and everything else will also continue sequentially up.
                //       Therefore, none of the next 10 seeds are going to produce a location smaller than this one.
                long[] endOfRange = [soil.endOfRange, fert.endOfRange, water.endOfRange, light.endOfRange, temp.endOfRange, humidity.endOfRange, location.endOfRange];

                return new Seed(seedId, soil.value, fert.value, water.value, light.value, temp.value, humidity.value, location.value, endOfRange.Min());
            }
        }

        private record MapRange(long SourceStart, long DestinationStart, long Length)
        {
            public long SourceEnd { get; init; } = SourceStart + Length;
        }

        private record SeedRange(long Start, long Length);
    }
}
