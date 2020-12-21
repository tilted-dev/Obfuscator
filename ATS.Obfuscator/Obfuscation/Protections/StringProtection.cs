using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ATS.Obfuscator.Obfuscation.Abstraction;
using ATS.Obfuscator.Obfuscation.Protections.Module;
using ATS.Obfuscator.Utils;
using ATS.Obfuscator.Utils.DnUtils;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using TypeAttributes = dnlib.DotNet.TypeAttributes;

namespace ATS.Obfuscator.Obfuscation.Protections
{
    public class StringProtection : AbstractObfuscation
    {
        public override int Uniq => 4;
        private readonly ModuleDefMD2 _moduleDefMd2;
        
        public StringProtection(ModuleDefMD2 module)
        {
            _moduleDefMd2 = module;
        }

        private static string Encrypt(string source)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
        }
        
        private static MethodDef Inject(ModuleDef asmDef)
        {
            var typeModule = ModuleDefMD.Load(typeof(RuntimeHelper).Module);
            var typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(RuntimeHelper).MetadataToken));
            TypeDef typeDefUser = new TypeDefUser("Obfuscator", asmDef.CorLibTypes.Object.TypeDefOrRef);
            typeDefUser.Attributes = TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Class | TypeAttributes.AnsiClass;
            asmDef.Types.Add(typeDefUser);
            var members = InjectHelper.Inject(typeDef, typeDefUser, asmDef);
            var init = (MethodDef)members.Single(m => m.Name == "Decrypt");
            return init;
        }

        public override Task Start()
        {
            ConsoleWriter.WriteLine("Шифрование строк...", Colors.Chocolate);
            var decryptMethod = Inject(_moduleDefMd2);

            foreach (var method in from type in _moduleDefMd2.Types.ToList() from method in type.Methods where method.Body != null where method != decryptMethod select method)
            {
                for (var i = 0; i < method.Body.Instructions.Count; i++)
                {
                    if (method.Body.Instructions[i].OpCode != OpCodes.Ldstr) continue;
                        
                    var oldStr = method.Body.Instructions[i].Operand.ToString();
                    method.Body.Instructions[i].Operand = Encrypt(oldStr);
                    method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, decryptMethod));
                    i++;
                }

                method.Body.OptimizeMacros();
                method.Body.OptimizeBranches();
            }
            
            return Task.CompletedTask;
        }
        
    }
}