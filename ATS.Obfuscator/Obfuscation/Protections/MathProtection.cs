using System;
using System.Collections.Generic;
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
    public class MathProtection : Abstraction.AbstractObfuscation
    {
        public override int Uniq => 1;
        private readonly ModuleDefMD2 _moduleDefMd2;
        
        public MathProtection(ModuleDefMD2 moduleDefMd2)
        {
            _moduleDefMd2 = moduleDefMd2;
        }
        
        private static IEnumerable<Instruction> CreateMathInstructions(int value)
        {
            var instructions = new List<Instruction>();
            var nums = RandomUtil.RandomIntegers(100000, 4).ToArray();
            
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, value - nums[0] + nums[1] - nums[2] + nums[3]));
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, nums[0]));
            instructions.Add(Instruction.Create(OpCodes.Sub));
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, nums[1]));
            instructions.Add(Instruction.Create(OpCodes.Add));
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, nums[2]));
            instructions.Add(Instruction.Create(OpCodes.Sub));
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, nums[3]));
            instructions.Add(Instruction.Create(OpCodes.Add));
            return instructions;
        }

        public override Task Start()
        {
            ConsoleWriter.WriteLine("Шифрование чисел...", Colors.Coral);
            foreach (var typeDef in _moduleDefMd2.GetTypes())
            {
                foreach (var typeDefMethod in typeDef.Methods)
                {
                    if (!typeDefMethod.CanObfuscate(false)) continue;

                    var bodyInstructions = typeDefMethod.Body.Instructions;
                    
                    for (var i = 0; i < bodyInstructions.Count; i++)
                    {
                        var instruction = typeDefMethod.Body.Instructions[i];

                        if (!(instruction.Operand is int)) continue;
                        
                        var instructions = CreateMathInstructions(Convert.ToInt32(instruction.Operand));

                        foreach (var instr in instructions)
                        {
                            typeDefMethod.Body.Instructions.Insert(i + 1, instr);
                            i++;
                        }
                    }
                }
            }
            
            return Task.CompletedTask;
        }
    }
}