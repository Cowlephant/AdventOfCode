using AdventOfCode.Core;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solution;

[AoCYearDay(2023, 2)]
public sealed class Day02Year2023 : IAoCDaySolver
{
	[AoCExpectedExampleAnswers("8")]
	public string SolvePartOne(List<string> input)
	{
		var possibleGameIdSum = 0;

		foreach (var line in input)
		{
			Dictionary<string, int> blockColorCounts = new()
			{
				{ "red", 0 },
				{ "green", 0 },
				{ "blue", 0 }
			};

			Dictionary<string, int> blockCountThresholds = new()
			{
				{ "red", 12 },
				{ "green", 13 },
				{ "blue", 14 }
			};

			var indexMatch = new Regex(@"\d+").Match(line).Value;
			var gameIndex = int.Parse(indexMatch);
			var modifiedLine = Regex.Replace(line, @"^Game \d+: ", string.Empty);

			int currentBlockCount = 0;
			bool isGamePossible = true;

			var lineChunks = modifiedLine
				.Replace(";", " ;")
				.Split(' ')
				.Select(s => s.Replace(",", string.Empty));

			foreach (var chunk in lineChunks)
			{
				// Chunk is a delimiter, starting new set
                if (chunk == ";")
                {
					blockColorCounts["red"] = 0;
					blockColorCounts["green"] = 0;
					blockColorCounts["blue"] = 0;
					continue;
				}
                // Chunk is a block count
                if (int.TryParse(chunk, out int blockCount))
				{
					currentBlockCount = blockCount;
				}
				// Chunk is a block color descriptor
				else
				{
					blockColorCounts[chunk] += currentBlockCount;
					if (blockColorCounts[chunk] > blockCountThresholds[chunk])
					{
						isGamePossible = false;
						break;
					}
				}
			}

			if (isGamePossible)
			{
				possibleGameIdSum += gameIndex;
			}
		}

		return possibleGameIdSum.ToString();
	}

	[AoCExpectedExampleAnswers("2286")]
	public string SolvePartTwo(List<string> input)
	{
		var possibleGamePowers = 0;

		foreach (var line in input)
		{
			Dictionary<string, int> blockColorMaxCounts = new()
			{
				{ "red", 0 },
				{ "green", 0 },
				{ "blue", 0 }
			};

			var modifiedLine = Regex.Replace(line, @"^Game \d+: ", string.Empty);

			int currentBlockCount = 0;

			var lineChunks = modifiedLine
				.Replace(";", " ;")
				.Split(' ')
				.Select(s => 
					s.Replace(",", string.Empty));

			foreach (var chunk in lineChunks)
			{
				// Chunk is a delimiter or blank, continue on with yourself
				if (chunk == ";" || string.IsNullOrWhiteSpace(chunk))
				{
					continue;
				}
				// Chunk is a block count
				if (int.TryParse(chunk, out int blockCount))
				{
					currentBlockCount = blockCount;
				}
				// Chunk is a block color descriptor
				else
				{
					blockColorMaxCounts[chunk] = 
						currentBlockCount > blockColorMaxCounts[chunk] ? 
						currentBlockCount : 
						blockColorMaxCounts[chunk];
				}
			}

			possibleGamePowers +=
				(blockColorMaxCounts["red"] *
				blockColorMaxCounts["green"] *
				blockColorMaxCounts["blue"]);
		}

		return possibleGamePowers.ToString();
	}
}
