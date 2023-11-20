namespace AdventOfCode.Core;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal sealed class AoCExpectedExampleAnswersAttribute : Attribute
{
    public IEnumerable<string> ExpectedExampleAnswers { get; private set; }
    public AoCExpectedExampleAnswersAttribute(params string[] expectedExampleAnswer)
    {
        ExpectedExampleAnswers = expectedExampleAnswer;
    }
}