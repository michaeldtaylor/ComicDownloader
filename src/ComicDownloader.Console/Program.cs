using System.Collections.Generic;
using ManyConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ComicDownloader.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };

            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
            var result = ConsoleCommandDispatcher.DispatchCommand(commands, args, System.Console.Out);

            System.Console.WriteLine("Press any key to continue...");
            System.Console.ReadLine();

            return result;
        }
    }
}
