namespace AdventOfCode.Core;

internal sealed class AoCSettings
{
	public string PersonalToken { get; set; } = "<DO_NOT_COMMIT_ME>";
	public bool UseExampleData { get; set; } = false;
	public bool RunAllDays { get; set; } = true;
	public int YearToRun { get; set; }
	public IEnumerable<int> DaysToRun { get; set; } = new List<int>();
	public bool RunPartOne { get; set; } = true;
	public bool RunPartTwo { get; set; } = true;
	public string SolutionFolderPath { get; set; } = "Solution";
	public string DataFolderPath { get; set; } = "Data";
}
