using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Core
{
    public abstract class AdventOfCodeDayBase : IAdventOfCodeRunner
    {
        protected List<string> Answers { get; set; }
        private readonly AdventOfCodeInputReader inputReader;

        protected AdventOfCodeDayBase(AdventOfCodeInputReader inputReader)
        {
            Answers = new List<string>();
            this.inputReader = inputReader;
        }

        protected (IEnumerable<IEnumerable<string>> PartOne,
                   IEnumerable<IEnumerable<string>> PartTwo)
            GetFileData()
        {
            // Clear the previous Answers
            Answers = new List<string>();

            return inputReader.GetData();
        }

        [Benchmark]
        public abstract IEnumerable<string> RunPartOne();
        [Benchmark]
        public abstract IEnumerable<string> RunPartTwo();
    }
}
