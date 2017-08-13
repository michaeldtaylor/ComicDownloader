using System;
using System.Collections.Generic;
using ComicDownloader.Console.Domain.Providers;
using ManyConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StructureMap;

namespace ComicDownloader.Console
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };

            var container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.AddAllTypesOf(typeof(IComicProvider));
                    x.WithDefaultConventions();
                });
            });

            try
            {
                var commands = StructureMapCommandProvider.FindCommandsInSameAssemblyUsingStructureMap(typeof(Program), container);

                return ConsoleCommandDispatcher.DispatchCommand(commands, args, System.Console.Out);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);

                return -1;
            }
        }
    }
}
