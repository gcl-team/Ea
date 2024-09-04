using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Ea.Standard;

namespace Ea.Demo;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
            });
            builder.SetMinimumLevel(LogLevel.Debug);
        });
    }
}