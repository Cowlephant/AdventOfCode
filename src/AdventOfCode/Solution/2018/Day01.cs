using AdventOfCode.Core;

namespace AdventOfCode.Solution
{
	[AdventOfCodeYear(2018)]
	public sealed class Day01 : AdventOfCodeDayBase
	{
		[ExpectedExampleAnswers("3", "0", "-6")]
		public override string RunPartOne(IEnumerable<string> dataSet)
		{
			var finalFrequency = 0;

			foreach (var item in dataSet)
			{
				var frequency = int.Parse(item);
				finalFrequency += frequency;
			}

			return finalFrequency.ToString();
		}

		[ExpectedExampleAnswers("0", "10", "5", "14")]
		public override string RunPartTwo(IEnumerable<string> dataSet)
		{
			var usedFrequencies = new HashSet<int>() { 0 };
			var currentFrequency = 0;
			var scanningFrequencies = true;
			var firstRepeatingFrequency = 0;

			while (scanningFrequencies)
			{
				foreach (var item in dataSet)
				{
					var frequencyChange = int.Parse(item);
					currentFrequency += frequencyChange;

					if (usedFrequencies.TryGetValue(currentFrequency, out int _))
					{
						firstRepeatingFrequency = currentFrequency;
						scanningFrequencies = false;
						break;
					}

					usedFrequencies.Add(currentFrequency);
				}
			}

			return firstRepeatingFrequency.ToString();
		}
	}
}
