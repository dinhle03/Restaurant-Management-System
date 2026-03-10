namespace Ecommerce.DTO
{
    public class QrResolveResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = "";
        public int TableId { get; set; }
    }
}
