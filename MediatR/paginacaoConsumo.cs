using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(Program).Assembly);
        services.AddSingleton(new HttpClient());

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var tasks = Enumerable.Range(1, 10).Select(pageNumber => 
            mediator.Send(new FetchPageCommand(pageNumber))
        );

        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            Console.WriteLine(result.Substring(0, Math.Min(100, result.Length)) + "...");
        }
    }
}
