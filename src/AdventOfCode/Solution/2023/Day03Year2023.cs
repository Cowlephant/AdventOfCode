using AdventOfCode.Core;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solution;

[AoCYearDay(2023, 3)]
public sealed class Day03Year2023 : IAoCDaySolver
{
	[AoCExpectedExampleAnswers("4361")]
	public string SolvePartOne(List<string> input)
	{
		// Adjacency is min-index -1 plus max-index +1 for engine,
		// checked on previous row, current row (excluding number index range) and next row

		var enginePartRegex = new Regex(@"\d+");

		var partNumberSum = 0;

		for (int i = 0; i < input.Count; i++)
		{
			string previousSchematicRow = i > 0 ? input[i - 1] : string.Empty;
			string currentSchematicRow = input[i];
			string nextSchematicRow = i < input.Count - 1 ? input[i + 1] : string.Empty;

			var enginePartMatches = enginePartRegex.Matches(currentSchematicRow);

			foreach (Match enginePartMatch in enginePartMatches)
			{
				bool isPreviousRowValid = IsAnySymbolAdjacent(
						previousSchematicRow,
						enginePartMatch.Value,
						enginePartMatch.Index,
						enginePartMatch.Index + enginePartMatch.Value.Length);

				bool isCurrentRowValid = IsAnySymbolAdjacent(
						currentSchematicRow,
						enginePartMatch.Value,
						enginePartMatch.Index,
						enginePartMatch.Index + enginePartMatch.Value.Length);

				bool isNextRowValid = IsAnySymbolAdjacent(
						nextSchematicRow,
						enginePartMatch.Value,
						enginePartMatch.Index,
						enginePartMatch.Index + enginePartMatch.Value.Length);

				if (isPreviousRowValid
					|| isCurrentRowValid
					|| isNextRowValid)
				{
					partNumberSum += int.Parse(enginePartMatch.Value);
				}
			}
		}

		return partNumberSum.ToString();
	}

	private bool IsAnySymbolAdjacent(
		string schematicRowToCheck,
		string partNumber,
		int partIndexStart,
		int partIndexEnd)
	{
		if (string.IsNullOrWhiteSpace(schematicRowToCheck))
		{
			return false;
		}

		var schematicSubstring = string.Empty;
		if (partIndexStart > 0
			&& partIndexEnd < schematicRowToCheck.Length - 1)
		{
			schematicSubstring = schematicRowToCheck.Substring(partIndexStart - 1, partNumber.Length + 2);
		}
		else if (partIndexStart <= 0)
		{
			schematicSubstring = schematicRowToCheck.Substring(partIndexStart, partNumber.Length + 2);
		}
		else
		{
			schematicSubstring = schematicRowToCheck.Substring(partIndexStart - 1, partNumber.Length + 1);
		}

		return schematicSubstring.Any(c => !char.IsDigit(c) && c != '.');
	}

	[AoCExpectedExampleAnswers("467835")]
	public string SolvePartTwo(List<string> input)
	{
		var gearMatchRegex = new Regex(@"\*");

		var partGearRatiosSum = 0;

		for (int i = 0; i < input.Count; i++)
		{
			string previousSchematicRow = i > 0 ? input[i - 1] : string.Empty;
			string currentSchematicRow = input[i];
			string nextSchematicRow = i < input.Count - 1 ? input[i + 1] : string.Empty;

			var gearMatches = gearMatchRegex.Matches(currentSchematicRow);

			foreach (Match gearMatch in gearMatches)
			{
				int partGearRatio = GetPartGearRatio(gearMatch, previousSchematicRow, currentSchematicRow, nextSchematicRow);

				partGearRatiosSum += partGearRatio;
			}
		}

		return partGearRatiosSum.ToString();
	}

	private int GetPartGearRatio(
		Match gearMatch,
		string previousSchematicRow,
		string currentSchematicRow,
		string nextSchematicRow)
	{
		var enginePartRegex = new Regex(@"\d+");
		var previousEnginePartMatches = enginePartRegex.Matches(previousSchematicRow);
		var currentEnginePartMatches = enginePartRegex.Matches(currentSchematicRow);
		var nextEnginepartMatches = enginePartRegex.Matches(nextSchematicRow);
		var schematicRowLength = currentSchematicRow.Length;

		List<int> partNumbers = [];
		partNumbers.AddRange(GetAdjacentPartNumbers(previousEnginePartMatches, gearMatch.Index, schematicRowLength));
		partNumbers.AddRange(GetAdjacentPartNumbers(currentEnginePartMatches, gearMatch.Index, schematicRowLength));
		partNumbers.AddRange(GetAdjacentPartNumbers(nextEnginepartMatches, gearMatch.Index, schematicRowLength));

		if (partNumbers.Count == 2)
		{
			var partGearRatio = partNumbers[0] * partNumbers[1];
			return partGearRatio;
		}

		return 0;
	}

	private List<int> GetAdjacentPartNumbers(MatchCollection matches, int gearIndex, int schematicRowLength)
	{
		List<int> partNumbers = matches
			.Where(
				p => gearIndex >= p.Index - 1
				&& gearIndex <= p.Index + p.Length)
			.Select(p => int.Parse(p.Value))
			.ToList();

		return partNumbers;
	}
}
