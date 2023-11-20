using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Core.CommandLineInterface.Commands
{
	public sealed class AddDaySettings : CommandSettings
	{
		[CommandOption("-d|--day <DAY>")]
		[Description("The day or days for which to create the files for.")]
		public int[] Day { get; set; } = null!;

		[CommandOption("-y|--year <YEAR>")]
		[DefaultValue(0)]
		[Description("The year to create the files for. If not provided, uses value from configuration instead.")]
		public int Year { get; set; }
	}

	public sealed class AddDayCommand : Command<AddDaySettings>
	{
		private readonly AoCSettings runnerSettings;
		private readonly string solutionPath;
		private readonly string dataPath;

		public AddDayCommand(AoCSettings runnerSettings)
		{
			this.runnerSettings = runnerSettings;
			var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
			solutionPath = $"{Path.Combine(projectDirectory, $@"{runnerSettings.SolutionFolderPath}")}";
			dataPath = $"{Path.Combine(projectDirectory, $@"{runnerSettings.DataFolderPath}")}";
		}

		[SuppressMessage("Minor Code Smell", "S6605:Collection-specific \"Exists\" method should be used instead of the \"Any\" extension", Justification = "Not applicable")]
		public override int Execute([NotNull] CommandContext context, [NotNull] AddDaySettings settings)
		{
			if (settings.Day.Any(d => d < 1 || d > 25))
			{
				AnsiConsole.MarkupLine("[yellow]Days must be within range of 1-25[/]");
				return 0;
			}

			int year = settings.Year == 0 ? runnerSettings.YearToRun : settings.Year;

			foreach (var day in settings.Day)
			{

				string rewritePrompt = $"Files already exist for Year {year} Day {settings.Day}. " +
					$"ARE YOU SURE YOU WISH TO [red]OVERWRITE ALL CONTENTS[/]?";

				if (DoAnyFilesExist(year, day) &&
					!AnsiConsole.Confirm(rewritePrompt, defaultValue: false))
				{
					AnsiConsole.MarkupLine($"[yellow]Skipping Year {year} Day {settings.Day}[/]");
					continue;
				}

				AnsiConsole.MarkupLine($@"Generating solution file in{'\t'} {solutionPath}\{year}");
				AnsiConsole.MarkupLine($@"Generating data files in{'\t'} {dataPath}\{year}");
				CreateInputFiles(year, day);
			}

			return 0;
		}

		private bool DoAnyFilesExist(int year, int day)
		{
			var dayName = $"Day{day:D2}";
			var filePaths = new List<string>
			{
				$@"{solutionPath}\{year}\{dayName}Year{year}.cs",
				$@"{dataPath}\{year}\{dayName}.txt",
				$@"{dataPath}\{year}\{dayName}Example.txt"
			};

			if (filePaths.Exists(f => File.Exists(f)))
			{
				return true;
			}

			return false;
		}

		private void CreateInputFiles(int year, int day)
		{
			var solutionYearPath = $@"{solutionPath}\{year}";
			var dataYearPath = $@"{dataPath}\{year}";
			var dayName = $"Day{day:D2}";
			var exampleData = $$"""
                [part1]
                REPLACE ME WITH EXAMPLE DATA FOR PART 1 OF {{dayName}}. I CAN EVEN HAVE MULTIPLES OF THE SAME PART FOR MULTIPLE SETS OF DATA
                [part2]
                REPLACE ME WITH EXAMPLE DATA FOR PART 2 OF {{dayName}}
                """;


			if (!Directory.Exists(solutionYearPath))
			{
				Directory.CreateDirectory(solutionYearPath);
			}
			if (!Directory.Exists(dataYearPath))
			{
				Directory.CreateDirectory(dataYearPath);
			}

			File.WriteAllText($@"{solutionPath}\{year}\{dayName}Year{year}.cs", GenerateDayTemplate(year, day));
			File.WriteAllText($@"{dataPath}\{year}\{dayName}.txt", null);
			File.WriteAllText($@"{dataPath}\{year}\{dayName}Example.txt", exampleData);
		}


		private static string GenerateDayTemplate(int year, int day)
		{
			var className = $"Day{day:D2}Year{year}";
			var dayTemplate = $$"""
            using AdventOfCode.Core;

            namespace AdventOfCode.Solution
            {
            	[AoCYearDay({{year}}, {{day}})]
            	public sealed class {{className}} : IAoCDaySolver
            	{
            		[AoCExpectedExampleAnswers("")]
            		public string SolvePartOne(List<string> input)
            		{
                        // Solve

            			return "Not Implemented";
            		}

            		[AoCExpectedExampleAnswers("")]
            		public string SolvePartTwo(List<string> input)
            		{
                        // Solve

            			return "Not Implemented";
            		}
            	}
            }

            """;

			return dayTemplate;
		}
	}
}
