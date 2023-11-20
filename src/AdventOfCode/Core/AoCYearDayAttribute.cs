namespace AdventOfCode.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class AoCYearDayAttribute : Attribute
{
    public int Year { get; private set; }
    public int Day { get; private set; }

    public AoCYearDayAttribute(int year, int day)
    {
        Year = year;
        Day = day;
    }
}