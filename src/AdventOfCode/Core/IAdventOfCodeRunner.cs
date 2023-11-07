namespace AdventOfCode.Core
{
    public interface IAdventOfCodeRunner
    {
        public string RunPartOne(IEnumerable<string> dataSet);
        public string RunPartTwo(IEnumerable<string> dataSet);
    }
}
