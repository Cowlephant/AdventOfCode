namespace AdventOfCode
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ExpectedExampleAnswersAttribute : Attribute
    {
        public IEnumerable<string> ExpectedExampleAnswers { get; private set; }
        public ExpectedExampleAnswersAttribute(params string[] expectedExampleAnswer)
        {
            ExpectedExampleAnswers = expectedExampleAnswer;
        }
    }
}