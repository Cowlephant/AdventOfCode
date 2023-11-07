namespace AdventOfCode.Core
{
    public abstract class AoCDayBase : IAoCDaySolver
    {
        public abstract string SolvePartOne(IEnumerable<string> input);
        public abstract string SolvePartTwo(IEnumerable<string> input);
    }
}
