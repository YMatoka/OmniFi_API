using Microsoft.EntityFrameworkCore;
using OmniFi_API.Data;
using OmniFi_DTOs.Dtos.Cryptos;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using System.Linq.Expressions;
using System.Linq;
using OmniFi_API.Models.Banks;

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

        public async Task<CryptoExchangeAccount?> GetWithEntitiesAsync(Expression<Func<CryptoExchangeAccount, bool>> filter, bool tracked = false)
        {
            IQueryable<CryptoExchangeAccount> query = _dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            query = query.Where(filter);

            await query
                .Include(x => x.CryptoExchange)
                .Include(x => x.CryptoApiCredential)
                    .ThenInclude(x => x!.AesIV)
                .Include(x => x.CryptoApiCredential)
                    .ThenInclude(x => x!.AesKey)
                .LoadAsync();

            return query.FirstOrDefault();
        }

        public async Task UpdateAsync(
            CryptoExchangeAccount cryptoExchangeAccount,
            CryptoExchangeAccountUpdateDTO cryptoExchangeAccountUpdateDTO)
        {
            try
            {
                if (cryptoExchangeAccountUpdateDTO.ApiKey is not null)
                {
                    cryptoExchangeAccount.CryptoApiCredential!.ApiKey = await _stringEncryptionService.EncryptAsync(
                        cryptoExchangeAccountUpdateDTO.ApiKey,
                        cryptoExchangeAccount!.CryptoApiCredential!.AesKey!.Key,
                        cryptoExchangeAccount!.CryptoApiCredential!.AesIV!.IV);
                }

                if (cryptoExchangeAccountUpdateDTO.ApiSecret is not null)
                {
                    cryptoExchangeAccount.CryptoApiCredential!.ApiSecret = await _stringEncryptionService.EncryptAsync(
                        cryptoExchangeAccountUpdateDTO.ApiSecret,
                        cryptoExchangeAccount!.CryptoApiCredential!.AesKey!.Key,
                        cryptoExchangeAccount!.CryptoApiCredential!.AesIV!.IV);
                }

                await _cryptoApiCredentialRepository.UpdateAsync(cryptoExchangeAccount.CryptoApiCredential!);
            }
            catch (Exception)
            {
                throw;
            }


        }
    }
}

