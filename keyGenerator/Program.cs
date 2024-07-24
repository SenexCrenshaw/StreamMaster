using StreamMaster.Domain.Crypto;

namespace keyGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string key = KeyGenerator.GenerateKey();
            string key64 = KeyGenerator.GenerateKey(64);
            string key32 = KeyGenerator.GenerateKey(32);
            string key16 = KeyGenerator.GenerateKey(16);
            Console.WriteLine(key);
            Console.WriteLine(key64);
            Console.WriteLine(key32);
            Console.WriteLine(key16);
        }
    }
}
