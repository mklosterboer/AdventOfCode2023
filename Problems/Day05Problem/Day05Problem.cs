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

        public override object PartTwo()
        {
            var input = GetInputStringList().ToList();

            var seedFactory = new SeedFactory(input);

            var seedPairs = new Regex(@"\d+ \d+")
                .Matches(GetFirstRow().Split(":")[1])
                .Select(x => x.Value.Split(" ").Select(y => long.Parse(y)).ToArray());

            var seedRanges = seedPairs.Select(x => new LongRange(x[0], x[0] + x[1] - 1)).ToList();

            // Shamelessly "borrowed" from Kevin's solution
            var soilRanges = SeedFactory.GetMapRanges(seedRanges, seedFactory.SeedToSoilMap);
            var fertRanges = SeedFactory.GetMapRanges(soilRanges, seedFactory.SoilToFertMap);
            var waterRanges = SeedFactory.GetMapRanges(fertRanges, seedFactory.FertToWaterMap);
            var lightRanges = SeedFactory.GetMapRanges(waterRanges, seedFactory.WaterToLightMap);
            var tempRanges = SeedFactory.GetMapRanges(lightRanges, seedFactory.LightToTempMap);
            var humdityRanges = SeedFactory.GetMapRanges(tempRanges, seedFactory.TempToHumidityMap);
            var locationRanges = SeedFactory.GetMapRanges(humdityRanges, seedFactory.HumidityToLocationMap);

            return locationRanges?.MinBy(x => x.Start)?.Start ?? 0;
        }

        private class Mapping
        {
            public List<MapRange> Ranges { get; init; }

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
                    var length = matches[2] - 1;

                    Ranges.Add(new MapRange(sourceStart, destinationStart, length));
                }

                Ranges = Ranges.OrderBy(x => x.DestinationStart).ToList();
            }

            public long GetMappedValue(long value)
            {
                foreach (var range in Ranges)
                {
                    if (TryGetDestination(range, value, out var destination))
                    {
                        return destination;
                    }
                }

                return value;
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

        private record Seed(long Id, long Soil, long Fert, long Water, long Light, long Temp, long Humidity, long Location);

        private class SeedFactory(List<string> input)
        {
            public Mapping SeedToSoilMap { get; init; } = new Mapping(input, "seed-to-soil map:");
            public Mapping SoilToFertMap { get; init; } = new Mapping(input, "soil-to-fertilizer map:");
            public Mapping FertToWaterMap { get; init; } = new Mapping(input, "fertilizer-to-water map:");
            public Mapping WaterToLightMap { get; init; } = new Mapping(input, "water-to-light map:");
            public Mapping LightToTempMap { get; init; } = new Mapping(input, "light-to-temperature map:");
            public Mapping TempToHumidityMap { get; init; } = new Mapping(input, "temperature-to-humidity map:");
            public Mapping HumidityToLocationMap { get; init; } = new Mapping(input, "humidity-to-location map:");

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

            public static List<LongRange> GetMapRanges(List<LongRange> source, Mapping destination)
            {
                var result = new List<LongRange>();

                foreach (var sourceRange in source.OrderBy(x => x.Start))
                {
                    var sourceRanges = new List<LongRange>();
                    var localDestinationRanges = new List<LongRange>();

                    foreach (var destinationRange in destination.Ranges.OrderBy(x => x.SourceStart))
                    {
                        if (sourceRange.Start > destinationRange.SourceEnd)
                        {
                            // This range does not overlap at all
                            continue;
                        }
                        if (sourceRange.Start >= destinationRange.SourceStart && sourceRange.End <= destinationRange.SourceEnd)
                        {
                            // Source range is entirely within destination range
                            var startOffset = sourceRange.Start - destinationRange.SourceStart;
                            var endOffset = destinationRange.SourceEnd - sourceRange.End;

                            localDestinationRanges.Add(new LongRange(destinationRange.DestinationStart + startOffset, destinationRange.DestinationEnd - endOffset));
                            sourceRanges.Add(sourceRange);

                            // No need to continue, we covered the entire source range.
                            break;
                        }

                        if (sourceRange.Start > destinationRange.SourceStart && sourceRange.Start <= destinationRange.SourceEnd && sourceRange.End > destinationRange.SourceEnd)
                        {
                            // Source overlaps end of destination range
                            var overlapAmount = destinationRange.SourceEnd - sourceRange.Start;

                            localDestinationRanges.Add(new LongRange(destinationRange.DestinationEnd - overlapAmount, destinationRange.DestinationEnd));
                            sourceRanges.Add(new LongRange(sourceRange.Start, destinationRange.SourceEnd));
                        }
                        else if (sourceRange.Start <= destinationRange.SourceStart && sourceRange.End >= destinationRange.SourceEnd)
                        {
                            // Destination range is entirely within source range
                            localDestinationRanges.Add(new LongRange(destinationRange.DestinationStart, destinationRange.DestinationEnd));
                            sourceRanges.Add(new LongRange(destinationRange.SourceStart, destinationRange.SourceEnd));
                        }
                        else if (sourceRange.Start < destinationRange.SourceStart && sourceRange.End >= destinationRange.SourceStart && sourceRange.End < destinationRange.SourceEnd)
                        {
                            // Source overlaps start of destination range
                            var overlapAmount = sourceRange.End - destinationRange.SourceStart;

                            localDestinationRanges.Add(new LongRange(destinationRange.DestinationStart, destinationRange.DestinationStart + overlapAmount));
                            sourceRanges.Add(new LongRange(destinationRange.SourceStart, sourceRange.End));
                        }
                        else if (sourceRange.End < destinationRange.SourceStart)
                        {
                            // No more overlap to check
                            break;
                        }
                    }

                    var sourceCopy = sourceRange with { };
                    foreach (var range in sourceRanges.OrderBy(x => x.Start))
                    {
                        if (sourceCopy.Start < range.Start)
                        {
                            localDestinationRanges.Add(new LongRange(sourceCopy.Start, range.Start - 1));
                            sourceCopy = sourceCopy with { Start = range.End + 1 };
                        }
                        else if (sourceCopy.Start == range.Start && sourceCopy.End != range.End)
                        {
                            sourceCopy = sourceCopy with { Start = range.End + 1 };
                        }
                        else if (sourceCopy.End > range.End)
                        {
                            localDestinationRanges.Add(new LongRange(range.End + 1, sourceCopy.End));
                        }
                    }

                    if (localDestinationRanges.Count == 0)
                    {
                        localDestinationRanges.Add(sourceRange);
                    }

                    result.AddRange(localDestinationRanges);
                }

                return result;
            }
        }

        private record MapRange(long SourceStart, long DestinationStart, long Length)
        {
            public long SourceEnd { get; init; } = SourceStart + Length;
            public long DestinationEnd { get; init; } = DestinationStart + Length;
        }

        private record LongRange(long Start, long End);
    }
}
