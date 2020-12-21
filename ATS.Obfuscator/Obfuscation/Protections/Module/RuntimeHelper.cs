using System;
using System.Text;

namespace ATS.Obfuscator.Obfuscation.Protections.Module
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public static class RuntimeHelper
    {
        public static string Decrypt(string value)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }
    }
}