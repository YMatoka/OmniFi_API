namespace OmniFi_API.Models
{
    public class Banks
    {
        public int BankID { get; set; }
        public required string BankName { get; set; }
        public byte BankLogo { get; set; }
        public int APIEndPointBalance { get; set; }
    }
}
