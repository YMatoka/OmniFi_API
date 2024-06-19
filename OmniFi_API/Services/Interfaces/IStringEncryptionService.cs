namespace OmniFi_API.Services.Interfaces
{
    public interface IStringEncryptionService
    {
 
        public Task<byte[]> EncryptAsync(string clearText, byte[] encryptionKey);

        public Task<string> DecryptAsync(byte[] encrypted, byte[] encryptionKey);

        public byte[] GenerateAesKey();

    }
}
