using System;

namespace OverlyComplicatedDataTools.Cryptography
{
    public interface ICryptography
    {
        byte[] Encrypt(string toEncrypt);
        string Decrypt(byte[] toDecrypt);
    }
}
