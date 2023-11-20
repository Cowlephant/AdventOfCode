using AdventOfCode.Core.CommandLineInterface.Commands;
using Spectre.Console;

namespace AdventOfCode.Core.CommandLineInterface;

internal static class AoCHelper
{
	/// <summary>
	/// Validates whether a personal token is provided, whether or not it is valid.
	/// Will output a message to the console if it is not set.
	/// </summary>
	/// <param name="personalToken">The personal token to be validated.</param>
	/// <returns></returns>
	public static bool ValidatePersonalTokenIsProvided(string personalToken)
	{
		bool isTokenInvalid = string.IsNullOrWhiteSpace(personalToken)
			|| personalToken == "<DO_NOT_COMMIT_ME>";

		if (isTokenInvalid)
		{
			AnsiConsole.MarkupLine("AocSettings.PersonalToken is not set. Use User Secrets or Environment Variables to set this.\n" +
				"[red]BE CAREFUL YOU ARE NOT COMMITTING ANY SECRETS AND [yellow]DO NOT ADD[/] THIS VALUE TO [yellow]appsettings.json[/][/]");

			return false;
		}
		else
		{
			return true;
		}
	}

	public static ValidationResult ValidateDateNotTooEarly(HttpCommandSettings settings)
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
