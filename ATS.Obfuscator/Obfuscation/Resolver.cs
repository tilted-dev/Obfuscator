using System.IO;
using System.Windows.Media;
using ATS.Obfuscator.Utils;
using dnlib.DotNet;

namespace ATS.Obfuscator.Obfuscation
{
    public static class Resolver
    {
        public static bool IsModuleValid(string path, out ModuleDefMD2 moduleDefMd2)
        {
            try
            {
                var flag = true;
                moduleDefMd2 = ModuleDefMD.Load(path);
                
                var asmResolver = new AssemblyResolver();
                var modCtx = new ModuleContext(asmResolver);
                asmResolver.DefaultModuleContext = modCtx;
                asmResolver.EnableTypeDefCache = true;
                asmResolver.DefaultModuleContext = new ModuleContext(asmResolver);
                
                ConsoleWriter.WriteLine($"Начата обфускация {moduleDefMd2.Name}.", Colors.Indigo);
                
                foreach (var dependency in moduleDefMd2.GetAssemblyRefs())
                {
                    var assembly = asmResolver.Resolve(dependency, moduleDefMd2);
                    
                    if (assembly != null)
                    {
                        ConsoleWriter.WriteLine($"Распознана библиотека: {assembly.Name.String}.", Colors.DarkMagenta);
                        continue;
                    }
                    
                    ConsoleWriter.WriteError($"Невозможно распознать библиотеку: {dependency.Name.String}.");
                    flag = false;
                }

                if (!flag)
                {
                    ConsoleWriter.WriteError("Обфускация отменена.");
                    return false;
                }
            }
            catch
            {
                moduleDefMd2 = null;
                ConsoleWriter.WriteError($"Произошла ошибка во время распознавания модуля {Path.GetFileName(path)}.");
                return false;
            }

            return true;
        }
    }
}
