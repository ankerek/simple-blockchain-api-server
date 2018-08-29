using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

// cryptography utility methods
// cryptography keys and verification using ECDSA secp256k1 algorithm
// dependent on BouncyCastle library

namespace DisBlockchain.Utils
{
    public class Cryptography
    {
        static string algorithm = "ECDSA";
        static X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");
        static ECDomainParameters domainParameters = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

        // generate private, public key pair
        public static Tuple<string, string> GenerateKeyPair()
        {
            ECKeyPairGenerator gen = new ECKeyPairGenerator(algorithm);
            ECKeyGenerationParameters keyGenParam = new ECKeyGenerationParameters(domainParameters, new SecureRandom());
            gen.Init(keyGenParam);
            AsymmetricCipherKeyPair keys = gen.GenerateKeyPair();

            ECPrivateKeyParameters privateKey = (ECPrivateKeyParameters)(keys.Private);
            ECPublicKeyParameters publicKey = (ECPublicKeyParameters)(keys.Public);

            //Console.WriteLine(ToHexString(publicKey.Q.GetEncoded()));
            //Console.WriteLine(GetPublicKeyFromPrivateKey(ToHexString(privateKey.D.ToByteArray())));

            //Console.WriteLine(ToHexString(privateKey.D.ToByteArray()));
            //Console.WriteLine(privateKey.D.ToString(16));

            return new Tuple<string, string>(ToHexString(privateKey.D.ToByteArray()), ToHexString(publicKey.Q.GetEncoded()));
        }

        // get signature signed by privateKey
        public static byte[] GetSignature(string privateKey, string data)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] inputData = encoder.GetBytes(data);
            ECPrivateKeyParameters privateKeyParams = GetPrivateKeyParamsFromString(privateKey);

            ISigner signer = SignerUtilities.GetSigner(algorithm);
            signer.Init(true, privateKeyParams);
            signer.BlockUpdate(inputData, 0, inputData.Length);

            return signer.GenerateSignature();
        }

        // verify signature by public key
        public static bool VerifySignature(string publicKey, string data, byte[] signedData)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] inputData = encoder.GetBytes(data);
            ECPublicKeyParameters publicKeyParams = GetPublicKeyParamsFromString(publicKey);


            var signer = SignerUtilities.GetSigner(algorithm);
            signer.Init(false, publicKeyParams);
            signer.BlockUpdate(inputData, 0, inputData.Length);

            return signer.VerifySignature(signedData);
        }

        public static ECPrivateKeyParameters GetPrivateKeyParamsFromString(string privateKey)
        {
            return new ECPrivateKeyParameters(algorithm, HexStringToBigInteger(privateKey), domainParameters);
        }

        public static ECPublicKeyParameters GetPublicKeyParamsFromString(string publicKey)
        {
            byte[] publicKeyBytes = HexStringToByteArray(publicKey);
            byte[] x = publicKeyBytes.Skip(1).Take(32).ToArray();
            byte[] y = publicKeyBytes.Skip(33).ToArray();

            BigInteger xbi = new BigInteger(1, x);
            BigInteger ybi = new BigInteger(1, y);

            ECPublicKeyParameters publicKeyParams = new ECPublicKeyParameters(algorithm, curve.Curve.CreatePoint(xbi, ybi), domainParameters);

            return publicKeyParams;
        }

        public static string GetPublicKeyFromPrivateKey(string privateKey)
        {
            byte[] privateKeyBytes = HexStringToByteArray(privateKey);
            Org.BouncyCastle.Math.EC.ECPoint q = domainParameters.G.Multiply(new BigInteger(privateKeyBytes));

            return ToHexString(q.GetEncoded());
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
                result.Append(hash[i].ToString("x2"));
            }
            return result.ToString();
        }

        // convert bytes array to hex string
        public static string ToHexString(IEnumerable<byte> bytes)
        {
            string output = string.Empty;
            foreach (byte b in bytes)
            {
                output += b.ToString("x2");
            }
            return output;
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        // convert hex to big integer
        public static BigInteger HexStringToBigInteger(string hex)
        {
            return new BigInteger(hex, 16);
        }
              
    }
}
