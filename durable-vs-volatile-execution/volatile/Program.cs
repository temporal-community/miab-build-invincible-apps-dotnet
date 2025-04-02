using System;
using System.Threading;
using Microsoft.Extensions.Logging;

public static class Program
{
    public static void Main()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        var logger = loggerFactory.CreateLogger("Program");

        for (int i = 1; i <= 10; i++)
        {
            logger.LogInformation("{Number}", i);
            Thread.Sleep(1000); // Sleep for 1 second (1000 milliseconds)
        }
    }
}