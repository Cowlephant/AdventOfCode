namespace AdventOfCode.Solution
{
	[AdventOfCodeYear(2018)]
	public sealed class Day01 : AdventOfCodeDayBase
	{
		[ExpectedExampleAnswer("1")]
		public override string RunPartOne()
		{
			var input = DayInputReader.GetData();

			var finalFrequency = 0;

			foreach (var item in input)
			{
				var frequency = int.Parse(item);
				finalFrequency += frequency;
			}

			return finalFrequency.ToString();
		}

		[ExpectedExampleAnswer("897")]
		public override string RunPartTwo()
		{
			var input = DayInputReader.GetData();

			var usedFrequencies = new HashSet<int>() { 0 };
			var currentFrequency = 0;
			var scanningFrequencies = true;
			var firstRepeatingFrequency = 0;

			while (scanningFrequencies)
			{

				foreach (var item in input)
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
