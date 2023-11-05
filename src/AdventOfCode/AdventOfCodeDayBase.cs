using BenchmarkDotNet.Attributes;

namespace AdventOfCode
{
	public abstract class AdventOfCodeDayBase : IAdventOfCodeRunner
	{
		[Benchmark]
		public abstract string RunPartOne();
		[Benchmark]
		public abstract string RunPartTwo();
	}
}
