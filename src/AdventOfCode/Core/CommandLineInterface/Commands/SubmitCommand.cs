using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace AdventOfCode.Core.CommandLineInterface.Commands;

internal sealed class SubmitSettings : HttpCommandSettings
{
	[CommandOption("-p|--part <PART>")]
	[Description("Which part should be submitted as an answer.")]
	public int Part { get; set; }
}

internal sealed class SubmitCommand : AsyncCommand<SubmitSettings>
{
	private readonly AoCSettings runnerSettings;
	private readonly IHttpClientFactory httpClientFactory;
	private readonly AoCRunner runner;

	public SubmitCommand(
		AoCSettings runnerSettings,
		IHttpClientFactory httpClientFactory,
		AoCRunner runner)
	{
		this.runnerSettings = runnerSettings;
		this.httpClientFactory = httpClientFactory;
		this.runner = runner;
	}

	public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] SubmitSettings settings)
	{
		if (!AoCHelper.ValidatePersonalTokenIsProvided(runnerSettings.PersonalToken))
		{
			return 0;
		}

		var filePath = $@"Data\{settings.Year}\Day{settings.Day:D2}.txt";
		if (!File.Exists(filePath))
		{
			AnsiConsole.MarkupLine($"[yellow]Data file does not exist for Year {settings.Year} Day {settings.Day}.\n" +
				$"Please create this file first, using the [blue]add[/] command.[/]");
			return 0;
		}
		else
		{
			var fileInfo = new FileInfo(filePath);
			if (fileInfo.Length < 10
				&& File.ReadAllText(filePath).Length == 0)
			{
				AnsiConsole.MarkupLine($"[yellow]Data file has no content for Year {settings.Year} Day {settings.Day}.\n" +
					$"Please add input data manually or use the [blue]input[/] command.[/]");

				return 0;
			}
		}

		AoCSettings submitSettings = runnerSettings;
		runnerSettings.UseExampleData = false;
		runnerSettings.YearToRun = settings.Year;
		runnerSettings.DaysToRun = new[] { settings.Day };
		runnerSettings.RunPartOne = settings.Part == 1;
		runnerSettings.RunPartTwo = settings.Part == 2;

		DayResult dayResult = runner.Run(submitSettings).First();

		var answer = runnerSettings.RunPartOne ?
				dayResult.PartOneResults.First().Answer :
				dayResult.PartTwoResults.First().Answer;

		AnsiConsole.MarkupLine("[yellow]Submitting input data...[/]");
		HttpClient client = httpClientFactory.CreateClient();
		var submitEndpoint = $"{settings.BaseUrl}/{settings.Year}/day/{settings.Day}/answer";
		client.DefaultRequestHeaders.Add("cookie", $"session={runnerSettings.PersonalToken}");
		client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", settings.UserAgent);

		var request = new HttpRequestMessage(HttpMethod.Post, submitEndpoint);
		var requestContent = new List<string>()
		{
			$"level={settings.Part}",
			$"answer={answer}"
		};
		request.Content = new StringContent(string.Join("&", requestContent));
		request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.FormUrlEncoded);

		var response = await client.SendAsync(request);

		var responseContent = await response.Content.ReadAsStringAsync();

		if (response.StatusCode == HttpStatusCode.NotFound)
		{
			if (responseContent.Contains("don't repeatedly request this endpoint before it unlocks"))
			{
				AnsiConsole.MarkupLine("[red]The day's puzzle you have submitted is not available yet. " +
					"Please do not submit until the day's puzzle has started.[/]");
			}
			else
			{
				AnsiConsole.MarkupLine("[red]The day's puzzle you have submitted is not found.[/]");
			}
		}
		else if (responseContent.Contains("You don't seem to be solving"))
		{
			AnsiConsole.MarkupLine("[red]You don't seem to be solving the right level. Did you already complete it?[/]");
		}
		else if ((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
		{
			AnsiConsole.MarkupLine("[red]The server indicated there was an error. " +
				"This may indicate you have submitted an invalid Personal Token[/]");
		}
		else if (responseContent.Contains("That's the right answer"))
		{
			AnsiConsole.MarkupLine("[green]Congratulations, you got the correct answer![/]");
		}
		else if (responseContent.Contains("That's not the right answer"))
		{
			var regex = new Regex("(please wait ).*( before trying again.)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

			var timeToWaitMessage = regex.Match(responseContent).Groups[0].Value;

			AnsiConsole.MarkupLine($"[red]The answer you have submitted is incorrect... {timeToWaitMessage}[/]");
		}
		else if (responseContent.Contains("You gave an answer too recently"))
		{
			var regex = new Regex("(You have ).*( left to wait.)");

			var timeToWaitMessage = regex.Match(responseContent).Groups[0].Value;

			AnsiConsole.MarkupLine($"[red]You gave an answer too recently. {timeToWaitMessage}[/]");
		}
		else
		{
			AnsiConsole.MarkupLine($"[red]Received an unknown response from the server.[/]");
		}

		return 0;
	}

	public override ValidationResult Validate(CommandContext context, SubmitSettings settings)
	{
		if (settings.Part < 1 || settings.Part > 2)
		{
			return ValidationResult.Error("Part must be 1 or 2.");
		}

		return AoCHelper.ValidateDateNotTooEarly(settings);
	}
}
