namespace AdventOfCode.Core
{
    public abstract class AdventOfCodeDayBase : IAdventOfCodeRunner
    {
        public abstract string RunPartOne(IEnumerable<string> dataSet);
        public abstract string RunPartTwo(IEnumerable<string> dataSet);
    }
}
