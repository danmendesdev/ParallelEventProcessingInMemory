using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class FetchPageCommand : IRequest<string>
{
    public int PageNumber { get; }

    public FetchPageCommand(int pageNumber)
    {
        PageNumber = pageNumber;
    }
}

public class FetchPageCommandHandler : IRequestHandler<FetchPageCommand, string>
{
    private const string BaseUrl = "https://example.com/resource?page=";
    private readonly HttpClient _httpClient;

    public FetchPageCommandHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> Handle(FetchPageCommand request, CancellationToken cancellationToken)
    {
        var url = BaseUrl + request.PageNumber;
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            throw new HttpRequestException($"Error fetching page {request.PageNumber}: {response.StatusCode}");
        }
    }
}
