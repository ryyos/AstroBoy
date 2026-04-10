using System;
using System.Security.Cryptography;
using System.Text;

namespace AstroBoy.Utils;

public static class Encrypts
{
    public static string Md5Hash(string input)
    {
        var encoding = Encoding.UTF8;
        int byteCount = encoding.GetByteCount(input);
        
        Span<byte> bytes = byteCount <= 256 ? stackalloc byte[byteCount] : new byte[byteCount];
        encoding.GetBytes(input, bytes);
        Span<byte> hashBytes = stackalloc byte[MD5.HashSizeInBytes];
        using var md5 = MD5.Create();
        if (!md5.TryComputeHash(bytes, hashBytes, out _))
            throw new InvalidOperationException("Hash computation failed");
        
        Span<char> result = stackalloc char[hashBytes.Length * 2];
        for (int i = 0; i < hashBytes.Length; i++)
            hashBytes[i].TryFormat(result.Slice(i * 2), out _, "x2");
        return new string(result);
    }
}
