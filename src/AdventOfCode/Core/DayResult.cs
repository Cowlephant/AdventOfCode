namespace AdventOfCode.Core;

internal sealed record class DayResult(
	string DayName,
	int DayYear,
	bool IsUsingExampleData,
	IEnumerable<PartResult> PartOneResults,
	IEnumerable<PartResult> PartTwoResults)
{
}
