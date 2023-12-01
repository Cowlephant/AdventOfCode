namespace AdventOfCode.Core;

internal sealed class AoCAnswersYear
{
	public int Year { get; set; }
	public List<AoCAnswersDay> Days { get; set; }

	public AoCAnswersYear(int year, List<AoCAnswersDay> days)
	{
		Year = year;
		Days = days;
	}
}
