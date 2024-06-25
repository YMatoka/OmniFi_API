namespace OmniFi_API.Dtos.Banks
{
    public class BankDTO
    {
        public int BankID { get; set; }
        public required string BankName { get; set; }
        public required string ImageUrl { get; set; }
    }
}
