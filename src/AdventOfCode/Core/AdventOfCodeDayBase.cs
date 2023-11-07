using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Core
{
    public abstract class AdventOfCodeDayBase : IAdventOfCodeRunner
    {
        [Benchmark]
        public abstract string RunPartOne(IEnumerable<string> dataSet);
        [Benchmark]
        public abstract string RunPartTwo(IEnumerable<string> dataSet);
    }
}
