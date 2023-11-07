namespace AdventOfCode.Core
{
    public sealed class AoCException : Exception
    {
        public AoCException(string? message) : base(message)
        {
        }
    }
}
