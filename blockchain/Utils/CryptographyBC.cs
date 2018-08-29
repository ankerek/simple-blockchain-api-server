using System;
using System.Security.Cryptography;
using System.Text;

// cryptography resources https://docs.microsoft.com/en-us/dotnet/standard/security/cryptographic-signatures

namespace DisBlockchain.Utils
{
    public class CryptographyBC
    {
        // generate private, public key pair
        public static Tuple<string, string> GenerateKeyPair()
        {
            string privateKey;
            string publicKey;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    privateKey = Convert.ToBase64String(rsa.ExportCspBlob(true));
                    publicKey = Convert.ToBase64String(rsa.ExportCspBlob(false));
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return new Tuple<string, string>(privateKey, publicKey);
        }

        // generate signature string signed by private key
        public static byte[] GenerateSignature(string privateKey, string data)
        {
            // hash input data
            byte[] hashedData = GenerateSHA256(data);

            // create a rsa and import privateKey
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(Convert.FromBase64String(privateKey));

            //Create a RSAPKCS1SignatureFormatter object and pass it the   
            //RSACryptoServiceProvider to transfer the private key. 
            RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            //Set the hash algorithm to SHA256
            rsaFormatter.SetHashAlgorithm("SHA256");

            //Create a signature for hashedData
            byte[] signedData = rsaFormatter.CreateSignature(hashedData);

            return signedData;
        }

        // verify signature
        public static bool VerifySignature(string publicKey, string data, byte[] signedData)
        {
            // transform publicKey to usable format
            byte[] publicKeyBytes = Convert.FromBase64String(publicKey);

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(publicKeyBytes);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            RSADeformatter.SetHashAlgorithm("SHA256");

            if (RSADeformatter.VerifySignature(GenerateSHA256(data), signedData))
            {
                Console.WriteLine("The signature is valid.");
                return true;
            }

            Console.WriteLine("The signature is not valid.");
            return false;
        }

        public static string GetPublicKeyFromPrivateKey(string privateKey)
        {
            // create a rsa and import privateKey
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(Convert.FromBase64String(privateKey));

            // export public key
            return Convert.ToBase64String(rsa.ExportCspBlob(false));
        }

        // generate sha256 string
        public static string GenerateSHA256String(string inputString)
        {
            byte[] hash = GenerateSHA256(inputString);
            return GetStringFromHash(hash);
        }

        public static byte[] GenerateSHA256(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);

            return hash;
        }

        // transform sha256 hash to string
        public static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
              
    }
}
