
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;

namespace Presentation.Controller;

internal class HttpClientController
{
    private HttpClient _httpClient;
    private Uri _baseUri;
    HttpClientController(string ip = "127.0.0.1")
    {
        try
        {
            _baseUri = new Uri(ip);
            _httpClient = new();
            _httpClient.BaseAddress = _baseUri;
            _httpClient.DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        catch (SocketException ex)
        {
            //log
            Console.WriteLine($"Error at creating HttpClient\nError: {ex.Message}");
        }

    }

    ~HttpClientController()
    {
        _httpClient.Dispose();
        _httpClient = null;
    }

    private async Task Request()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUri}/users");
        request.Content = new StringContent("{\"username\":\"deniial\",\"password\":\"testy\"}", Encoding.UTF8, "application/json");
    }
}
