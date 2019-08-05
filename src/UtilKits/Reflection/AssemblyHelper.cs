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
            return GetAssemblies(assemblyName, new string[] { }).Single();
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
            var assemblyNames = new List<string>();
            var dependencies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(lib => lib.FullName == assemblyName || lib.FullName.StartsWith(assemblyName));

            foreach (var library in dependencies)
            {
                assemblyNames.Add(library.FullName);
                assemblyNames.AddRange(library.GetReferencedAssemblies()
                    .Where(lib => lib.FullName == assemblyName || lib.FullName.StartsWith(assemblyName))
                    .Select(lib => lib.FullName));
            }

            foreach (var name in assemblyNames.Distinct())
            {
                if (excludedName.Any(e => string.Compare(e, name, true) == 0)) continue;

                yield return Assembly.Load(new AssemblyName(name));
            }

        }
    }
}
