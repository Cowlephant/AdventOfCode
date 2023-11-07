using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AdventOfCode.Core
{
    internal sealed class AdventOfCodeRunner
    {
        private readonly AdventOfCodeSettings settings;
        private readonly AdventOfCodeInputReader dayInputReader;

        public AdventOfCodeRunner(IConfiguration configuration, AdventOfCodeInputReader dayInputReader)
        {
            settings = configuration.GetSection(nameof(AdventOfCodeSettings)).Get<AdventOfCodeSettings>()!;
            this.dayInputReader = dayInputReader;
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
                throw new AdventOfCodeException("No days configured to run.");
            }
        }

        private void RunParts(IAdventOfCodeRunner dayRunner)
        {
            const int dividerLengthTop = 65;
            var dayNumber = int.Parse(dayRunner.GetType().Name!.Replace("Day", ""));

            var dayHeader = $"Day {dayNumber} {new string('-', dividerLengthTop)}";
            Console.WriteLine(dayHeader);

            if (settings.RunPartOne)
            {
                IEnumerable<string> answers = dayRunner.RunPartOne();

                Console.WriteLine($"\tPart 1");

                foreach (var (answer, index) in answers.Select((a, i) => (a, i)))
                {
                    string answerResult = ValidateExpectedAnswer(
                        answers, dayRunner, "RunPartOne", answer, index);

                    Console.WriteLine($"\t\tDataset {index + 1} Answer: {answerResult}");
                }
            }
            if (settings.RunPartTwo)
            {
                IEnumerable<string> answers = dayRunner.RunPartTwo();

                Console.WriteLine($"\tPart 2");

                foreach (var (answer, index) in answers.Select((a, i) => (a, i)))
                {
                    string answerResult = ValidateExpectedAnswer(
                        answers, dayRunner, "RunPartTwo", answer, index);

                    Console.WriteLine($"\t\tDataset {index + 1} Answer: {answerResult}");
                }
            }

            Console.WriteLine(new string('-', dayHeader.Length));
            Console.WriteLine();
        }

        private string ValidateExpectedAnswer(
            IEnumerable<string> answers,
            IAdventOfCodeRunner dayRunner,
            string partName,
            string answer,
            int index)
        {
            var expectedAnswers = dayRunner.GetType().GetMethod(partName)!
                .GetCustomAttributes<ExpectedExampleAnswersAttribute>()!
                    .SelectMany(a => a.ExpectedExampleAnswers)
                    .ToList();
            var answerCount = answers.Count();
            var expectedAnswerCount = expectedAnswers.Count;

            if (answerCount != expectedAnswerCount)
            {
                throw new AdventOfCodeException($"Expected answer count must match the provided number of answers. " +
                    $"Answers: {answerCount} ExpectedAnswers: {expectedAnswerCount}");
            }

            string expectedanswer = expectedAnswers[index];
            bool isExpectedAnswerCorrect = answer == expectedanswer;
            string expectedAnswerResult;
            if (answer != "Not Implemented" && settings.UseExampleData)
            {
                expectedAnswerResult = isExpectedAnswerCorrect ? $"(CORRECT)" : $"(INCORRECT)";
                expectedAnswerResult = $"{expectedAnswerResult} Expected: {expectedanswer} Actual: {answer}";
            }
            else
            {
                expectedAnswerResult = answer;
            }

            return expectedAnswerResult;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "Performance optimization not necessary")]
        private IEnumerable<IAdventOfCodeRunner> GetAllDays(IEnumerable<string>? filteredDays = null)
        {
            var adventOfCodeRunnerType = typeof(IAdventOfCodeRunner);

            var allDays = adventOfCodeRunnerType.Assembly.GetTypes()
                .Where(type => adventOfCodeRunnerType.IsAssignableFrom(type)
                && type.CustomAttributes.Any(a => a.AttributeType == typeof(AdventOfCodeYearAttribute))
                && type.GetCustomAttribute<AdventOfCodeYearAttribute>()!.Year == settings.YearToRun
                && !type.IsAbstract);

            foreach (var day in allDays.Select(d => d.Name))
            {
                var className = day;
                var namingPattern = new Regex(@"^Day[012]\d");

                if (!namingPattern.IsMatch(className))
                {
                    throw new AdventOfCodeException($"Day class name does not match required pattern Day##: {className}");
                }
            }

            List<IAdventOfCodeRunner> daysToRun = new();

            if (filteredDays == null)
            {
                foreach (var day in allDays)
                {
                    var dayToRun = (IAdventOfCodeRunner)Activator.CreateInstance(day, dayInputReader)!;
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
                    var dayToRun = (IAdventOfCodeRunner)Activator.CreateInstance(day, dayInputReader)!;

                    daysToRun.Add(dayToRun);
                }
            }

            return daysToRun;
        }
    }
}
