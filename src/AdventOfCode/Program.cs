using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
	static internal partial class Program
	{
		public static IConfiguration Configuration { get; private set; } = null!;
		public static RunnerSettings RunnerSettings { get; private set; } = null!;

		private const int dividerLengthTop = 30;
		private const int dividerLengthBottom = 46;

		static void Main()
		{
			Configuration = new ConfigurationBuilder()
			   .AddJsonFile("appsettings.json")
			   .Build();

			RunnerSettings = ConfigurationBinder.Get<RunnerSettings>(Configuration.GetSection(nameof(RunnerSettings)))!;

			var withExampleData = RunnerSettings.UseExampleData ? " With Example Data" : "";

			if (RunnerSettings.RunAllDays)
			{
				Console.WriteLine($"Running All Days{withExampleData}");

				foreach (var day in GetAllDays())
				{
					RunParts(day);
				}
			}
			else if (RunnerSettings.DaysToRun.Any())
			{
				var daysToRun = string.Join(", ", RunnerSettings.DaysToRun);
				Console.WriteLine($"Running Specific Days{withExampleData}: {daysToRun}");

				foreach (var day in GetAllDays(RunnerSettings.DaysToRun))
				{
					RunParts(day);
				}
			}
			else
			{
				throw new AdventOfCodeException("No days configured to run.");
			}
		}

		private static void RunParts(IAdventOfCodeRunner dayRunner)
		{
			var dayNumber = int.Parse(dayRunner.GetType().Name!.Replace("Day", ""));

			Console.Write($"Running Day {dayNumber}\t");
			Console.WriteLine(new string('-', dividerLengthTop));

			if (RunnerSettings.RunPartOne)
			{
				var answer = dayRunner.RunPartOne();
				string expectedAnswerResult = ValidateExpectedAnswer(dayRunner, "RunPartOne", answer);

				Console.WriteLine($"\tPart 1 Answer: {expectedAnswerResult}{answer}");
			}
			if (RunnerSettings.RunPartTwo)
			{
				var answer = dayRunner.RunPartTwo();
				string expectedAnswerResult = ValidateExpectedAnswer(dayRunner, "RunPartTwo", answer);

				Console.WriteLine($"\tPart 2 Answer: {expectedAnswerResult}{answer}");
			}
			
			Console.WriteLine(new string('-', dividerLengthBottom));
			Console.WriteLine();

			static string ValidateExpectedAnswer(IAdventOfCodeRunner dayRunner, string partName, string answer)
			{
				var expectedAnswer = dayRunner.GetType().GetMethod(partName)!
					.GetCustomAttribute<ExpectedExampleAnswerAttribute>()!.ExpectedExampleAnswer;

				bool isExpectedAnswerCorrect = answer == expectedAnswer;
				string expectedAnswerResult;
				if (answer != "Not Implemented" && RunnerSettings.UseExampleData)
				{
					expectedAnswerResult = isExpectedAnswerCorrect ? "(CORRECT)\t" : "(INCORRECT)\t";
				}
				else
				{
					expectedAnswerResult = string.Empty;
				}

				return expectedAnswerResult;
			}
		}

		private static IEnumerable<IAdventOfCodeRunner> GetAllDays(IEnumerable<string>? filteredDays = null)
		{
			var adventOfCodeRunnerType = typeof(IAdventOfCodeRunner);

			var allDays = adventOfCodeRunnerType.Assembly.GetTypes()
				.Where(type => adventOfCodeRunnerType.IsAssignableFrom(type)
				&& type.CustomAttributes.Any(a => a.AttributeType == typeof(AdventOfCodeYearAttribute))
				&& type.GetCustomAttribute<AdventOfCodeYearAttribute>()!.Year == RunnerSettings.YearToRun
				&& !type.IsAbstract);

			foreach (var day in allDays.Select(d => d.Name))
			{
				var className = day;
				var namingPattern = DayPatternRegex();

				if (!namingPattern.IsMatch(className))
				{
					throw new AdventOfCodeException($"Day class name does not match required pattern Day##: {className}");
				}
			}

			List<IAdventOfCodeRunner> daysToRun = new();

			if (filteredDays == null)
			{
				foreach (var day in allDays)
				{
					var dayToRun = (IAdventOfCodeRunner)Activator.CreateInstance(day)!;
					daysToRun.Add(dayToRun);
				}
			}
			else
			{
				var selectedDays = allDays.Where(day =>
					filteredDays.Any(d =>
						d.Equals(day.Name, StringComparison.CurrentCultureIgnoreCase)));

				foreach (var day in selectedDays)
				{
					var dayToRun = (IAdventOfCodeRunner)Activator.CreateInstance(day)!;

					daysToRun.Add(dayToRun);
				}
			}

			return daysToRun;
		}

		[GeneratedRegex(@"^Day[012]\d")]
		private static partial Regex DayPatternRegex();
	}
}