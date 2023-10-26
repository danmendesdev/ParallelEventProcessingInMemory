using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    private const string BaseUrl = "https://example.com/resource?page=";
    private const int MaxRetries = 3;

    static async Task Main()
    {
        var httpClient = new HttpClient();
        var tasks = new List<Task>();
        var pagesToFetch = new ConcurrentQueue<int>(Enumerable.Range(1, 10));

        while (!pagesToFetch.IsEmpty)
        {
            if (pagesToFetch.TryDequeue(out int pageNumber))
            {
                tasks.Add(FetchPage(httpClient, pageNumber, pagesToFetch));
            }
        }

        await Task.WhenAll(tasks);
    }

    static async Task FetchPage(HttpClient httpClient, int pageNumber, ConcurrentQueue<int> retryQueue, int retryCount = 0)
    {
        try
        {
            var url = BaseUrl + pageNumber;
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Page {pageNumber}: {content.Substring(0, Math.Min(100, content.Length))}...");
            }
            else
            {
                Console.WriteLine($"Error fetching page {pageNumber}: {response.StatusCode}");
                RequeueIfNeeded(retryQueue, pageNumber, retryCount);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception fetching page {pageNumber}: {ex.Message}");
            RequeueIfNeeded(retryQueue, pageNumber, retryCount);
        }
    }

    static void RequeueIfNeeded(ConcurrentQueue<int> retryQueue, int pageNumber, int retryCount)
    {
        if (retryCount < MaxRetries)
        {
            Console.WriteLine($"Retrying page {pageNumber}...");
            retryQueue.Enqueue(pageNumber);
        }
        else
        {
            Console.WriteLine($"Failed to fetch page {pageNumber} after {MaxRetries} attempts.");
        }
    }
}

