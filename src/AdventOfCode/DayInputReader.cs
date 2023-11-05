using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace AdventOfCode
{
	public static class DayInputReader
	{
		private static readonly IConfiguration configuration = Program.Configuration;

		public const string PartOneDelimiter = "[part1]";
		public const string PartTwoDelimiter = "[part2]";

		// We're returning a tuple to distinguish between Part 1 and Part 2 example data
		public static (IEnumerable<IEnumerable<string>> PartOne, IEnumerable<IEnumerable<string>> PartTwo) GetData()
		{
			bool useExampleData = bool.Parse(configuration["RunnerSettings:UseExampleData"]!);
			string yearToRun = configuration["RunnerSettings:YearToRun"]!;
			var exampleData = useExampleData ? "Example" : string.Empty;
			string[] exampleLineDelimiters = new[] { PartOneDelimiter, PartTwoDelimiter };

			// We need to use StackTrace to get our expected caller of the caller to this data.
			// Was originally using [CallerFilePath] but since we moved the call to base class it no longer worked.
			var stackTrace = new StackTrace();
			string callerTypeName = stackTrace.GetFrame(2)!.GetMethod()!.DeclaringType!.Name;

			var filePath = $"Data\\{yearToRun}\\{callerTypeName}{exampleData}.txt";
			List<string> inputData = File.ReadLines(filePath).ToList();

			if (inputData.Count == 0)
			{
				throw new AdventOfCodeException($"There is no {exampleData} Data provided to read from.");
			}

			var partOneReturnData = new List<List<string>>();
			var partTwoReturnData = new List<List<string>>();

			// If we're using example data, we might have multiple sets, separated by delimiter lines
			if (useExampleData)
			{
				var currentExampleDataSet = new List<string>();
				// Assume the first part is for Part 1 since it makes no sense to not
				// since the first set of example data in AoC will be for part 1
				var currentPart = PartOneDelimiter;

				foreach (var line in inputData)
				{
					// Check the current line to see if it's one of our expected delimiters
					if (exampleLineDelimiters.Contains(line.ToLowerInvariant()))
					{
						// Only if we have any data do we need to add and reset it
						if (currentExampleDataSet.Any())
						{
							switch (currentPart)
							{
								case PartOneDelimiter:
									partOneReturnData.Add(currentExampleDataSet);
									break;
								case PartTwoDelimiter:
									partTwoReturnData.Add(currentExampleDataSet);
									break;
								default: throw new AdventOfCodeException($"Invalid part delimiter. ({line})");
							}

							currentExampleDataSet = new List<string>();

							currentPart = line;
						}
					}
					else
					{
						// No delimiter, so we add it to the current example data set
						currentExampleDataSet.Add(line);
					}
				}

				// We've reached the end of the file, add our last data sets
				if (currentExampleDataSet.Any())
				{
					if (currentPart == PartOneDelimiter)
					{
						partOneReturnData.Add(currentExampleDataSet);
					}
					else
					{
						partTwoReturnData.Add(currentExampleDataSet);
					}
				}
			}
			else
			{
				// Add to both parts. Not optimized, we can change this later if needed
				partOneReturnData.Add(inputData);
				partTwoReturnData.Add(inputData);
			}

			return (partOneReturnData, partTwoReturnData);
		}
	}
}
