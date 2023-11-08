using Spectre.Console;
using System.Text.Json;

namespace AdventOfCode.Core
{
	public sealed class AoCResultsDisplay
	{
		public void Display(IEnumerable<DayResult> results)
		{
			var yearNode = new Tree($"Advent of Code {results.First().dayYear}");
			var yearWrapper = new Padder(yearNode)
				.PadLeft(5);

			foreach (var result in results)
			{
				var dayNode = yearNode.AddNode(result.dayName);
				
				var partOneNode = dayNode.AddNode("Part 1");
				var partTwoNode = dayNode.AddNode("Part 2");

				var partOneTable = CreatePartTable();
				var partTwoTable = CreatePartTable();

				partOneNode.AddNode(partOneTable);
				partTwoNode.AddNode(partTwoTable);

				bool allPartOneCorrect = true;
				bool allPartTwoCorrect = true;

				foreach (var partOneResult in result.PartOneResults)
				{
					CreatePartRow(partOneResult, partOneTable, out allPartOneCorrect);
				}

				foreach (var partTwoResult in result.PartTwoResults)
				{
					CreatePartRow(partTwoResult, partTwoTable, out allPartTwoCorrect);
				}

				StylizeSuccess(partOneTable, allPartOneCorrect);
				StylizeSuccess(partTwoTable, allPartTwoCorrect);
			}

			AnsiConsole.Write(yearWrapper);
		}

		private Table CreatePartTable()
		{
			var partTable = new Table()
				.RoundedBorder()
				.AddColumn("Result", options => { options.Alignment = Justify.Left; })
				.AddColumn("Duration", options => { options.Alignment = Justify.Right; })
				.AddColumn("Answer", options => { options.Alignment = Justify.Right; })
				.AddColumn("Expected", options => { options.Alignment = Justify.Right; })
				.Border(TableBorder.Rounded)
				.LeftAligned()
				.Collapse();

			return partTable;
		}

		private void CreatePartRow(PartResult partResult, Table partTable, out bool allAnswersCorrect)
		{
			bool isImplemented = partResult.Answer != "Not Implemented";
			var resultStatus = partResult.answersMatch ? "[green]CORRECT[/]" : "[red]INCORRECT[/]";
			resultStatus = isImplemented ? resultStatus : string.Empty;
			var duration = isImplemented ? partResult.Duration : string.Empty;
			partTable.AddRow(
				resultStatus,
				duration,
				partResult.Answer,
				partResult.ExpectedAnswer);

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
