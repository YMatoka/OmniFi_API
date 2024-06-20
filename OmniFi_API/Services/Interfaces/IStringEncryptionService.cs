namespace OmniFi_API.Services.Interfaces
{
    public interface IStringEncryptionService
    {
        public Task<byte[]> EncryptAsync(string clearText, byte[] encryptionKey, byte[] IV);
        public Task<string> DecryptAsync(byte[] encrypted, byte[] encryptionKey, byte[] IV);
        public byte[] GenerateAesKey();
        public byte[] GenerateAesIV();
    }
}
