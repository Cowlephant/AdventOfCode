namespace AdventOfCode.Core
{
	public sealed record class DayResult(
		string DayName,
		int DayYear,
		bool IsUsingExampleData,
		IEnumerable<PartResult> PartOneResults,
		IEnumerable<PartResult> PartTwoResults)
	{
	}
}
