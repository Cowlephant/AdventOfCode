﻿using AdventOfCode.Core;

namespace AdventOfCode.Solution
{
	[AoCYearDay(2018, 1)]
	public sealed class Day01Year2018 : IAoCDaySolver
	{
		[AoCExpectedExampleAnswers("3", "0", "-6")]
		public string SolvePartOne(List<string> input)
		{
			var finalFrequency = 0;

			foreach (var item in input)
			{
				var frequency = int.Parse(item);
				finalFrequency += frequency;
			}

			return finalFrequency.ToString();
		}

		[AoCExpectedExampleAnswers("0", "10", "5", "14")]
		public string SolvePartTwo(List<string> input)
		{
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