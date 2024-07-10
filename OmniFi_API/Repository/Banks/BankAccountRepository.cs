using OmniFi_API.Data;
using OmniFi_DTOs.Dtos.Banks;
using OmniFi_DTOs.Dtos.Cryptos;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Repository.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace OmniFi_API.Repository.Banks
{
    public class BankAccountRepository : BaseRepository<BankAccount>, IBankAccountRepository
    {
        private readonly IStringEncryptionService _stringEncryptionService;
        private readonly IBankCredentialRepository _bankCredentialRepository;
        private readonly IRepository<AesKey> _aesKeyRepository;
        private readonly IRepository<AesIV> _aesIVRepository;

        public BankAccountRepository(
            ApplicationDbContext db,
            IStringEncryptionService stringEncryptionService,
            IBankCredentialRepository bankCredentialRepository,
            IRepository<AesKey> aesKeyRepository,
            IRepository<AesIV> aesIVRepository) : base(db)
        {
            _stringEncryptionService = stringEncryptionService;
            _bankCredentialRepository = bankCredentialRepository;
            _aesKeyRepository = aesKeyRepository;
            _aesIVRepository = aesIVRepository;
        }

        public async Task CreateAsync(BankAccount bankAccount, BankAccountCreateDTO bankAccountCreateDTO)
        {
            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    await base.CreateAsync(bankAccount);

                    var encryptionKey = _stringEncryptionService.GenerateAesKey();
                    var IV = _stringEncryptionService.GenerateAesIV();

                    var credential = new BankCredential()
                    {
                        Password = await _stringEncryptionService.EncryptAsync(bankAccountCreateDTO.Password, encryptionKey, IV),
                        BankAccountID = bankAccount.BankAccountID
                    };

                    await _bankCredentialRepository.CreateAsync(credential);

                    var aesKey = new AesKey()
                    {
                        Key = encryptionKey,
                        BankCredentialId = credential.BankCredientialID
                    };

                    await _aesKeyRepository.CreateAsync(aesKey);

                    var aesIV = new AesIV()
                    {
                        IV = IV,
                        BankCredentialId = credential!.BankCredientialID
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

        public async Task<BankAccount?> GetWithEntitiesAsync(Expression<Func<BankAccount,bool>> filter, bool tracked = false)
        {
            IQueryable <BankAccount> query = _dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            query = query.Where(filter);

            await query
                .Include(x => x.Bank)
                .Include(x => x.BankCredential)
                    .ThenInclude(x => x!.AesIV)
                 .Include(x => x.BankCredential)
                    .ThenInclude(x => x!.AesKey)
                .LoadAsync();

            return await query.FirstOrDefaultAsync();
        }
    }
}
