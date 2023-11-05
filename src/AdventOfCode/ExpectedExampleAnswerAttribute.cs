namespace AdventOfCode
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ExpectedExampleAnswerAttribute : Attribute
    {
        public IEnumerable<string> ExpectedExampleAnswer { get; private set; }
        public ExpectedExampleAnswerAttribute(string expectedExampleAnswer)
        {
            ExpectedExampleAnswer = expectedExampleAnswer;
        }
    }
}