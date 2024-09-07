using Microsoft.Extensions.Logging;

namespace Ea.Demo;

sealed class Program
{
    static void Main(string[] args)
    {  
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