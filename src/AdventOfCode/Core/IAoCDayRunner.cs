namespace AdventOfCode.Core
{
	public interface IAoCDayRunner
	{
		public string RunPartOne(IEnumerable<string> input);
		public string RunPartTwo(IEnumerable<string> input);
	}
}
