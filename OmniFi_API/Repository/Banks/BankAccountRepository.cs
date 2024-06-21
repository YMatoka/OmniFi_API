using OmniFi_API.Data;
using OmniFi_API.Dtos.Banks;
using OmniFi_API.Dtos.Cryptos;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Repository.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using System.Data;

namespace OmniFi_API.Repository.Banks
{
    public class BankAccountRepository : Repository<BankAccount>, IBankAccountRepository
    {
        private readonly ApplicationDbContext _db;
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
            _db = db;
            _stringEncryptionService = stringEncryptionService;
            _bankCredentialRepository = bankCredentialRepository;
            _aesKeyRepository = aesKeyRepository;
            _aesIVRepository = aesIVRepository;
        }

        public async Task CreateAsync(BankAccount bankAccount, BankAccountCreateDTO bankAccountCreateDTO)
        {
            try
            {
                using (var transaction = _db.Database.BeginTransaction())
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
    }
}
