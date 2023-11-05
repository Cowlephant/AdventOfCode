using BenchmarkDotNet.Attributes;

namespace AdventOfCode
{
	public abstract class AdventOfCodeDayBase : IAdventOfCodeRunner
	{
		protected List<string> Answers { get; set; }

		protected (IEnumerable<IEnumerable<string>> PartOne, IEnumerable<IEnumerable<string>> PartTwo) GetFileData()
		{
			// Clear the previous Answers
			Answers = new List<string>();
			return DayInputReader.GetData();
		}

		protected AdventOfCodeDayBase()
		{
			Answers = new List<string>();
		}

		[Benchmark]
		public abstract IEnumerable<string> RunPartOne();
		[Benchmark]
		public abstract IEnumerable<string> RunPartTwo();
	}
}
