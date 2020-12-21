using System.Linq;
using dnlib.DotNet;

namespace ATS.Obfuscator.Utils.DnUtils.CIL
{
    internal static class InstructionUtils
    {
        public static bool CanObfuscate(this MethodDef methodDef, bool useNameFilter)
        {
            if (!methodDef.HasBody) return false;
            if (!methodDef.Body.HasInstructions) return false;
            if (useNameFilter && methodDef.Name == "Main") return false;
            return !methodDef.DeclaringType.IsGlobalModuleType && methodDef.CustomAttributes.All(a => a.TypeFullName != "DisableObfuscation");
        }
    }
}
