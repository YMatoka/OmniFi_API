namespace OmniFi_API.Options.Banks
{
    public class GocardlessBankInfoOptions
    {
        public static string SectionName => nameof(GocardlessBankInfoOptions);

        public Dictionary<string,BankInfo> BankInfos { get; set; } = new();

    }

    public class BankInfo
    {
        public string InstitutionId { get; set; } = string.Empty;
        public double AccessDurationInDays { get; set; } = default;
        public double TransactionTotalDays { get; set; } = default;

    }
}
