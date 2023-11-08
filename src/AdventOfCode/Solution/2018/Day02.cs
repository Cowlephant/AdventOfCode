using AdventOfCode.Core;

namespace AdventOfCode.Solution
{
	[AoCYear(2018)]
	public sealed class Day02 : IAoCDaySolver
	{
		[AoCExpectedExampleAnswers("12")]
		public string SolvePartOne(List<string> input)
		{
			int totalTwoCount = 0;
			int totalThreeCount = 0;

			input.ForEach(boxId =>
			{
				char[] boxLetters = boxId.ToCharArray();

				bool hasTwoCount = boxLetters.GroupBy(l => l)
					.Select(g => g.Count()).Any(g => g == 2);
				bool hasThreeCount = boxLetters.GroupBy(l => l)
					.Select(g => g.Count()).Any(g => g == 3);

				totalTwoCount += hasTwoCount ? 1: 0;
				totalThreeCount += hasThreeCount ? 1 : 0;
			});

			int checksum = totalTwoCount * totalThreeCount;

			return checksum.ToString();
		}

		[AoCExpectedExampleAnswers("fgij")]
		public string SolvePartTwo(List<string> input)
		{
            for(int i = 0; i < input.Count; i++)
			{
				var boxLength = input[i].Length;

				for (int j = 0; j < input.Count; j++)
				{
					if (i != j)
					{
						var boxA = input[i].ToCharArray();
						var boxB = input[j].ToCharArray();

						var scannedBox = boxA.Select((a, i) => boxB[i] == a ? a.ToString() : string.Empty)
							.ToList();
						scannedBox.RemoveAll(b => string.IsNullOrWhiteSpace(b));
						bool isPrototypeBox = scannedBox.Count == boxLength - 1;
						if (isPrototypeBox)
						{
							return string.Join("", scannedBox);
						}
					}
				}
			}

			return "No Prototype Box Found!";
		}
	}
}
