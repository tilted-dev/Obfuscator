using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ATS.Obfuscator.Utils;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace ATS.Obfuscator.Obfuscation
{
    public static class GlobalObfuscation
    {
        public static readonly string SaveDir = Path.Combine(Directory.GetCurrentDirectory(), "Protected");
        private static readonly List<ModuleDefMD2> Modules = new List<ModuleDefMD2>();
        
        public static async Task RunObfuscation(string path, IEnumerable<Type> obfuscations)
        {
            if (!Resolver.IsModuleValid(path, out var moduleDefMd2))
                return;

            var moduleIndex = Modules.FindIndex(m => m.Assembly.Name == moduleDefMd2.Assembly.Name);

            if (moduleIndex != -1)
            {
                Modules[moduleIndex] = moduleDefMd2;
            }
            else
            {
                Modules.Add(moduleDefMd2);
            }
            
            ConsoleWriter.WriteLine("Нанесение водяного знака...", Colors.Purple);
            moduleDefMd2.GlobalType.NestedTypes.Add(new TypeDefUser("ATS.Obfuscator"));

            await obfuscations.ToList()
                .Select(o => Activator.CreateInstance(o, moduleDefMd2) as Abstraction.AbstractObfuscation)
                .OrderBy(u => u?.Uniq)
                .ToList()
                .ForEachAsync(t => t.Start());

            if (!Directory.Exists(SaveDir)) Directory.CreateDirectory(SaveDir);

            var moduleWriterOptions = new ModuleWriterOptions(moduleDefMd2)
            {
                Logger = DummyLogger.NoThrowInstance
            };

            var newPath = SaveDir + "\\" + Path.GetFileName(path);
            
            moduleDefMd2.Write(newPath, moduleWriterOptions);
            ConsoleWriter.WriteComplete($"Обфускация {moduleDefMd2.Name.String} завершена.");
            
            moduleDefMd2.Dispose();
            
            ConsoleWriter.WriteLine(string.Empty, Colors.LemonChiffon);
        }
    }
}