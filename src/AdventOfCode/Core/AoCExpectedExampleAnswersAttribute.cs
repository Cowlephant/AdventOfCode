namespace AdventOfCode.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AoCExpectedExampleAnswersAttribute : Attribute
    {
        public IEnumerable<string> ExpectedExampleAnswers { get; private set; }
        public AoCExpectedExampleAnswersAttribute(params string[] expectedExampleAnswer)
        {
            ExpectedExampleAnswers = expectedExampleAnswer;
        }
    }
}