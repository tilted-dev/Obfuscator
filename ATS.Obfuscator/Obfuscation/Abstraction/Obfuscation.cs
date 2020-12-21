using System.Threading.Tasks;

namespace ATS.Obfuscator.Obfuscation.Abstraction
{
    public abstract class AbstractObfuscation
    {
        public abstract Task Start();
        public virtual int Uniq => 0;
    }
}