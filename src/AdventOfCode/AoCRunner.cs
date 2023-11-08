using AdventOfCode.Core;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
	internal sealed class AoCRunner
	{
		private readonly AoCSettings settings;
		private readonly AoCInputReader inputReader;
		private readonly AoCResultsDisplay resultsDisplay;
		private readonly Stopwatch stopwatch;

		public AoCRunner(IConfiguration configuration, AoCInputReader inputReader, AoCResultsDisplay resultsDisplay)
		{
			settings = configuration.GetSection(nameof(AoCSettings)).Get<AoCSettings>()!;
			this.inputReader = inputReader;
			this.resultsDisplay = resultsDisplay;
			stopwatch = new Stopwatch();
		}

		public void Run()
		{
			IEnumerable<DayResult> results = RunDays();
			resultsDisplay.Display(results);
		}

		private IEnumerable<DayResult> RunDays()
		{
			var dayResults = new List<DayResult>();

			if (settings.RunAllDays)
			{
				foreach (var day in GetAllDays())
				{
					var (partOneResults, partTwoResults) = RunParts(day);
					var dayResult = new DayResult(
						day.GetType().Name,
						settings.YearToRun,
						settings.UseExampleData,
						partOneResults,
						partTwoResults);
					dayResults.Add(dayResult);
				}
			}
			else if (settings.DaysToRun.Any())
			{
				foreach (var day in GetAllDays(settings.DaysToRun))
				{
					var (partOneResults, partTwoResults) = RunParts(day);
					var dayResult = new DayResult(
						day.GetType().Name,
						settings.YearToRun,
						settings.UseExampleData,
						partOneResults,
						partTwoResults);
					dayResults.Add(dayResult);
				}
			}
			else
			{
				throw new AoCException("No days configured to run.");
			}

			return dayResults;
		}

		private (IEnumerable<PartResult> PartOneResults, IEnumerable<PartResult> PartTwoResults)
			RunParts(IAoCDaySolver daySolver)
		{
			List<PartResult> partOneResults = new List<PartResult>();
			List<PartResult> partTwoResults = new List<PartResult>();

			string dayName = daySolver.GetType().Name;
			var (PartOneData, PartTwoData) = inputReader.GetData(dayName);

			var partOneExpectedAnswers = GetExpectedAnswers(daySolver.GetType(), "SolvePartOne");
			var partTwoExpectedAnswers = GetExpectedAnswers(daySolver.GetType(), "SolvePartTwo");

			if (settings.RunPartOne)
			{
				foreach (var (partOneData, index) in PartOneData.Select((d, i) => (d, i)))
				{
					stopwatch.Start();
					var answer = daySolver.SolvePartOne(partOneData);
					stopwatch.Stop();

					var expectedAnswer = partOneExpectedAnswers.ElementAtOrDefault(index) ?? "?";
					var durationMicroseconds = stopwatch.ElapsedTicks / (TimeSpan.TicksPerMillisecond / 1000); ;
					string durationFriendly = durationMicroseconds > 1000 ? $"{durationMicroseconds / 1000}ms" : $"{durationMicroseconds}μs";

					var partResult = new PartResult(answer, expectedAnswer, answer == expectedAnswer, durationFriendly);
					partOneResults.Add(partResult);
				}

				stopwatch.Reset();
			}
			if (settings.RunPartTwo)
			{
				foreach (var (partTwoData, index) in PartTwoData.Select((d, i) => (d, i)))
				{
					stopwatch.Start();
					var answer = daySolver.SolvePartTwo(partTwoData);
					stopwatch.Stop();

					var expectedAnswer = partTwoExpectedAnswers.ElementAtOrDefault(index) ?? "?";
					var duration = stopwatch.ElapsedTicks;
					string durationFriendly = duration > 1000 ? $"{duration / 1000}ms" : $"{duration}μs";

					var partResult = new PartResult(answer, expectedAnswer, answer == expectedAnswer, durationFriendly);
					partTwoResults.Add(partResult);
				}
			}

			stopwatch.Reset();

			return (partOneResults, partTwoResults);
		}

		private IEnumerable<string> GetExpectedAnswers(Type dayType, string partName)
		{
			var expectedAnswers = dayType.GetMethod(partName)!
				.GetCustomAttributes<AoCExpectedExampleAnswersAttribute>()!
					.SelectMany(a => a.ExpectedExampleAnswers)
					.ToList();

			return expectedAnswers;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "Performance optimization not necessary")]
		private List<IAoCDaySolver> GetAllDays(IEnumerable<string>? filteredDays = null)
		{
			var daySolverType = typeof(IAoCDaySolver);
			List<IAoCDaySolver> daysToRun = new();

			var allDays = daySolverType.Assembly.GetTypes()
				.Where(type => daySolverType.IsAssignableFrom(type)
				&& type.CustomAttributes.Any(a => a.AttributeType == typeof(AoCYearAttribute))
				&& type.GetCustomAttribute<AoCYearAttribute>()!.Year == settings.YearToRun
				&& !type.IsAbstract);

			foreach (var day in allDays.Select(d => d.Name))
			{
				var className = day;
				var namingPattern = new Regex(@"^Day[012]\d");

				if (!namingPattern.IsMatch(className))
				{
					throw new AoCException($"Day class name does not match required pattern Day##: {className}");
				}
			}

			if (filteredDays == null)
			{
				foreach (var day in allDays)
				{
					var dayToRun = (IAoCDaySolver)Activator.CreateInstance(day)!;
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
					var dayToRun = (IAoCDaySolver)Activator.CreateInstance(day, inputReader)!;

					daysToRun.Add(dayToRun);
				}
			}

			return daysToRun;
		}
	}
}
