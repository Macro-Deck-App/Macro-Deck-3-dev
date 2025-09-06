using System.Security.Cryptography;

namespace MacroDeck.Server.Application.Utils;

public static class EncryptionUtils
{
	public static byte[] EncryptToBytes(string plainText, byte[] key, byte[] iv)
	{
		using var aes = Aes.Create();
		aes.Key = key;
		aes.IV = iv;

		using var encryptor = aes.CreateEncryptor();
		using var ms = new MemoryStream();
		using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
		using var sw = new StreamWriter(cs);
		sw.Write(plainText);
		sw.Close();
		return ms.ToArray();
	}

	public static string Decrypt(byte[] cipherText, byte[] key, byte[] iv)
	{
		using var aes = Aes.Create();
		aes.Key = key;
		aes.IV = iv;

		using var decryptor = aes.CreateDecryptor();
		using var ms = new MemoryStream(cipherText);
		using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
		using var sr = new StreamReader(cs);
		return sr.ReadToEnd();
	}

	public static byte[] GenerateIv()
	{
		var iv = new byte[16];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(iv);
		return iv;
	}
}
