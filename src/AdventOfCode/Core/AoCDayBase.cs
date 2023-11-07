namespace AdventOfCode.Core
{
    public abstract class AoCDayBase : IAoCDayRunner
    {
        public abstract string RunPartOne(IEnumerable<string> input);
        public abstract string RunPartTwo(IEnumerable<string> input);
    }
}
