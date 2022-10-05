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
    public HttpClientController(string ip = "http://localhost/")
    {
        Console.Write("Client starting ...");
        try
        {
            _baseUri = new Uri(ip);
            _httpClient = new();
            _httpClient.BaseAddress = _baseUri;
            _httpClient.DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        catch (HttpRequestException ex)
        {
            //log
            Console.WriteLine($"Error at creating HttpClient\nError: {ex.Message}");
            throw;
        }
        Console.WriteLine(" OK!\n");
    }

    ~HttpClientController()
    {
        _httpClient.Dispose();
        _httpClient = null;
    }

    public async Task Request()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUri}/users");
        request.Content = new StringContent("{\"username\":\"deniial\",\"password\":\"testy\"}", Encoding.UTF8, "application/json");
        var response = await _httpClient.SendAsync(request); 
        Console.WriteLine(response.Content);
    }
}