namespace AdventOfCode.Core
{
	public sealed record class DayResult(
		string dayName,
		int dayYear,
		bool IsUsingExampleData,
		IEnumerable<PartResult> PartOneResults,
		IEnumerable<PartResult> PartTwoResults)
	{
	}
}
