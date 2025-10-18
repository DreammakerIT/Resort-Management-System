namespace LuxuryResort.Models
{
    public class BankSettings
    {
        public string BankCode { get; set; } // e.g., "VCB" (Vietcombank) per VietQR bank code
        public string AccountNumber { get; set; } // e.g., "0123456789"
        public string AccountName { get; set; } // e.g., "MANCHESTER LUXURY RESORT"
        public string Template { get; set; } = "compact2"; // VietQR template: compact, compact2, etc.
    }
}








