using Spectre.Console.Cli;
using System.ComponentModel;

namespace AdventOfCode.Core.CommandLineInterface.Commands
{
	public class HttpCommandSettings : CommandSettings
	{
		public string BaseUrl { get; private set; } = "https://adventofcode.com";
		public string UserAgent { get; private set; } = "github.com/cowlephant/AdventOfCode by admin@hexacow.com";

		[CommandOption("-y|--year <YEAR>")]
		[Description("Which year to get input for.")]
		public int Year { get; set; }

		[CommandOption("-d|--day <DAY>")]
		[Description("Which day to get input for.")]
		public int Day { get; set; }
	}
}
