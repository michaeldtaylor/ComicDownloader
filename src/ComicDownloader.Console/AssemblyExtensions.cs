using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ComicDownloader.Console
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static object ConvertTo(this object input, Type targetType)
        {
            var sourceType = input.GetType();
            var converter = TypeDescriptor.GetConverter(targetType);
            if (converter.CanConvertFrom(sourceType))
            {
                return TypeDescriptor.GetConverter(targetType).ConvertFrom(input);
            }

            if (sourceType == targetType)
            {
                return input;
            }

            return Convert.ChangeType(input, targetType);
        }
    }
}