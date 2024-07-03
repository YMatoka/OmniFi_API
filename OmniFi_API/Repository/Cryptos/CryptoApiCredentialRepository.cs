using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using OmniFi_API.Data;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Interfaces;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace OmniFi_API.Repository.Cryptos
{
    public class CryptoApiCredentialRepository : BaseRepository<CryptoApiCredential>, ICryptoApiCredentialRepository
    {
        private readonly IStringEncryptionService _stringEncryptionService;

        public CryptoApiCredentialRepository(
            ApplicationDbContext db, 
            IStringEncryptionService stringEncryptionService) : base(db)
        {
            _stringEncryptionService = stringEncryptionService;
        }

        public override async Task<CryptoApiCredential?> GetAsync(Expression<Func<CryptoApiCredential, bool>>? filter = null, string? includeProperties = null, bool tracked = true)
        {
            return await base.GetAsync(filter, includeProperties, tracked);
        }

        public async Task UpdateAsync(CryptoApiCredential cryptoApiCredential)
        {
            db.Update(cryptoApiCredential);
            await SaveAsync();
        }

    }
}
 