using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace AdventOfCode
{
	public static class DayInputReader
	{
		private static readonly IConfiguration configuration = Program.Configuration;

		public static List<string> GetData([CallerFilePath] string callerFilePath = "")
		{
			bool useExampledata = bool.Parse(configuration["RunnerSettings:UseExampleData"]!);
			string yearToRun = configuration["RunnerSettings:YearToRun"]!;
			var exampleData = useExampledata ? "Example" : string.Empty;

			var callerTypeName = Path.GetFileNameWithoutExtension(callerFilePath);
			var filePath = $"Data\\{yearToRun}\\{callerTypeName}{exampleData}.txt";
			var inputData = File.ReadLines(filePath).ToList();

			if (inputData.Count == 0)
			{
				throw new AdventOfCodeException($"There is no {exampleData} Data provided to read.");
			}

			return inputData;
		}
	}
}
