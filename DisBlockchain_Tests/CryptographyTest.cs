using System;
using DisBlockchain.Utils;
using Xunit;

namespace DisBlockchainTests
{
    public class CryptographyTest
    {
        public CryptographyTest()
        {
        }

        [Fact]
        public void SignatureVerification()
        {
            Tuple<string, string> keys = Cryptography.GenerateKeyPair();
            Tuple<string, string> keys2 = Cryptography.GenerateKeyPair();
            string data = "data";
            byte[] signedData = Cryptography.GetSignature(keys.Item1, data);

            Assert.True(Cryptography.VerifySignature(keys.Item2, data, signedData));
            Assert.False(Cryptography.VerifySignature(keys2.Item2, data, signedData));
        }
    }
}
