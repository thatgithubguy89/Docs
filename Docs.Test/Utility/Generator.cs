using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Newtonsoft.Json;
using System.Text;

namespace Docs.Test.Utility
{
    public class Generator
    {
        // Reference: https://stackoverflow.com/questions/67937686/testing-an-azure-function-in-net-5

        // Use this for functions that don't require a body
        public static HttpRequestData GenerateMockRequest()
        {
            var context = new Mock<FunctionContext>();

            var byteArray = Encoding.ASCII.GetBytes("test");
            var bodyStream = new MemoryStream(byteArray);

            var request = new Mock<HttpRequestData>(context.Object);
            request.Setup(r => r.Body).Returns(bodyStream);
            request.Setup(r => r.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(context.Object);
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.StatusCode);
                response.SetupProperty(r => r.Body, new MemoryStream());
                return response.Object;
            });

            return request.Object;
        }

        // Use this for functions that require a body
        public static HttpRequestData GenerateMockRequestWithBody(object? body)
        {
            var context = new Mock<FunctionContext>();

            var stringBody = JsonConvert.SerializeObject(body);
            var byteArray = Encoding.ASCII.GetBytes(stringBody);
            var bodyStream = new MemoryStream(byteArray);

            var request = new Mock<HttpRequestData>(context.Object);
            request.Setup(r => r.Body).Returns(bodyStream);
            request.Setup(r => r.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(context.Object);
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.StatusCode);
                response.SetupProperty(r => r.Body, new MemoryStream());
                return response.Object;
            });

            return request.Object;
        }
    }
}
