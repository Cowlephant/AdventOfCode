using AdventOfCode.Core;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    internal sealed class AoCRunner
    {
        private readonly AoCSettings settings;
        private readonly AoCInputReader inputReader;
        private readonly Stopwatch stopwatch;

        public AoCRunner(IConfiguration configuration, AoCInputReader inputReader)
        {
            settings = configuration.GetSection(nameof(AoCSettings)).Get<AoCSettings>()!;
            this.inputReader = inputReader;
            stopwatch = new Stopwatch();
        }

        public void Run()
        {
            RunDays();
        }

        private void RunDays()
        {
            var withExampleData = settings.UseExampleData ? " With Example Data" : "";

            if (settings.RunAllDays)
            {
                Console.WriteLine($"Running All Days{withExampleData}");

                foreach (var day in GetAllDays())
                {
                    RunParts(day);
                }
            }
            else if (settings.DaysToRun.Any())
            {
                var daysToRun = string.Join(", ", settings.DaysToRun);
                Console.WriteLine($"Running Specific Days{withExampleData}: {daysToRun}");

                foreach (var day in GetAllDays(settings.DaysToRun))
                {
                    RunParts(day);
                }
            }
            else
            {
                throw new AoCException("No days configured to run.");
            }
        }

        private void RunParts(IAoCDaySolver daySolver)
        {
            const int dividerLengthTop = 65;
            var dayNumber = int.Parse(daySolver.GetType().Name!.Replace("Day", ""));

            var dayHeader = $"Day {dayNumber} {new string('-', dividerLengthTop)}";
            Console.WriteLine(dayHeader);

            string dayName = daySolver.GetType().Name;
            var (PartOneData, PartTwoData) = inputReader.GetData(dayName);

            if (settings.RunPartOne)
            {
                List<(string Answer, long DurationTicks)> answers = new();

                foreach (var partOneData in PartOneData)
                {
                    stopwatch.Start();
                    var answer = daySolver.SolvePartOne(partOneData);
                    stopwatch.Stop();
                    var duration = stopwatch.ElapsedTicks;

                    answers.Add(new(answer, duration));
                }

                Console.WriteLine($"\tPart 1");

                foreach (var (answer, index) in answers.Select((a, i) => (a, i)))
                {
                    string answerResult = ValidateExpectedAnswer(
                        answers, daySolver, "SolvePartOne", answer, index);

                    Console.WriteLine($"\t\tDataset {index + 1} Answer: {answerResult}");
                }

                stopwatch.Reset();
            }
            if (settings.RunPartTwo)
            {
                List<(string Answer, long DurationTicks)> answers = new();

                foreach (var partTwoData in PartTwoData)
                {
                    stopwatch.Start();
                    var answer = daySolver.SolvePartOne(partTwoData);
                    stopwatch.Stop();
                    var duration = stopwatch.ElapsedTicks;

                    answers.Add(new(answer, duration));
                }

                Console.WriteLine($"\tPart 2");

                foreach (var (answer, index) in answers.Select((a, i) => (a, i)))
                {
                    string answerResult = ValidateExpectedAnswer(
                        answers, daySolver, "SolvePartTwo", answer, index);

                    Console.WriteLine($"\t\tDataset {index + 1} Answer: {answerResult}");
                }
            }

            Console.WriteLine(new string('-', dayHeader.Length));
            Console.WriteLine();
        }

        private string ValidateExpectedAnswer(
            IEnumerable<(string Answer, long Duration)> answers,
            IAoCDaySolver daySolver,
            string partName,
            (string Answer, long DurationTicks) answer,
            int index)
        {
            var expectedAnswers = daySolver.GetType().GetMethod(partName)!
                .GetCustomAttributes<AoCExpectedExampleAnswersAttribute>()!
                    .SelectMany(a => a.ExpectedExampleAnswers)
                    .ToList();
            var answerCount = answers.Count();
            var expectedAnswerCount = expectedAnswers.Count;

            if (answerCount != expectedAnswerCount)
            {
                throw new AoCException($"Expected answer count must match the provided number of answers.\n" +
                    "Check your [ExpectedExampleAnswers()] attribute!\n" +
                    $"Answers: {answerCount} ExpectedAnswers: {expectedAnswerCount}");
            }

            string expectedanswer = expectedAnswers[index];
            bool isExpectedAnswerCorrect = answer.Answer == expectedanswer;
            string expectedAnswerResult;
            if (answer.Answer != "Not Implemented" && settings.UseExampleData)
            {
                long answerMicroseconds = answer.DurationTicks / (TimeSpan.TicksPerMillisecond / 1000);
                // Conver to milliseconds if long enough, otherwise microseconds
                string answerString = answerMicroseconds > 1000 ? $"{answerMicroseconds / 1000}ms" : $"{answerMicroseconds}μs";

                expectedAnswerResult = isExpectedAnswerCorrect ? $"(CORRECT)" : $"(INCORRECT)";
                expectedAnswerResult = $"{expectedAnswerResult} (Duration: {answerString}) Expected: {expectedanswer} Actual: {answer.Answer}";
            }
            else
            {
                expectedAnswerResult = answer.Answer;
            }

            return expectedAnswerResult;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "Performance optimization not necessary")]
        private List<IAoCDaySolver> GetAllDays(IEnumerable<string>? filteredDays = null)
        {
            var daySolverType = typeof(IAoCDaySolver);

            var allDays = daySolverType.Assembly.GetTypes()
                .Where(type => daySolverType.IsAssignableFrom(type)
                && type.CustomAttributes.Any(a => a.AttributeType == typeof(AoCYearAttribute))
                && type.GetCustomAttribute<AoCYearAttribute>()!.Year == settings.YearToRun
                && !type.IsAbstract);

            foreach (var day in allDays.Select(d => d.Name))
            {
                var className = day;
                var namingPattern = new Regex(@"^Day[012]\d");

                if (!namingPattern.IsMatch(className))
                {
                    throw new AoCException($"Day class name does not match required pattern Day##: {className}");
                }
            }

            List<IAoCDaySolver> daysToRun = new();

            if (filteredDays == null)
            {
                foreach (var day in allDays)
                {
                    var dayToRun = (IAoCDaySolver)Activator.CreateInstance(day)!;
                    daysToRun.Add(dayToRun);
                }
            }
            else
            {
                var selectedDays = allDays.Where(day =>
                    filteredDays.Any(d =>
                        d.Equals(day.Name, StringComparison.CurrentCultureIgnoreCase)));

                foreach (var day in selectedDays)
                {
                    var dayToRun = (IAoCDaySolver)Activator.CreateInstance(day, inputReader)!;

                    daysToRun.Add(dayToRun);
                }
            }

            return daysToRun;
        }
    }
}
