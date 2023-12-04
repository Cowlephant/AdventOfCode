using AdventOfCode.Core;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solution;

[AoCYearDay(2023, 4)]
public sealed class Day04Year2023 : IAoCDaySolver
{
	[AoCExpectedExampleAnswers("13")]
	public string SolvePartOne(List<string> input)
	{
		var cardNumberRegex = new Regex(@"(?!\d+:)\d+");

		var totalCardPoints = 0;

		foreach (var line in input)
		{
			var numbers = line.Split('|');
			var winningNumbers = cardNumberRegex.Matches(numbers[0])
				.Select(m => m.Value);
			var numbersChosen = cardNumberRegex.Matches(numbers[1])
				.Select(m => m.Value);
			var winningChosenNumbersCount = numbersChosen
				.Count(n => winningNumbers.Contains(n));

			totalCardPoints += (int)Math.Pow(2, winningChosenNumbersCount - 1);
		}

		return totalCardPoints.ToString();
	}

	[AoCExpectedExampleAnswers("30")]
	public string SolvePartTwo(List<string> input)
	{
		// Matches numbers in winning and chosen cards
		var numbersRegex = new Regex(@"(?!\d+:)\d+");
		// Matches the card number itself
		var cardNumberRegex = new Regex(@"\d+(?=:)");

		// Card numbers, and how many times to process them, for future cards we reach
		Dictionary<int, int> cardsTimesToProcess = Enumerable.Range(1, input.Count)
			.Select(i => new KeyValuePair<int, int>(i, 0))
			.ToDictionary();

		var totalScratchCards = 0;

		foreach (var line in input)
		{
			var cardNumber = int.Parse(cardNumberRegex.Match(line).Value);
			var numbers = line.Split('|');
			var winningNumbers = numbersRegex.Matches(numbers[0])
				.Select(m => m.Value);
			var numbersChosen = numbersRegex.Matches(numbers[1])
				.Select(m => m.Value);
			var matchingNumberCount = numbersChosen
				.Count(n => winningNumbers.Contains(n));

			var timesToProcess = cardsTimesToProcess[cardNumber];

			var totalToAdd = 0;

			for (int i = 0; i <= timesToProcess; i++)
			{
				for (int j = 1; j <= matchingNumberCount; j++)
				{
					cardsTimesToProcess[cardNumber + j]++;
				}

				totalToAdd++;
			}

			totalScratchCards += totalToAdd;
		}

		return totalScratchCards.ToString();
	}
}
