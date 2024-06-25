using Microsoft.EntityFrameworkCore;
using OmniFi_API.Data;
using OmniFi_API.Dtos.Cryptos;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;

namespace OmniFi_API.Repository.Cryptos
{
    public class CryptoExchangeAccountRepository : BaseRepository<CryptoExchangeAccount>, ICryptoExchangeAccountRepository
    {
        private readonly IStringEncryptionService _stringEncryptionService;
        private readonly ICryptoApiCredentialRepository _cryptoApiCredentialRepository;
        private readonly IRepository<AesKey> _aesKeyRepository;
        private readonly IRepository<AesIV> _aesIVRepository;

        public CryptoExchangeAccountRepository(
            ApplicationDbContext db,
            IStringEncryptionService stringEncryptionService,
            ICryptoApiCredentialRepository cryptoApiCredentialRepository,
            IRepository<AesKey> aesKeytRepository,
            IRepository<AesIV> aesIVRepository) : base(db)
        {
            _stringEncryptionService = stringEncryptionService;
            _cryptoApiCredentialRepository = cryptoApiCredentialRepository;
            _aesKeyRepository = aesKeytRepository;
            _aesIVRepository = aesIVRepository;
        }

        public async Task CreateAsync(CryptoExchangeAccount cryptoExchangeAccount, CryptoExchangeAccountCreateDTO cryptoExchangeAccountCreateDTO)
        {

            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {

                    await base.CreateAsync(cryptoExchangeAccount);

                    var encryptionKey = _stringEncryptionService.GenerateAesKey();
                    var IV = _stringEncryptionService.GenerateAesIV();

                    var credential = new CryptoApiCredential()
                    {
                        ApiKey = await _stringEncryptionService.EncryptAsync(cryptoExchangeAccountCreateDTO.ApiKey, encryptionKey, IV),
                        ApiSecret = await _stringEncryptionService.EncryptAsync(cryptoExchangeAccountCreateDTO.ApiSecret, encryptionKey, IV),
                        CryptoExchangeAccountID = cryptoExchangeAccount.ExchangeAccountID,
                    };

                    await _cryptoApiCredentialRepository.CreateAsync(credential);

                    var aesKey = new AesKey()
                    {
                        Key = encryptionKey,
                        CryptoApiCredentialId = credential.CryptoApiCredentialID
                    };

                    await _aesKeyRepository.CreateAsync(aesKey);

                    var aesIV = new AesIV()
                    {
                        IV = IV,
                        CryptoApiCredentialId = credential!.CryptoApiCredentialID
                    };

                    await _aesIVRepository.CreateAsync(aesIV);

                    await transaction.CommitAsync();

                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

