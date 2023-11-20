using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Core.CommandLineInterface.Commands;

internal sealed class ClearCommand : Command
{
	public override int Execute([NotNull] CommandContext context)
	{
		Console.Clear();

		return 0;
	}
}
