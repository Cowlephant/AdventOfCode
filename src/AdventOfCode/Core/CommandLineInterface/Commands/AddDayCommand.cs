using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Core.CommandLineInterface.Commands
{
    public sealed class AddDaySettings : CommandSettings
    {
        [CommandArgument(0, "<DAY>")]
        public string? Day { get; set; }

        [CommandOption("-n|--name <NAME>")]
        public string? Name { get; set; }
    }

    public sealed class AddDayCommand : Command<AddDaySettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] AddDaySettings settings)
        {
            Console.WriteLine($"Creating Day - Day:{settings.Name}");

            return 0;
        }
    }
}
