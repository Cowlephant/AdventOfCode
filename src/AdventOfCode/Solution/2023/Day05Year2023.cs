using AdventOfCode.Core;

namespace AdventOfCode.Solution;

[AoCYearDay(2023, 5)]
public sealed class Day05Year2023 : IAoCDaySolver
{
	[AoCExpectedExampleAnswers("35")]
	public string SolvePartOne(List<string> input)
	{
		// First row of data are the seed values
		List<uint> seedsNumbers = input[0]
			.Split(": ")[1]
			.Split(" ")
			.Select(uint.Parse)
			.ToList();

		List<List<ConversionMap>> conversionMaps = string.Join("\n", input)
			.Split("\n\n") // Split each map from each other
			.Skip(1) // Disregard the seeds numbers
			.Select(c =>
				c.Split("\n") // Split each into list on newline
					.Skip(1) // Ignore the map's name
					.Select(n =>
						n.Split(" ") // Split each number on space
							.Select(uint.Parse) // Convert number to int
							.ToList())
					.Select(m => new ConversionMap(m[0], m[1], m[2])) // Map to object for easier reference
					.ToList())
			.ToList();

		uint lowestLocationValue = uint.MaxValue;

		foreach (uint seed in seedsNumbers)
		{
			var seedMappedValue = seed;
			foreach (var map in conversionMaps)
			{
				// Ensure that the mapped value is between range of source values
				var mappedValue = map.Find(m =>
					seedMappedValue >= m.Source
					&& seedMappedValue <= m.Source + m.Count - 1);

				if (mappedValue is not null)
				{
					// Relationship between any value and its mapped value if it exists, is VALUE = VALUE + (DESTINATION - SOURCE)
					// otherwise it's just the same value
					seedMappedValue += (mappedValue.Destination - mappedValue.Source);
				}
			}

			lowestLocationValue =
				seedMappedValue < lowestLocationValue ?
				seedMappedValue :
				lowestLocationValue;
		}

		return lowestLocationValue.ToString();
	}

	[AoCExpectedExampleAnswers("46")]
	public string SolvePartTwo(List<string> input)
	{
		// First row of data are the seed values
		List<uint> seedsNumbers = input[0]
			.Split(": ")[1]
			.Split(" ")
			.Select(uint.Parse)
			.ToList();

		List<SeedRange> seedsRanges = Enumerable.Range(0, seedsNumbers.Count / 2)
			.Select(i => seedsNumbers.Skip(i * 2).Take(2).ToList())
			.Select(s => new SeedRange(s[0], s[0] + s[1] - 1, s[1]))
			.ToList();

		List<List<ConversionMap>> conversionMaps = string.Join("\n", input)
		   .Split("\n\n") // Split each map from each other
		   .Skip(1) // Disregard the seeds numbers
		   .Select(c =>
			   c.Split("\n") // Split each into list on newline
				   .Skip(1) // Ignore the map's name
				   .Select(n =>
					   n.Split(" ") // Split each number on space
						   .Select(uint.Parse) // Convert number to int
						   .ToList())
				   .Select(m => new ConversionMap(m[0], m[1], m[2])) // Map to object for easier reference
				   .ToList())
		   .ToList();

		uint lowestLocationValue = uint.MaxValue;
		uint lowestLocationSeed = uint.MaxValue;

		foreach (var seedRange in seedsRanges)
		{
			ProcessSeedRange(
				conversionMaps,
				seedRange.Starting,
				seedRange.Ending,
				ref lowestLocationValue,
				ref lowestLocationSeed);
		}

		// Refactor these out as they share mostly the same code, just moving in different directions

		bool shouldContinue = true;
		// Try lower values from our lowest value until no longer lower
		while (shouldContinue)
		{
			uint lowerSeedValue = lowestLocationSeed - 1;
			// Validate new value is an acceptable range
			bool isNotInValidrange = !seedsRanges.Exists(
				s => lowestLocationSeed >= s.Starting
					&& lowestLocationSeed <= s.Ending);
			if (isNotInValidrange)
			{
				shouldContinue = false;
				continue;
			}

			uint seedMappedValue = RunThroughMap(conversionMaps, lowerSeedValue);

			if (seedMappedValue < lowestLocationValue)
			{
				lowestLocationValue = seedMappedValue;
				lowestLocationSeed = lowerSeedValue;

				Console.WriteLine("Walking lower... found lower value");
			}
			else
			{
				shouldContinue = false;
			}
		}

		shouldContinue = true;
		// Try higher values from our lowest value until no longer lower
		while (shouldContinue)
		{
			uint higherSeedValue = lowestLocationSeed + 1;
			// Validate new value is an acceptable range
			bool isNotInValidrange = !seedsRanges.Exists(
				s => lowestLocationSeed >= s.Starting
					&& lowestLocationSeed <= s.Ending);
			if (isNotInValidrange)
			{
				shouldContinue = false;
				continue;
			}

			uint seedMappedValue = RunThroughMap(conversionMaps, higherSeedValue);

			if (seedMappedValue < lowestLocationValue)
			{
				lowestLocationValue = seedMappedValue;
				lowestLocationSeed = higherSeedValue;
				Console.WriteLine("Walking higher... found lower value");
			}
			else
			{
				shouldContinue = false;
			}
		}

		return lowestLocationValue.ToString();
	}

	private void ProcessSeedRange(
		List<List<ConversionMap>> conversionMaps,
		uint newRangeStart,
		uint newRangeEnd,
		ref uint lowestLocationValue,
		ref uint lowestLocationSeed)
	{
		uint centerSeedToTry = (uint)Math.Round((double)((newRangeStart + newRangeEnd) / 2), 0);

		// Somehow our center seed is no longer within the acceptable range.
		// Break out of the recursion
		if (centerSeedToTry < newRangeStart
			|| centerSeedToTry > newRangeEnd)
		{
			return;
		}

		uint seedMappedValue = RunThroughMap(conversionMaps, centerSeedToTry);

		if (seedMappedValue < lowestLocationValue)
		{
			lowestLocationValue = seedMappedValue;
			lowestLocationSeed = centerSeedToTry;
			Console.WriteLine($"Found lower location value: {lowestLocationValue}");
		}

		if (newRangeEnd - newRangeStart >= 100)
		{
			// Recurse on lower half
			ProcessSeedRange(
				conversionMaps,
				newRangeStart,
				centerSeedToTry,
				ref lowestLocationValue,
				ref lowestLocationSeed);
			// Recurse on uopper half
			ProcessSeedRange(
				conversionMaps,
				centerSeedToTry,
				newRangeEnd,
				ref lowestLocationValue,
				ref lowestLocationSeed);
		}
	}

	private static uint RunThroughMap(List<List<ConversionMap>> conversionMaps, uint centerSeedToTry)
	{
		uint seedMappedValue = centerSeedToTry;

		foreach (var map in conversionMaps)
		{
			// Ensure that the mapped value is between range of source values
			var mappedValue = map.Find(m =>
				seedMappedValue >= m.Source
				&& seedMappedValue <= m.Source + m.Count - 1);

			if (mappedValue is not null)
			{
				// Relationship between any value and its mapped value if it exists, is VALUE = VALUE + (DESTINATION - SOURCE)
				// otherwise it's just the same value
				seedMappedValue += (mappedValue.Destination - mappedValue.Source);
			}
		}

		return seedMappedValue;
	}

	private sealed record class SeedRange(uint Starting, uint Ending, uint Count)
	{
	}

	private sealed record class ConversionMap(uint Destination, uint Source, uint Count)
	{
	}
}