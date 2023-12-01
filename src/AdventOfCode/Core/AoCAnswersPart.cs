namespace AdventOfCode.Core;

internal sealed class AoCAnswersPart
{
	public int Day { get; set; }
	public int Part { get; set; }
	public bool IsSolved { get; set; }
	public string CorrectAnswer { get; set; }
	public List<string> IncorrectAnswers { get; set; }
	public long DurationTicks { get; set; }
	public string DurationFriendly { get; set; }

	public AoCAnswersPart(
		int day,
		int part,
		bool isSolved,
		string correctAnswer,
		List<string> incorrectAnswers,
		long durationTicks,
		string durationFriendly)
	{
		Day = day;
		Part = part;
		IsSolved = isSolved;
		CorrectAnswer = correctAnswer;
		IncorrectAnswers = incorrectAnswers;
		DurationTicks = durationTicks;
		DurationFriendly = durationFriendly;
	}
}
