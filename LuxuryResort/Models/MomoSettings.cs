namespace LuxuryResort.Models
{
    public class MomoSettings
    {
        public string Endpoint { get; set; }
        public string PartnerCode { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string RedirectUrl { get; set; }
        public string IpnUrl { get; set; }
        public bool UseFakeGateway { get; set; } = true;
    }
}
