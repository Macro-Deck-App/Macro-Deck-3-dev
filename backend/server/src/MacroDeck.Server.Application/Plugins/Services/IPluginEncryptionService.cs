namespace MacroDeck.Server.Application.Plugins.Services;

public interface IPluginEncryptionService
{
	(byte[] publicKey, byte[] privateKey) GenerateKeyPair();
	byte[] GenerateSessionKey();
	byte[] EncryptSessionKey(byte[] sessionKey, byte[] clientPublicKey);
	byte[] DecryptData(byte[] encryptedData, byte[] sessionKey);
	byte[] EncryptData(byte[] data, byte[] sessionKey);
}