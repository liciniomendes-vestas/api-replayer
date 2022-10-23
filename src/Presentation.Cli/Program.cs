using System.Net.Http.Headers;
using System.Text.Json;

using Microsoft.AspNetCore.Http;

using Presentation.Cli;

if (args.Length != 1)
{
    Console.WriteLine("ERROR: You need to supply a database name as argument");

    return 1;
}

var dbName = Path.Combine(AppContext.BaseDirectory, "../../../../../", args[0]);
Console.WriteLine(dbName);
var storage = new Storage(dbName);

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://localhost:9000");

while (true)
{
    var request = await storage.GetNextAsync();
    if (request is null) break;

    var headers = JsonSerializer.Deserialize<Dictionary<string, string[]>>(request.Headers);
    foreach (var header in headers.Keys)
    {
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header, string.Join(';', headers[header]));
    }

    var r = new HttpRequestMessage(new HttpMethod(request.Method), new Uri(request.Path));
    if (r.Method == HttpMethod.Post) r.Content = new StringContent(request.Body);
    var response = await httpClient.SendAsync(r);

    Console.WriteLine($"{request.Method} {request.Path} - {response.StatusCode}");
}

return 0;
