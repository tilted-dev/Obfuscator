using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using ATS.Obfuscator.Obfuscation.Generator;
using ATS.Obfuscator.Utils;
using ATS.Obfuscator.Utils.DnUtils.CIL;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ATS.Obfuscator.Obfuscation.Protections
{
    public class ConstantProtection : Abstraction.AbstractObfuscation
    {
        public override int Uniq => 2;
        
        private readonly ModuleDefMD2 _moduleDefMd2;
        
        public ConstantProtection(ModuleDefMD2 moduleDefMd2)
        {
            _moduleDefMd2 = moduleDefMd2;
        }

        private static void MeltString(MethodDef methodDef)
        {
            if (!methodDef.CanObfuscate(false)) return;
            
            foreach(var instruction in methodDef.Body.Instructions)
            {
                if (instruction.OpCode != OpCodes.Ldstr) continue;
                
                MethodDef newMethod = new MethodDefUser(RandomUtil.RandomString(25), MethodSig.CreateStatic(methodDef.DeclaringType.Module.CorLibTypes.String), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig) { Body = new CilBody() };
                newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ldstr, instruction.Operand.ToString()));
                newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ret));
                methodDef.DeclaringType.Methods.Add(newMethod);
                instruction.OpCode = OpCodes.Call;
                instruction.Operand = newMethod;
            }
        }
        
        private static void MeltInteger(MethodDef methodDef)
        {
            if (!methodDef.CanObfuscate(false)) return;
            
            foreach(var instruction in methodDef.Body.Instructions)
            {
                if (instruction.OpCode != OpCodes.Ldc_I4) continue;
                MethodDef newMethod = new MethodDefUser(RandomUtil.RandomString(25), MethodSig.CreateStatic(methodDef.DeclaringType.Module.CorLibTypes.Int32), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig) { Body = new CilBody() };
                newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I4, instruction.GetLdcI4Value()));
                newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ret));
                methodDef.DeclaringType.Methods.Add(newMethod);
                instruction.OpCode = OpCodes.Call;
                instruction.Operand = newMethod;
            }
        }
        
        public override Task Start()
        {
            ConsoleWriter.WriteLine("Шифрование констант...", Colors.Brown);
            foreach (var typeDef in _moduleDefMd2.GetTypes())
            {
                foreach (var typeDefMethod in typeDef.Methods.ToList())
                {
                    MeltString(typeDefMethod);
                    MeltInteger(typeDefMethod);
                }
            }
            
            return Task.CompletedTask;
        }
    }
}