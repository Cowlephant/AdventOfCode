using AdventOfCode.Core;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
	public sealed class AoCRunner
	{
		private AoCSettings settings;
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

		public void Run(AoCSettings? settings = null)
		{
			// If passed in settings, use those instead of the ones configured at startup
			if (settings is not null)
			{
				this.settings = settings;
			}

			IEnumerable<DayResult> results = RunDays();
			resultsDisplay.Display(results);
		}

		private List<DayResult> RunDays()
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
			List<PartResult> partOneResults = new();
			List<PartResult> partTwoResults = new();

			string dayName = daySolver.GetType().Name;
			var (PartOneData, PartTwoData) = inputReader.GetData(dayName);

			var partOneExpectedAnswers = GetExpectedAnswers(daySolver.GetType(), "SolvePartOne");
			var partTwoExpectedAnswers = GetExpectedAnswers(daySolver.GetType(), "SolvePartTwo");

			if (settings.RunPartOne)
			{
				RunPart(daySolver, PartOneData, partOneResults, partOneExpectedAnswers, isPartOne: true);
				stopwatch.Reset();
			}
			if (settings.RunPartTwo)
			{
				RunPart(daySolver, PartTwoData, partTwoResults, partTwoExpectedAnswers, isPartOne: false);
				stopwatch.Reset();
			}

			stopwatch.Reset();

			return (partOneResults, partTwoResults);
		}

		private void RunPart(
			IAoCDaySolver daySolver,
			IEnumerable<IEnumerable<string>> partData,
			List<PartResult> partResults,
			IEnumerable<string> expectedAnswers,
			bool isPartOne)
		{
			foreach (var (partDatum, index) in partData.Select((d, i) => (d, i)))
			{
				stopwatch.Start();
				var answer = isPartOne ? daySolver.SolvePartOne(partDatum.ToList())
									   : daySolver.SolvePartTwo(partDatum.ToList());
				stopwatch.Stop();

				var expectedAnswer = expectedAnswers.ElementAtOrDefault(index) ?? "?";
				var duration = stopwatch.ElapsedTicks;
				string durationFriendly = duration > 1000 ? $"{duration / 1000}ms" : $"{duration}μs";

				var partResult = new PartResult(answer, expectedAnswer, answer == expectedAnswer, durationFriendly);
				partResults.Add(partResult);
			}
		}

		private static List<string> GetExpectedAnswers(Type dayType, string partName)
		{
			var expectedAnswers = dayType.GetMethod(partName)!
				.GetCustomAttributes<AoCExpectedExampleAnswersAttribute>()!
					.SelectMany(a => a.ExpectedExampleAnswers)
					.ToList();

			return expectedAnswers;
		}

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
					var dayToRun = (IAoCDaySolver)Activator.CreateInstance(day)!;

					daysToRun.Add(dayToRun);
				}
			}

			return daysToRun;
		}
	}
}
