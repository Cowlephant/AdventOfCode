namespace AdventOfCode.Core
{
    public sealed class AdventOfCodeException : Exception
    {
        public AdventOfCodeException(string? message) : base(message)
        {
        }
    }
}
