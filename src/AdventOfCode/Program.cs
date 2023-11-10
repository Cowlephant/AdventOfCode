using AdventOfCode.Core;
using AdventOfCode.Core.CommandLineInterface;
using AdventOfCode.Core.CommandLineInterface.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;
using System.CommandLine.Parsing;

namespace AdventOfCode
{
	static internal partial class Program
	{
		public static IConfiguration Configuration { get; set; } = null!;
		public static AoCSettings RunnerSettings { get; set; } = null!;

		static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.BuildConfiguration()
				.Build();

			var host = Host.CreateDefaultBuilder()
				.ConfigureServices((context, services) =>
				{
					services.AddSingleton(serviceProvider =>
					{
						return builder.GetSection(nameof(AoCSettings)).Get<AoCSettings>()!;
					});
					services.AddTransient<AoCRunner>();
					services.AddTransient<AoCInputReader>();
					services.AddTransient<AoCResultsDisplay>();
					services.AddSingleton(new TypeRegistrar(services));
				})
				.Build();

			var registrar = host.Services.GetRequiredService<TypeRegistrar>();

			RunCLI(registrar);
		}

		private static IConfigurationBuilder BuildConfiguration(this IConfigurationBuilder builder)
		{
			var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
			builder.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environment}.json", optional: true)
				.AddEnvironmentVariables();

			return builder;
		}

		private static void RunCLI(TypeRegistrar registrar)
		{
			var app = new CommandApp(registrar);
			ConfigureCLI(app);

			while (true)
			{
				Console.Write("> ");
				var input = Console.ReadLine() ?? string.Empty;

				var commandInput = CommandLineStringSplitter.Instance.Split(input);
				if (commandInput.Any())
				{
					int commandCode = app.Run(commandInput);

					if (commandCode == 2)
					{
						break;
					}
				}
			}
		}

		private static void ConfigureCLI(CommandApp app)
		{
			app.Configure(config =>
			{
				config.SetApplicationName(string.Empty);
			});

			app.Configure(config =>
			{
				config.AddCommand<ExitCommand>("exit")
					.WithDescription("Exits the application. Use \"quit\" \"exit\" \"stop\" \"end\" or \"q\"")
					.WithAlias("quit")
					.WithAlias("stop")
					.WithAlias("end")
					.WithAlias("q");
				config.AddCommand<ClearCommand>("clear")
					.WithDescription("Clears the console.")
					.WithAlias("cls");
				config.AddCommand<AddDayCommand>("add")
						.WithDescription(
							"Adds one or more day template files to your project.\n" +
							"If no year specified, current calendar year will be used.")
						.WithAlias("create")
						.WithAlias("new")
						.WithExample("add -d 2")
						.WithExample("add -y 2023 -d 2")
						.WithExample("add -y 2023 -d 1 -d 2 -d 3 -d 4 -d 5");
				config.AddCommand<RunCommand>("run")
					.WithAlias("solve")
					.WithDescription("Runs your code with specified parameters to solve input.")
					.WithExample("run -d 2")
					.WithExample("run --year 2023 -d 1 -d 2 --real")
					.WithExample("solve -y 2023 -p 1")
					.WithExample("solve -y 2023 -p 2 --day 1");
			});
		}
	}
}
