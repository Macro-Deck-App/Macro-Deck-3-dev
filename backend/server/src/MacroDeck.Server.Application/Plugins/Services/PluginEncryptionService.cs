using System.Security.Cryptography;
using Serilog;

namespace MacroDeck.Server.Application.Plugins.Services;

public class PluginEncryptionService : IPluginEncryptionService
{
	private readonly ILogger _logger = Log.ForContext<PluginEncryptionService>();

	public (byte[] publicKey, byte[] privateKey) GenerateKeyPair()
	{
		using var rsa = RSA.Create(2048);
		var publicKey = rsa.ExportRSAPublicKey();
		var privateKey = rsa.ExportRSAPrivateKey();

		_logger.Debug("Generated RSA key pair with 2048-bit key size");
		return (publicKey, privateKey);
	}

	public byte[] GenerateSessionKey()
	{
		var sessionKey = new byte[32]; // 256-bit key for AES
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(sessionKey);

		_logger.Debug("Generated 256-bit AES session key");
		return sessionKey;
	}

	public byte[] EncryptSessionKey(byte[] sessionKey, byte[] clientPublicKey)
	{
		using var rsa = RSA.Create();
		rsa.ImportRSAPublicKey(clientPublicKey, out _);

		var encryptedSessionKey = rsa.Encrypt(sessionKey, RSAEncryptionPadding.OaepSHA256);
		_logger.Debug("Encrypted session key with client's RSA public key");
		return encryptedSessionKey;
	}

	public byte[] DecryptData(byte[] encryptedData, byte[] sessionKey)
	{
		// Extract IV and tag from encrypted data
		// Format: [IV(12 bytes)][Tag(16 bytes)][EncryptedData]
		var iv = new byte[12];
		var tag = new byte[16];
		var cipherText = new byte[encryptedData.Length - 28];

		Array.Copy(encryptedData, 0, iv, 0, 12);
		Array.Copy(encryptedData, 12, tag, 0, 16);
		Array.Copy(encryptedData, 28, cipherText, 0, cipherText.Length);

		using var gcm = new AesGcm(sessionKey, 16);
		var plainText = new byte[cipherText.Length];
		gcm.Decrypt(iv, cipherText, tag, plainText);

		_logger.Debug("Decrypted data using AES-GCM with session key");
		return plainText;
	}

	public byte[] EncryptData(byte[] data, byte[] sessionKey)
	{
		using var gcm = new AesGcm(sessionKey, 16);

		var iv = new byte[12];
		var tag = new byte[16];
		var cipherText = new byte[data.Length];

		RandomNumberGenerator.Fill(iv);
		gcm.Encrypt(iv, data, cipherText, tag);

		// Combine IV, tag and cipher text
		var result = new byte[iv.Length + tag.Length + cipherText.Length];
		Array.Copy(iv, 0, result, 0, iv.Length);
		Array.Copy(tag, 0, result, iv.Length, tag.Length);
		Array.Copy(cipherText, 0, result, iv.Length + tag.Length, cipherText.Length);

		_logger.Debug("Encrypted data using AES-GCM with session key");
		return result;
	}
}
