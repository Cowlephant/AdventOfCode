namespace AdventOfCode
{
	public sealed class RunnerSettings
	{
		public bool UseExampleData { get; set; } = false;
		public bool RunAllDays { get; set; } = true;
		public int YearToRun { get; set; }
		public IEnumerable<string> DaysToRun { get; set; } = new List<string>();
		public bool RunPartOne { get; set; } = true;
		public bool RunPartTwo { get; set; } = true;
	}
}
