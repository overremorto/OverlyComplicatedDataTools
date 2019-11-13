using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace OverlyComplicatedDataTools.Cryptography
{
    public class LocalMachineCryptography : ICryptography
    {
        private byte[] _entropy;
        public LocalMachineCryptography()
        {
            _entropy = new byte[] { 5, 3, 4, 66, 3, 22, 34, 45, 88, 200, 15, 253, 0 };
        }
        public string Decrypt(byte[] toDecrypt)
        {
            var unprotectedBytes = ProtectedData.Unprotect(toDecrypt, _entropy, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(unprotectedBytes);

        }

        public byte[] Encrypt(string toEncrypt)
        {
            var bytes = Encoding.UTF8.GetBytes(toEncrypt);
            return ProtectedData.Protect(bytes, _entropy, DataProtectionScope.LocalMachine);
        }
    }
}
