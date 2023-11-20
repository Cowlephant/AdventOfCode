using AdventOfCode.Core;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;

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

		public IEnumerable<DayResult> Run(AoCSettings? settings = null)
		{
			// If passed in settings, use those instead of the ones configured at startup
			if (settings is not null)
			{
				this.settings = settings;
			}

			IEnumerable<DayResult> results = RunDays();
			resultsDisplay.Display(results);

			return RunDays();
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
						$"Day {day.GetType().GetCustomAttribute<AoCYearDayAttribute>()!.Day}",
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
						$"Day {day.GetType().GetCustomAttribute<AoCYearDayAttribute>()!.Day}",
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
			List<PartResult> partOneResults = [];
			List<PartResult> partTwoResults = [];

			var day = daySolver.GetType().GetCustomAttribute<AoCYearDayAttribute>()!.Day;
			string dayName = $"Day{day:D2}";
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

		private List<IAoCDaySolver> GetAllDays(IEnumerable<int>? filteredDays = null)
		{
			filteredDays ??= Enumerable.Empty<int>();
			var daySolverType = typeof(IAoCDaySolver);
			List<IAoCDaySolver> daysToRun = [];

			var allFilteredDays = daySolverType.Assembly.GetTypes()
				.Where(type => daySolverType.IsAssignableFrom(type)
				&& type.CustomAttributes.Any(a => a.AttributeType == typeof(AoCYearDayAttribute))
				&& type.GetCustomAttribute<AoCYearDayAttribute>()!.Year == settings.YearToRun
				&& filteredDays.Contains(type.GetCustomAttribute<AoCYearDayAttribute>()!.Day)
				&& !type.IsAbstract);


			foreach (var day in allFilteredDays)
			{
				var dayToRun = (IAoCDaySolver)Activator.CreateInstance(day)!;

				daysToRun.Add(dayToRun);
			}

			return daysToRun;
		}
	}
}
