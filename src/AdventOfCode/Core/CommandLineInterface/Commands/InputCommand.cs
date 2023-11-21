using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Core.CommandLineInterface.Commands;

internal sealed class InputCommand : AsyncCommand<HttpCommandSettings>
{
	private readonly AoCSettings runnerSettings;
	private readonly IHttpClientFactory httpClientFactory;

	public InputCommand(AoCSettings runnerSettings, IHttpClientFactory httpClientFactory)
	{
		this.runnerSettings = runnerSettings;
		this.httpClientFactory = httpClientFactory;
	}

	public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] HttpCommandSettings settings)
	{
		if (!AoCHelper.ValidatePersonalTokenIsProvided(runnerSettings.PersonalToken)) 
		{
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

	public override ValidationResult Validate(CommandContext context, HttpCommandSettings settings)
	{
		return AoCHelper.ValidateDateNotTooEarly(settings);
	}
}
