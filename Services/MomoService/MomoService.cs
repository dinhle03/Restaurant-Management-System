using Ecommerce.DTO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Services.MomoService
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        private readonly HttpClient _httpClient;

        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
            _httpClient = new HttpClient();
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model)
        {
            string orderId = model.OrderId.ToString() + "-" + DateTime.Now.Ticks.ToString();
            string orderInfoText = string.IsNullOrWhiteSpace(model.OrderInfomation) ? $"Thanh toan don hang {orderId}" : model.OrderInfomation;
            string finalReturnUrl = string.IsNullOrEmpty(model.ReturnUrl) ? _options.Value.ReturnUrl : model.ReturnUrl;
            long amountLong = Convert.ToInt64(model.Amount);
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";
            string requestType = _options.Value.RequestType;
            string lang = "vi";

            var rawData =
                $"accessKey={_options.Value.AccessKey}" +
                $"&amount={amountLong}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={_options.Value.NotifyUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfoText}" +
                $"&partnerCode={_options.Value.PartnerCode}" +
                $"&redirectUrl={finalReturnUrl}" +
                $"&requestId={requestId}" +
                $"&requestType={requestType}";


            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            // 3. Cấu hình RestClient với UserAgent để qua mặt Nginx 403
            var client = new RestClient(_options.Value.MomoApiUrl)
            {
                Timeout = 30000,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)"
            };

            var request = new RestRequest("", Method.POST);

            // 4. Body JSON
            var requestData = new
            {
                partnerCode = _options.Value.PartnerCode,
                requestId = requestId,
                amount = amountLong,
                orderId = orderId,
                orderInfo = orderInfoText,
                redirectUrl = finalReturnUrl,
                ipnUrl = _options.Value.NotifyUrl,
                requestType = requestType,
                extraData = extraData,
                lang = lang,
                signature = signature
            };

            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddJsonBody(requestData);

            var response = await client.ExecuteAsync(request);

            Console.WriteLine("==== MOMO REQUEST ====");
            Console.WriteLine(JsonConvert.SerializeObject(requestData));

            Console.WriteLine("==== MOMO RESPONSE ====");
            Console.WriteLine(response.Content);


            var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);

            if (momoResponse == null || momoResponse.ResultCode != 0)
            {
                throw new Exception("MoMo error: " + response.Content);
            }

            return momoResponse;
        }

        // Các hàm phụ trợ giữ nguyên
        public MomoExecuteResponseModel PaymentExecute(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;

            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo
            };
        }

        private static string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);

            return BitConverter.ToString(hashBytes)
                .Replace("-", "")
                .ToLower();
        }

    }
}