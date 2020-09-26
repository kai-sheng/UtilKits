using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using System;

namespace UtilKits.Reflection
{
    public class AssemblyHelper
    {
        /// <summary>
        /// 取得單一的 Assembly
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns></returns>
        public static Assembly GetAssembly(string assemblyName)
        {
            return GetAssemblies(assemblyName, new string[] { }).First();
        }

        /// <summary>
        /// 取得全部的 Assembly
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies(string assemblyName)
        {
            return GetAssemblies(assemblyName, new string[] { });
        }

        /// <summary>
        /// 取得全部的 Assembly 
        /// </summary>
        /// <param name="assemblyName">Assembly 名稱</param>
        /// <param name="excludedName">排除名稱</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies(string assemblyName, string[] excludedName)
        {
            var dependencies = DependencyContext.Default.RuntimeLibraries
                                                .Where(lib =>
                                                       lib.Name == assemblyName ||
                                                       lib.Name.StartsWith(assemblyName, System.StringComparison.Ordinal));

            foreach (var library in dependencies)
            {
                if (excludedName.Any(e => string.Compare(e, library.Name, true) == 0)) continue;

                yield return Assembly.Load(new AssemblyName(library.Name));
            }

        }
    }
}
