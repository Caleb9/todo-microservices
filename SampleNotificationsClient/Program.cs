using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SampleNotificationsClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Provide TODO list id as parameter.");
                return;
            }

            var listId = args[0];

            var connection =
                new HubConnectionBuilder()
                    .WithUrl("http://localhost:5100/todo")
                    .Build();

            connection.On<Guid>("sendToClient", async itemId =>
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"http://localhost:5000/items/{itemId}");
                if (response.IsSuccessStatusCode)
                {
                    var newItem = await response.Content.ReadFromJsonAsync<NewItem>();
                    Console.WriteLine($"New TODO item: {newItem?.Value}");
                }

            });

            await connection.StartAsync();
            await connection.InvokeAsync("SubscribeToList", listId);

            Console.WriteLine("Press enter to stop");
            Console.ReadLine();
        }

        private record NewItem(Guid Id, string Value);
    }
}