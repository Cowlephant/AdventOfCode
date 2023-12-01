using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;

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
	private readonly string projectDirectory;

	public SubmitCommand(
		AoCSettings runnerSettings,
		IHttpClientFactory httpClientFactory,
		AoCRunner runner)
	{
		this.runnerSettings = runnerSettings;
		this.httpClientFactory = httpClientFactory;
		this.runner = runner;
		projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
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
			AnsiConsole.MarkupLine($"[yellow]Data file does not exist for Year [teal]{settings.Year}[/] Day [teal]{settings.Day}[/].\n" +
				$"Please create this file first, using the [teal]add[/] command.[/]");
			return 0;
		}
		else
		{
			var fileInfo = new FileInfo(filePath);
			if (fileInfo.Length < 10
				&& File.ReadAllText(filePath).Length == 0)
			{
				AnsiConsole.MarkupLine($"[yellow]Data file has no content for Year [teal]{settings.Year}[/] Day [teal]{settings.Day}[/].\n" +
					$"Please add input data manually or use the [teal]input[/] command.[/]");

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

		if (answer == "Not Implemented")
		{
			AnsiConsole.MarkupLine($"[yellow]No code is provided, or you forgot to change the " +
				$"return answer value for Year [teal]{settings.Year}[/] Day [teal]{settings.Day}[/][/].\n");

			return 0;
		}

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

		// Too early for day or not found
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
		// Already solved or didn't solve previous part
		else if (responseContent.Contains("You don't seem to be solving"))
		{
			AnsiConsole.MarkupLine("[red]You don't seem to be solving the right level. Did you already complete it?[/]");
		}
		// Invalid Personal Token or generic server error
		else if ((int)response.StatusCode >= 500 && (int)response.StatusCode <= 599)
		{
			AnsiConsole.MarkupLine("[red]The server indicated there was an error. " +
				"This may indicate you have submitted an invalid Personal Token[/]");
		}
		// Correct answer
		else if (responseContent.Contains("That's the right answer"))
		{
			AnsiConsole.MarkupLine("[green]Congratulations, you got the correct answer![/]");

			var duration = runnerSettings.RunPartOne ?
				dayResult.PartOneResults.First().Duration :
				dayResult.PartTwoResults.First().Duration;
			var durationFriendly = runnerSettings.RunPartOne ?
				dayResult.PartOneResults.First().DurationFriendly :
				dayResult.PartTwoResults.First().DurationFriendly;

			LogAnswer(settings.Year, settings.Day, settings.Part, answer, isAnswerCorrect: true, duration, durationFriendly);
		}
		// Incorrect answer
		else if (responseContent.Contains("That's not the right answer"))
		{
			var regex = new Regex("(please wait ).*( before trying again.)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

			var timeToWaitMessage = regex.Match(responseContent).Groups[0].Value;

			string tooHighTooLowMessage;
			if (responseContent.Contains("your answer is too low"))
			{
				tooHighTooLowMessage = " and is too low";
			}
			else if (responseContent.Contains("your answer is too high"))
			{
				tooHighTooLowMessage = " and is too high";
			}
			else
			{
				tooHighTooLowMessage = "";
			}

			AnsiConsole.MarkupLine($"[red]The answer you have submitted is incorrect{tooHighTooLowMessage}... {timeToWaitMessage}[/]");

			// All information for the correct answer is irrelevant here
			LogAnswer(settings.Year, settings.Day, settings.Part, answer, isAnswerCorrect: false, TimeSpan.Zero, string.Empty);
		}
		// Answered too recently
		else if (responseContent.Contains("You gave an answer too recently"))
		{
			var regex = new Regex("(You have ).*( left to wait.)");

			var timeToWaitMessage = regex.Match(responseContent).Groups[0].Value;

			AnsiConsole.MarkupLine($"[red]You gave an answer too recently. {timeToWaitMessage}[/]");
		}
		// Unknown response
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

	private void LogAnswer(int year, int day, int part, string answer, bool isAnswerCorrect, TimeSpan duration, string durationFriendly)
	{
		if (part < 1 || part > 2)
		{
			throw new AoCException("Part is invalid. Cannot creae data records.");
		}

		var dataModified = false;
		var answerFilePath = Path.Combine($@"{projectDirectory}\Data\{year}\{year}Results.json");
		List<AoCAnswersYear> yearAnswers;

		var serializerOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
		};

		InitializeFile(answerFilePath, serializerOptions, year, day, part, answer, isAnswerCorrect, duration, durationFriendly);

		var answersRaw = File.ReadAllText(answerFilePath);
		yearAnswers = JsonSerializer.Deserialize<List<AoCAnswersYear>>(answersRaw)!;

		var dayAnswer = yearAnswers.First(y => y.Year == year)
			.Days.First(d => d.Day == day);

		AoCAnswersPart partAnswer = part == 1 ? dayAnswer.Part1! : dayAnswer.Part2!;

		partAnswer.IsSolved = isAnswerCorrect;

		if (isAnswerCorrect)
		{
			partAnswer.IsSolved = true;
			partAnswer.CorrectAnswer = answer;
			partAnswer.DurationTicks = duration.Ticks;
			partAnswer.DurationFriendly = durationFriendly;

			dataModified = true;
		}
		else
		{
			// Don't add incorrect answer again if already exists
			if (!partAnswer.IncorrectAnswers.Contains(answer))
			{
				partAnswer.IsSolved = false;
				partAnswer.IncorrectAnswers.Add(answer);

				dataModified = true;
			}
		}

		if (dataModified)
		{
			var fileJson = JsonSerializer.Serialize(yearAnswers, typeof(IEnumerable<AoCAnswersYear>), serializerOptions);

			File.WriteAllText(answerFilePath, fileJson);
		}
	}

	private void InitializeFile(
		string filePath, 
		JsonSerializerOptions serializerOptions, 
		int year, 
		int day, 
		int part, 
		string answer, 
		bool isAnswerCorrect, 
		TimeSpan duration, 
		string durationFriendly)
	{
		var dataModified = false;
		var fileJson = string.Empty;
		List<AoCAnswersYear> yearAnswers;

		// Create the data file if it doesn't already exist
		if (!File.Exists(filePath))
		{
			yearAnswers = CreateYearDayPart(year, day, part, answer, isAnswerCorrect, duration.Ticks, durationFriendly);

			fileJson = JsonSerializer.Serialize(yearAnswers, typeof(IEnumerable<AoCAnswersYear>), serializerOptions);

			File.WriteAllText(filePath, fileJson);

			return;
		}

		var answersRaw = File.ReadAllText(filePath);
		yearAnswers = JsonSerializer.Deserialize<List<AoCAnswersYear>>(answersRaw)!;

		// Data file exists and year data exists
		if (yearAnswers.Exists(y => y.Year == year))
		{
			var dayAnswers = yearAnswers.First(y => y.Year == year).Days;

			// Initialize Day and Parts data
			if (!dayAnswers.Exists(d => d.Day == day))
			{
				var dayAnswer = new AoCAnswersDay(day,
					part1: new AoCAnswersPart(
						day,
						part: 1,
						isSolved: false,
						correctAnswer: string.Empty,
						incorrectAnswers: [],
						durationTicks: -1,
						durationFriendly: string.Empty),
					part2: new AoCAnswersPart(
						day,
						part: 2,
						isSolved: false,
						correctAnswer: string.Empty,
						incorrectAnswers: [],
						durationTicks: -1,
						durationFriendly: string.Empty)
					);

				dayAnswers.Add(dayAnswer);
				dataModified = true;
			}
		}
		// Initialize Year, Day and Parts data
		else
		{
			var yearAnswer = CreateYearDayPart(year, day, part, answer, isAnswerCorrect, duration.Ticks, durationFriendly)[0];

			yearAnswers.Add(yearAnswer);
			dataModified = true;
		}

		if (dataModified)
		{
			fileJson = JsonSerializer.Serialize(yearAnswers, typeof(IEnumerable<AoCAnswersYear>), serializerOptions);

			File.WriteAllText(filePath, fileJson);
		}
	}

	// Generates data for both the year and day together for new files
	private List<AoCAnswersYear> CreateYearDayPart(
		int year, 
		int day, 
		int part, 
		string answer, 
		bool isAnswerCorrect, 
		long durationTicks, 
		string durationFriendly)
	{
		AoCAnswersDay dayAnswer;
		if (part == 1)
		{
			dayAnswer = new AoCAnswersDay(day,
				part1: new AoCAnswersPart(
					day,
					part: 1,
					isSolved: isAnswerCorrect,
					correctAnswer: isAnswerCorrect ? answer : string.Empty,
					incorrectAnswers: isAnswerCorrect ? [] : [answer],
					durationTicks: isAnswerCorrect ? durationTicks : -1,
					durationFriendly: isAnswerCorrect ? durationFriendly : string.Empty),
				part2: new AoCAnswersPart(
					day,
					part: 2,
					isSolved: false,
					correctAnswer: string.Empty,
					incorrectAnswers: [],
					durationTicks: -1,
					durationFriendly: string.Empty)
				);
		}
		else
		{
			dayAnswer = new AoCAnswersDay(day,
				part1: new AoCAnswersPart(
					day,
					part: 1,
					isSolved: false,
					correctAnswer: string.Empty,
					incorrectAnswers: [],
					durationTicks: -1,
					durationFriendly: string.Empty),
				part2: new AoCAnswersPart(
					day,
					part: 2,
					isSolved: isAnswerCorrect,
					correctAnswer: isAnswerCorrect ? answer : string.Empty,
					incorrectAnswers: isAnswerCorrect ? [] : [answer],
					durationTicks: isAnswerCorrect ? durationTicks : -1,
					durationFriendly: isAnswerCorrect ? durationFriendly : string.Empty)
				);

			
		}

		var yearAnswers = new List<AoCAnswersYear>
		{
				new(year, [dayAnswer])
		};

		return yearAnswers;
	}
}
