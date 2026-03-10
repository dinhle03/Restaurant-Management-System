using Newtonsoft.Json; // Đảm bảo có dòng này

namespace Ecommerce.DTO
{
    public class MomoCreatePaymentResponseModel
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("resultCode")]
        public int ResultCode { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("localMessage")]
        public string LocalMessage { get; set; }

        [JsonProperty("requestType")]
        public string RequestType { get; set; }

        [JsonProperty("payUrl")] // Rất quan trọng để không bị null
        public string PayUrl { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("qrCodeUrl")]
        public string QrCodeUrl { get; set; }

        [JsonProperty("deeplink")]
        public string Deeplink { get; set; }
    }
}