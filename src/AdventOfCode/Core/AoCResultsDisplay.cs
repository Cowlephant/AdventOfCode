using Spectre.Console;
using System.Text.Json;

namespace AdventOfCode.Core
{
	public sealed class AoCResultsDisplay
	{
		private readonly AoCSettings settings;

		public AoCResultsDisplay(AoCSettings settings)
		{
			this.settings = settings;
		}

		public void Display(IEnumerable<DayResult> results)
		{
			var yearNode = new Tree($"Advent of Code {results.First().dayYear}");
			var yearWrapper = new Padder(yearNode)
				.PadLeft(5);

			foreach (var result in results)
			{
				var dayNode = yearNode.AddNode(result.dayName);

				if (settings.RunPartOne)
				{
					var partOneTable = CreatePartTable();
					var partOneNode = dayNode.AddNode("Part 1");
					partOneNode.AddNode(partOneTable);
					bool allPartOneCorrect = true;

					foreach (var partOneResult in result.PartOneResults)
					{
						CreatePartRow(partOneResult, partOneTable, out allPartOneCorrect);
					}

					if (settings.UseExampleData)
					{
						StylizeSuccess(partOneTable, allPartOneCorrect);
					}
				}

				if (settings.RunPartTwo)
				{
					var partTwoNode = dayNode.AddNode("Part 2");
					var partTwoTable = CreatePartTable();
					partTwoNode.AddNode(partTwoTable);
					bool allPartTwoCorrect = true;

					foreach (var partTwoResult in result.PartTwoResults)
					{
						CreatePartRow(partTwoResult, partTwoTable, out allPartTwoCorrect);
					}

					if (settings.UseExampleData)
					{
						StylizeSuccess(partTwoTable, allPartTwoCorrect);
					}
				}
			}

			AnsiConsole.Write(yearWrapper);
		}

		private Table CreatePartTable()
		{
			Table partTable;

			if (settings.UseExampleData)
			{
				partTable = new Table()
					.RoundedBorder()
					.AddColumn("Result", options => { options.Alignment = Justify.Left; })
					.AddColumn("Duration", options => { options.Alignment = Justify.Right; })
					.AddColumn("Answer", options => { options.Alignment = Justify.Right; })
					.AddColumn("Expected", options => { options.Alignment = Justify.Right; })
					.Border(TableBorder.Rounded)
					.LeftAligned()
					.Collapse();
			}
			else
			{
				partTable = new Table()
					.RoundedBorder()
					.AddColumn("Duration", options => { options.Alignment = Justify.Right; })
					.AddColumn("Answer", options => { options.Alignment = Justify.Right; })
					.Border(TableBorder.Rounded)
					.LeftAligned()
					.Collapse();
			}

			return partTable;
		}

		private void CreatePartRow(PartResult partResult, Table partTable, out bool allAnswersCorrect)
		{
			bool isImplemented = partResult.Answer != "Not Implemented";

			var resultStatus = partResult.answersMatch ? "[green]CORRECT[/]" : "[red]INCORRECT[/]";
			resultStatus = isImplemented ? resultStatus : string.Empty;
			var duration = isImplemented ? partResult.Duration : string.Empty;

			if (settings.UseExampleData)
			{
				partTable.AddRow(
					resultStatus,
					duration,
					partResult.Answer,
					partResult.ExpectedAnswer);
			}
			else
			{
				partTable.AddRow(duration, partResult.Answer);
			}

			if (!partResult.answersMatch)
			{
				allAnswersCorrect = false;
			}
			else
			{
				allAnswersCorrect = true;
			}
		}

		private void StylizeSuccess(Table table, bool isSuccessful)
		{
			if (isSuccessful)
			{
				table.BorderColor(Color.Green);
			}
			else
			{
				table.BorderColor(Color.Red);
			}
		}
	}
}
