using System.Threading.Tasks;
using System.Windows.Media;
using ATS.Obfuscator.Utils;
using dnlib.DotNet;

namespace ATS.Obfuscator.Obfuscation.Protections
{
    public class NamespaceProtection : Abstraction.AbstractObfuscation
    {
        public override int Uniq => 3;
        private readonly ModuleDefMD2 _moduleDefMd2;
        
        public NamespaceProtection(ModuleDefMD2 moduleDefMd2)
        {
            _moduleDefMd2 = moduleDefMd2;
        }
        
        public override Task Start()
        {
            ConsoleWriter.WriteLine("Удаление пространства имен...", Colors.MidnightBlue);
            foreach (var typeDef in _moduleDefMd2.GetTypes())
            {
                typeDef.Namespace = "";
            }
            
            return Task.CompletedTask;
        }
    }
}