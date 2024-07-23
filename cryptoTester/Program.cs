namespace cryptoTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string key1 = "serverKey";
            string key2 = "groupKey";

            string originalText = "Hello, World!";
            Console.WriteLine($"Original: {originalText}");

            // First encryption
            string encrypted1 = AesEncryption.Encrypt(originalText, key1);
            Console.WriteLine($"Encrypted once: {encrypted1}");

            // Decrypt second encryption
            string decrypted13 = AesEncryption.Decrypt(encrypted1, key1);
            Console.WriteLine($"Decrypted once: {decrypted13}");

            // Second encryption
            string encrypted2 = AesEncryption.Encrypt(encrypted1, key2);
            Console.WriteLine($"Encrypted twice: {encrypted2}");

            // Decrypt second encryption
            string decrypted1 = AesEncryption.Decrypt(encrypted2, key2);
            Console.WriteLine($"Decrypted once: {decrypted1}");

            // Decrypt first encryption
            string decrypted2 = AesEncryption.Decrypt(decrypted1, key1);
            Console.WriteLine($"Decrypted twice: {decrypted2}");

        }
    }
}
