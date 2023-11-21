namespace AdventOfCode.Core;

internal sealed record class PartResult(string Answer, string ExpectedAnswer, bool AnswersMatch, TimeSpan Duration, string DurationFriendly)
{
}
