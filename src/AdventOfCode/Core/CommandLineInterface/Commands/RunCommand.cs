﻿using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode.Core.CommandLineInterface.Commands
{
	public sealed class RunSettings : CommandSettings
	{
		[CommandOption("-a|--all <ALL>")]
		[DefaultValue(false)]
		public bool All { get; set; }

		[CommandOption("-r|--real <REAL>")]
		[DefaultValue(false)]
		public bool Real { get; set; }

		[CommandOption("-d|--day <DAY>")]
		public string[]? Day { get; set; }

		[CommandOption("-p|--part <PART>")]
		public string? Part { get; set; }

		[CommandOption("-y|--year <YEAR>")]
		public int Year { get; set; }
	}

	public sealed class RunCommand : Command<RunSettings>
	{
		private readonly AoCRunner runner;
		private readonly AoCSettings runnerSettings;
		private RunSettings runSettings = null!;

		public RunCommand(AoCRunner runner, AoCSettings runnerSettings)
		{
			this.runner = runner;
			this.runnerSettings = runnerSettings;
		}

		public override int Execute([NotNull] CommandContext context, [NotNull] RunSettings settings)
		{
			runSettings = settings;

			if (settings.All)
			{
				RunAllDays();
			}
			else if (settings.Day?.Length > 0)
			{
				RunSelectedDays(settings.Day);
			}
			else
			{
				RunWithConfiguration();
			}

			return 0;
		}

		private void RunAllDays()
		{
			ConfigureSettings(runAllDays: true);

			AnsiConsole.MarkupLine("[yellow]Running All Days[/]");

			runner.Run(runnerSettings);
		}

		private void RunSelectedDays(string[] daysToRun)
		{
			ConfigureSettings(runAllDays: false);

			AnsiConsole.MarkupLine($"[yellow]Running Selected Days[/]: {string.Join(", ", daysToRun)}");

			runner.Run(runnerSettings);
		}

		private void ConfigureSettings(bool runAllDays)
		{
			var (runPartOne, runPartTwo) = ProcessPartsInput(runSettings.Part ?? string.Empty);
			runnerSettings.RunAllDays = runAllDays;
			runnerSettings.DaysToRun = ProcessDaysInput(runSettings.Day);
			runnerSettings.RunPartOne = runPartOne;
			runnerSettings.RunPartTwo = runPartTwo;
			runnerSettings.UseExampleData = !runSettings.Real;
			runnerSettings.YearToRun = runSettings.Year > 0 ? runSettings.Year : runnerSettings.YearToRun;

			if (runSettings.Real)
			{
				AnsiConsole.MarkupLine($"[springgreen3]Using Real Data[/]");
			}
			else
			{
				AnsiConsole.MarkupLine($"[deepskyblue3_1]Using Example Data[/]");
			}
		}

		private static (bool RunPartOne, bool RunPartTwo) ProcessPartsInput(string part)
		{
			var regex = new Regex(@"([(one)(two)12])$", RegexOptions.IgnoreCase);

			var match = regex.Match(part);

			if (match.Success)
			{
				return match.Value.ToLowerInvariant() switch
				{
					"one" => (true, false),
					"1" => (true, false),
					"two" => (false, true),
					"2" => (false, true),
					_ => (true, true)
				};
			}

			return (true, true);
		}

		private static IEnumerable<string> ProcessDaysInput(string[]? daysToRun)
		{
			if (daysToRun is null || daysToRun.Length == 0)
			{
				return Enumerable.Empty<string>();
			}

			var regex = new Regex(@"\d{1,2}$");
			List<string> processedDays = new();
			foreach (var dayInput in daysToRun)
			{
				var match = regex.Match(dayInput);

				int day = 0;
				var isValidDay = match.Success
					&& int.TryParse(match.Value, out day)
					&& Enumerable.Range(1, 25).Contains(day);

				if (isValidDay)
				{
					processedDays.Add($"Day{day:D2}");
				}
			}

			return processedDays;
		}

		private void RunWithConfiguration()
		{
			Table settingsTable = new Table()
					.AddColumn("Setting")
					.AddColumn("Value", options => { options.Width = 30; })
					.AddRow("Year To Run", $"{runnerSettings.YearToRun}")
					.AddRow("Use Example Data", $"{runnerSettings.UseExampleData}")
					.AddRow("Run All Days",
						$"{runnerSettings.RunAllDays} " +
						$"{(runnerSettings.RunAllDays ? "\n\n[yellow2](Overriding Run Days)[/]\n" : string.Empty)}")
					.AddRow("Run Part One", $"{runnerSettings.RunPartOne}")
					.AddRow("Run Part Two", $"{runnerSettings.RunPartTwo}")
					.AddRow("Run Days",
						$"{string.Join(", ", runnerSettings.DaysToRun)} " +
						$"{(runnerSettings.RunAllDays ? "\n\n[red](Overridden by Run All Days)[/]" : string.Empty)}")
					.Border(TableBorder.Rounded)
					.BorderColor(Color.Teal)
					.RoundedBorder()
					.LeftAligned()
					.Collapse();

			AnsiConsole.Write(settingsTable);

			if (!AnsiConsole.Confirm("Not enough parameters provided. Run with application configuration instead?"))
			{
				AnsiConsole.MarkupLine("[red]Aborting...[/]");
			}
			else
			{
				AnsiConsole.MarkupLine("[yellow]Running With Application Configuration[/]");

				runner.Run();
			}
		}
	}
}
