using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Core.CommandLineInterface.Commands
{
	public sealed class ExitCommand : Command
	{
		public override int Execute([NotNull] CommandContext context)
		{
			AnsiConsole.Markup("[yellow]Exiting...[/]");
			return 2;
		}
	}
}
