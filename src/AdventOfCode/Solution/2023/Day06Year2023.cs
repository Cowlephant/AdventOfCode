using AdventOfCode.Core;
using System.Runtime.InteropServices;

namespace AdventOfCode.Solution;

[AoCYearDay(2023, 6)]
public sealed class Day06Year2023 : IAoCDaySolver
{
	[AoCExpectedExampleAnswers("288")]
	public string SolvePartOne(List<string> input)
	{
		var times = input[0]
			.Split(new char[0]) // Split on whitespace
			.Where(l => !string.IsNullOrWhiteSpace(l)) // Remove whitespace
			.Skip(1) // Remove row title
			.Select(l => long.Parse(l.Trim())) // Convert to int
			.ToList();
		var distances = input[1]
			.Split(new char[0])
			.Where(l => !string.IsNullOrWhiteSpace(l))
			.Skip(1)
			.Select(l => long.Parse(l.Trim()))
			.ToList();

		List<(long Time, long Distance)> timeDistances = [];

		for (int i = 0; i < times.Count; i++)
		{
			timeDistances.Add((times[i], distances[i]));
		}

		long waysToBeatRecord = 1;

		foreach (var timeDistance in timeDistances)
		{
			waysToBeatRecord *= CalculateWaysToBeatTime(timeDistance);
		}

		return waysToBeatRecord.ToString();
	}

	private static long CalculateWaysToBeatTime((long Time, long Distance) timeDistance)
	{
		long waysToBeatTime = 0;

		// Ignore 0 and maximum seconds
		for (int i = 1; i <= timeDistance.Time - 1; i++)
		{
			var timeToPress = i;
			var timeRemaining = timeDistance.Time - timeToPress;
			var distanceTravelled = timeToPress * timeRemaining;
			bool beatsTime = distanceTravelled > timeDistance.Distance;

			if (beatsTime)
			{
				waysToBeatTime++;
			}
		}

		return waysToBeatTime;
	}

	[AoCExpectedExampleAnswers("71503")]
	public string SolvePartTwo(List<string> input)
	{
		var time = long.Parse(
			string.Join("", input[0]
				.Split(new char[0]) // Split on whitespace
				.Where(l => !string.IsNullOrWhiteSpace(l)) // Remove whitespace
				.Skip(1))); // Remove row title

		var distance = long.Parse(
			string.Join("", input[1]
				.Split(new char[0])
				.Where(l => !string.IsNullOrWhiteSpace(l))
				.Skip(1)));

		var waysToBeatRecord = CalculateWaysToBeatTime((time, distance));

		return waysToBeatRecord.ToString();
	}
}
