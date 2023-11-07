namespace AdventOfCode.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class AdventOfCodeYearAttribute : Attribute
    {
        public int Year { get; private set; }
        public AdventOfCodeYearAttribute(int year)
        {
            Year = year;
        }
    }
}