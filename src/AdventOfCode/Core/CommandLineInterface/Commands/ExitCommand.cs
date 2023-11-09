using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Core.CommandLineInterface.Commands
{
	public sealed class ExitCommand : Command
	{
		public override int Execute([NotNull] CommandContext context)
		{
			if (!AnsiConsole.Confirm("Are you sure you wish to exit?"))
			{
				return 0;
			}
			else
			{
				return 2;
			}
		}
	}
}
