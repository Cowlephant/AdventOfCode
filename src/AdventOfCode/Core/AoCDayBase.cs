namespace AdventOfCode.Core
{
    public abstract class AoCDayBase : IAdventOfCodeRunner
    {
        public abstract string RunPartOne(IEnumerable<string> dataSet);
        public abstract string RunPartTwo(IEnumerable<string> dataSet);
    }
}
