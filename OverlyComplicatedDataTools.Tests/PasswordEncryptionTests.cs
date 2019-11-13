using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using OverlyComplicatedDataTools.Cryptography;

namespace OverlyComplicatedDataTools.Tests
{
    [TestClass]
    public class PasswordEncryptionTests
    {
        [TestMethod]
        public void TestEncryption()
        {
            var cryptography = new LocalMachineCryptography();
            var str = "hello world!";
            var encryptedBytes = cryptography.Encrypt(str);
            var decryptedStr = cryptography.Decrypt(encryptedBytes);
            Assert.AreEqual(str, decryptedStr);
        }
    }
}
