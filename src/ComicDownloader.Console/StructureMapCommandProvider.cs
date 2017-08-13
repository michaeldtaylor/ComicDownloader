using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ManyConsole;
using StructureMap;

namespace ComicDownloader.Console
{
    public static class StructureMapCommandProvider
    {
        public static IEnumerable<ConsoleCommand> FindCommandsInSameAssemblyUsingStructureMap(Type typeInSameAssembly, IContainer container)
        {
            if (typeInSameAssembly == null)
            {
                throw new ArgumentNullException(nameof(typeInSameAssembly));
            }

            return FindCommandsInAssembly(typeInSameAssembly.Assembly, container);
        }

        private static IEnumerable<ConsoleCommand> FindCommandsInAssembly(Assembly assembly, IContainer container)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var commandTypes = assembly.GetLoadableTypes()
                .Where(t => t.IsSubclassOf(typeof(ConsoleCommand)))
                .Where(t => !t.IsAbstract)
                .OrderBy(t => t.FullName);

            return commandTypes.Select(commandType => (ConsoleCommand)container.GetInstance(commandType)).ToList();
        }
    }
}