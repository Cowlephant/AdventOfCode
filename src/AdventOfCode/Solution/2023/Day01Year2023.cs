using AdventOfCode.Core;

namespace AdventOfCode.Solution;

[AoCYearDay(2023, 1)]
public sealed class Day01Year2023 : IAoCDaySolver
{
	[AoCExpectedExampleAnswers("142")]
	public string SolvePartOne(List<string> input)
	{
		return SumFirstLastDigits(input);
	}

	[AoCExpectedExampleAnswers("281")]
	public string SolvePartTwo(List<string> input)
	{
		Dictionary<string, int> wordsDigits = new Dictionary<string, int>
		{
			{ "one", 1 },
			{ "two", 2 },
			{ "three", 3 },
			{ "four", 4 },
			{ "five", 5 },
			{ "six", 6 },
			{ "seven", 7 },
			{ "eight", 8 },
			{ "nine", 9 },
			{ "1", 1 },
			{ "2", 2 },
			{ "3", 3 },
			{ "4", 4 },
			{ "5", 5 },
			{ "6", 6 },
			{ "7", 7 },
			{ "8", 8 },
			{ "9", 9 }
		};

		List<int> modifiedInput = new();

		for (int i = 0; i < input.Count; i++)
		{
			Dictionary<int, int> words = new();

			foreach (var word in wordsDigits)
			{
				var index = input[i].IndexOf(word.Key);

				if (index > -1)
				{
					words.Add(index, word.Value);
				}
			}

			foreach (var word in wordsDigits)
			{
				var index = input[i].LastIndexOf(word.Key);

				if (index > -1)
				{
					words.TryAdd(index, word.Value);
				}
			}

			var digitsOrdered = words.OrderBy(w => w.Key);
			var earliestDigit = digitsOrdered.First();
			var latestDigit = digitsOrdered.Last();
			var parsedValue = $"{earliestDigit.Value}{latestDigit.Value}";
			modifiedInput.Add(int.Parse(parsedValue));
		}

		return modifiedInput.Sum().ToString();
	}

	private string SumFirstLastDigits(List<string> input)
	{
		List<int> digits = new List<int>();

		for (var i = 0; i < input.Count; i++)
		{
			int firstDigit = 0;
			int lastDigit = 0;

			// Get first digit
			for (var j = 0; j < input[i].Length; j++)
			{
				var item = input[i][j].ToString();
				if (int.TryParse(item, out int number))
				{
					firstDigit = number;
					break;
				}
			}

			// Get last digit
			for (var j = (input[i].Length) - (1); j >= 0; j--)
			{
				var item = input[i][j].ToString();
				if (int.TryParse(item, out int number))
				{
					lastDigit = number;
					break;
				}
			}

			digits.Add(int.Parse($"{firstDigit}{lastDigit}"));
		}

		return digits.Sum().ToString();
	}
}
