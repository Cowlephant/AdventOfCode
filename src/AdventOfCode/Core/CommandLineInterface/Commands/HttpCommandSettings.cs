using Spectre.Console.Cli;

namespace AdventOfCode.Core.CommandLineInterface.Commands
{
	public class HttpCommandSettings : CommandSettings
	{
		public string BaseUrl { get; private set; } = "https://adventofcode.com";
		public string UserAgent { get; private set; } = "github.com/cowlephant/AdventOfCode by admin@hexacow.com";
	}
}
