using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ATS.Obfuscator.Obfuscation.Generator;
using ATS.Obfuscator.Utils;
using ATS.Obfuscator.Utils.DnUtils.CIL;
using dnlib.DotNet;

namespace ATS.Obfuscator.Obfuscation.Protections
{
    public class ModuleProtection : Abstraction.AbstractObfuscation
    {
        public override int Uniq => 5;
        private readonly ModuleDefMD2 _moduleDefMd2;
        
        public ModuleProtection(ModuleDefMD2 moduleDefMd2)
        {
            _moduleDefMd2 = moduleDefMd2;
        }
        public override Task Start()
        {
            ConsoleWriter.WriteLine("Шифрование модуля...", Colors.Peru);
            foreach (var type in _moduleDefMd2.Types.ToList())
            {
                foreach (var method in type.Methods.Where(m => !m.IsConstructor && !m.IsVirtual))
                {
                    if(!method.CanObfuscate(true) || method.Name.StartsWith("get_")) continue;
                    
                    method.Name = RandomUtil.RandomString(20);
                    
                    foreach (var methodParameter in method.Parameters)
                    {
                        methodParameter.Name = RandomUtil.RandomString(10);
                    }
                }

                if (!type.IsGlobalModuleType)
                {
                    type.Name = RandomUtil.RandomString(20);
                }
            }
            
            return Task.CompletedTask;
        }
    }
}