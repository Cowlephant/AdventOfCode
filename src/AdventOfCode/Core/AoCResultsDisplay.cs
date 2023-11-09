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
			if (!results.Any())
			{
				AnsiConsole.MarkupLine($"No results found to run for year [red]{settings.YearToRun}[/]");
				return;
			}

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
					var allPartOneCorrect = true;
					var allPartOneWrong = true;

					foreach (var partOneResult in result.PartOneResults)
					{
						CreatePartRow(partOneResult, partOneTable, out bool isAnswerCorrect);

						allPartOneCorrect = allPartOneCorrect && isAnswerCorrect;
						allPartOneWrong = allPartOneWrong && !isAnswerCorrect;
					}

					if (settings.UseExampleData)
					{
						StylizeTableSuccess(partOneTable, allPartOneCorrect, allPartOneWrong);
					}
				}

				if (settings.RunPartTwo)
				{
					var partTwoNode = dayNode.AddNode("Part 2");
					var partTwoTable = CreatePartTable();
					partTwoNode.AddNode(partTwoTable);
					var allPartTwoCorrect = true;
					var allPartTwoWrong = true;

					foreach (var partTwoResult in result.PartTwoResults)
					{
						CreatePartRow(partTwoResult, partTwoTable, out bool isAnswerCorrect);

						allPartTwoCorrect = allPartTwoCorrect && isAnswerCorrect;
						allPartTwoWrong = allPartTwoWrong && !isAnswerCorrect;
					}

					if (settings.UseExampleData)
					{
						StylizeTableSuccess(partTwoTable, allPartTwoCorrect, allPartTwoWrong);
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
				partTable.Columns[0].Width = 10;
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

		private void CreatePartRow(PartResult partResult, Table partTable, out bool answerCorrect)
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

			answerCorrect = partResult.answersMatch;
		}

		private static void StylizeTableSuccess(Table table, bool isAllCorrect, bool isAllWrong)
		{
			// All correct
			if (isAllCorrect && !isAllWrong)
			{
				table.BorderColor(Color.Green);
			}
			// Some correct
			else if (!isAllCorrect && !isAllWrong)
			{
				table.BorderColor(Color.Yellow);
			}
			// All wrong
			else
			{
				table.BorderColor(Color.Red);
			}
		}
	}
}
