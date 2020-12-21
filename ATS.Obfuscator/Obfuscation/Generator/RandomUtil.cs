using System;
using System.Collections.Generic;
using System.Linq;

namespace ATS.Obfuscator.Obfuscation.Generator
{
    public static class RandomUtil
    {
        private static readonly char[] Codes = "れづれなるまゝに日暮らし硯にむかひて心にうりゆくよな事を、こはかとなく書きつくればあやうこそものぐるほけれ".ToCharArray();
        private static readonly Random Random = new Random();
        
        public static string RandomString(byte length)
        {
            return new string(Enumerable.Repeat(Codes, length).Select(p => p[Random.Next(0, p.Length)]).ToArray());
        }
        
        public static IEnumerable<int> RandomIntegers(int length, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return Random.Next(0, length);
            }
        }
    }
}