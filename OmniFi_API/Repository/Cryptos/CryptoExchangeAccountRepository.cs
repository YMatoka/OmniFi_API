using Microsoft.EntityFrameworkCore;
using OmniFi_API.Data;
using OmniFi_API.Dtos.Cryptos;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;

namespace OmniFi_API.Repository.Cryptos
{
    public class CryptoExchangeAccountRepository : Repository<CryptoExchangeAccount>, ICryptoExchangeAccountRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IStringEncryptionService _stringEncryptionService;
        private readonly ICryptoApiCredentialRepository _cryptoApiCredentialRepository;
        private readonly IRepository<AesKey> _aesKeytRepository;

        public CryptoExchangeAccountRepository(
            ApplicationDbContext db,
            IStringEncryptionService stringEncryptionService,
            ICryptoApiCredentialRepository cryptoApiCredentialRepository,
            IRepository<AesKey> aesKeytRepository) : base(db)
        {
            _db = db;
            _stringEncryptionService = stringEncryptionService;
            _cryptoApiCredentialRepository = cryptoApiCredentialRepository;
            _aesKeytRepository = aesKeytRepository;
        }

        public async Task CreateAsync(CryptoExchangeAccount cryptoExchangeAccount, CryptoExchangeAccountCreateDTO cryptoExchangeAccountCreateDTO)
        {

                try
                {
                    await base.CreateAsync(cryptoExchangeAccount);

                    var savedCryptoExchangeAccount = await base.GetAsync(
                        (x) => x.UserID == cryptoExchangeAccount.UserID
                        && x.CryptoExchangeID == cryptoExchangeAccount.CryptoExchangeID,
                        tracked: false);

                    var encryptionKey = _stringEncryptionService.GenerateAesKey();

                    var credential = new CryptoApiCredential()
                    {
                        ApiKey = await _stringEncryptionService.EncryptAsync(cryptoExchangeAccountCreateDTO.ApiKey, encryptionKey),
                        ApiSecret = await _stringEncryptionService.EncryptAsync(cryptoExchangeAccountCreateDTO.ApiSecret, encryptionKey),
                        CryptoExchangeAccountID = cryptoExchangeAccount!.ExchangeAccountID,
                    };

                    await _cryptoApiCredentialRepository.CreateAsync(credential);

                    credential = await _cryptoApiCredentialRepository.GetAsync(
                        (x) => x.CryptoExchangeAccountID == cryptoExchangeAccount.ExchangeAccountID,
                        tracked: false);

                    var aesKey = new AesKey()
                    {
                        Key = encryptionKey,
                        CryptoApiCredentialId = credential!.CryptoApiCredentialID
                    };

                    await _aesKeytRepository.CreateAsync(aesKey);

                    _db.Database.CommitTransaction();

                }
                catch (Exception)
                {

                    _db.Database.RollbackTransaction();
                    throw;
                }
            }
        }
    }

