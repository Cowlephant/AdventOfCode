namespace AdventOfCode.Core;

internal sealed class AoCAnswersDay
{
	public int Day { get; set; }
	public AoCAnswersPart? Part1 { get; set; }
	public AoCAnswersPart? Part2 { get; set; }

	public AoCAnswersDay(int day, AoCAnswersPart? part1, AoCAnswersPart? part2)
	{
		Day = day;
		Part1 = part1;
		Part2 = part2;
	}
}
