using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Core.CommandLineInterface.Commands
{
	public sealed class InputSettings : HttpCommandSettings
	{
		[CommandOption("-y|--year <YEAR>")]
		[Description("Which year to get input for.")]
		public int Year { get; set; }

		[CommandOption("-d|--day <DAY>")]
		[Description("Which day to get input for.")]
		public int Day { get; set; }
	}

	public sealed class InputCommand : AsyncCommand<InputSettings>
	{
		private readonly AoCSettings runnerSettings;
		private readonly IHttpClientFactory httpClientFactory;

		public InputCommand(AoCSettings runnerSettings, IHttpClientFactory httpClientFactory)
		{
			this.runnerSettings = runnerSettings;
			this.httpClientFactory = httpClientFactory;
		}

		public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] InputSettings settings)
		{
			if (runnerSettings.PersonalToken == "<DO_NOT_COMMIT_ME>")
			{
				AnsiConsole.MarkupLine("AocSettings.PersonalToken is not set. Use User Secrets or Environment Variables to set this.\n" +
					"[red]BE CAREFUL YOU ARE NOT COMMITTING ANY SECRETS AND [yellow]DO NOT ADD[/] THIS VALUE TO [yellow]appsettings.json[/][/]");
				return 0;
			}

			var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
			var filePath = Path.Join(projectDirectory, $@"Data\{settings.Year}\Day{settings.Day:D2}.txt");
			if (!File.Exists(filePath))
			{
				AnsiConsole.MarkupLine($"[yellow]Data file does not exist for Year {settings.Year} Day {settings.Day}.\n" +
					$"Please create this file first, using the [blue]add[/] command.[/]");
				return 0;
			}

			AnsiConsole.MarkupLine("[yellow]Downloading input data...[/]");
			HttpClient client = httpClientFactory.CreateClient();
			var inputEndpoint = $"{settings.BaseUrl}/{settings.Year}/day/{settings.Day}/input";
			client.DefaultRequestHeaders.Add("cookie", $"session={runnerSettings.PersonalToken}");
			client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", settings.UserAgent);

			var response = await client.GetAsync(inputEndpoint);

			if (response.IsSuccessStatusCode)
			{
				var inputData = await response.Content.ReadAsStringAsync();

				File.WriteAllText(filePath, inputData);

				AnsiConsole.MarkupLine("[yellow]Downloading input data...[/]");
			}
			else
			{
				AnsiConsole.MarkupLine($"[red]Could not complete request: {response.ReasonPhrase}[/]");
			}

			return 0;
		}

		public override ValidationResult Validate(CommandContext context, InputSettings settings)
		{
			bool isBadRequest = settings.Year == 0 || settings.Day == 0;
			if (isBadRequest)
			{
				return ValidationResult.Error("Not enough information provided. Please provide valid Year and Day options.");
			}

			bool isYearOutOfRange = settings.Year < 2015 || settings.Year > DateTime.Now.Year;
			if (isYearOutOfRange)
			{
				return ValidationResult.Error("The year you have specified is outside the range of existing Advent of Code events.");
			}

			bool isTooEarly = settings.Year == DateTime.Now.Year && DateTime.Now.Month < 12;
			if (isTooEarly)
			{
				var date = DateOnly.FromDateTime(DateTime.Now);

				if (date.Month < 10)
				{
					return ValidationResult.Error("It's not December! You'll have to be patient.");
				}

				var isBeforeHalloween = date.Month == 10 && date.Day < 31;
				if (isBeforeHalloween)
				{
					return ValidationResult.Error("It's not December! You haven't even carved a pumpkin yet!");
				}

				if (date.Month == 11)
				{
					DateOnly novemberFirst = new(date.Year, 11, 1);
					int daysUntilThursday = ((int)DayOfWeek.Thursday - (int)novemberFirst.DayOfWeek + 7) % 7;
					DateOnly thanksgivingDay = novemberFirst.AddDays(21 + daysUntilThursday);

					var isBeforeThanksgiving = date.Day < thanksgivingDay.Day;
					if (isBeforeThanksgiving)
					{
						return ValidationResult.Error("It's not December! You haven't even carved a turkey yet!");
					}
				}
			}

			bool isDaysOutOfRange = settings.Day < 1 || settings.Day > 25;
			if (isDaysOutOfRange)
			{
				return ValidationResult.Error("Days must be within range of 1-25.");
			}

			else return ValidationResult.Success();
		}
	}
}
