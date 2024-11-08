﻿using OmniFi_API.Models.Api.Banks;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;

namespace OmniFi_API.Models.Encryption
{
    public class AesKey
    {
        public int AesKeyId { get; set; }
        public required byte[] Key { get; set; }
        public int? BankDataApiCredentialId { get; set; }
        public BankDataApiCredential? BankDataApiCredential { get; set; }
        public int? CryptoApiCredentialId { get; set; }
        public CryptoApiCredential? CryptoApiCredential { get; set; }
    }
}
