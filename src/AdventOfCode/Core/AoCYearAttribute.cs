namespace AdventOfCode.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class AoCYearAttribute : Attribute
    {
        public int Year { get; private set; }
        public AoCYearAttribute(int year)
        {
            Year = year;
        }
    }
}