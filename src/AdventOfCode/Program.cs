﻿using AdventOfCode.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdventOfCode
{
    static internal partial class Program
	{
		public static IConfiguration Configuration { get; set; } = null!;
		public static AoCSettings RunnerSettings { get; set; } = null!;

		static void Main()
		{
			var builder = new ConfigurationBuilder()
				.BuildConfiguration()
				.Build();

			var host = Host.CreateDefaultBuilder()
				.ConfigureServices((context, services) =>
				{
					services.AddTransient(serviceProvider =>
					{
						return builder.GetSection(nameof(AoCSettings)).Get<AoCSettings>()!;
					});
					services.AddTransient<AoCRunner>();
					services.AddTransient<AoCInputReader>();
					services.AddTransient<AoCResultsDisplay>();
				})
				.Build();

			AoCRunner application = host.Services.GetService<AoCRunner>()!;
			application.Run();
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
	}
}