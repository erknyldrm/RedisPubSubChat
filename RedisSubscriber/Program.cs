using ElasticCore;
using RedisCore;
using System.Configuration;

using (RedisClient client = new())
using (ElasticCoreService<ChatModel> elastic = new())
{
    //Get Chat History From Elastic                
    var chats = elastic.SearchChatLog(5);
    Console.WriteLine("TOP 5 Message History:");
    Console.WriteLine("".PadRight(60, '*'));
    foreach (var chat in chats)
    {
        Console.WriteLine($"-{chat.From}({chat.PostDate}): {chat.Message}");
    }
    Console.WriteLine("".PadRight(60, '*'));
    Console.WriteLine();
    //------------------------------
    var pubSub = client.GetSubscriber();
    bool isStay = true;
    while (isStay)
    {
        await pubSub.SubscribeAsync(client.Channel, (cannel, message) =>
        {
            Console.WriteLine(Environment.NewLine + "Joe: " + message);

            Console.Write("Write Message : ");
            var message2 = Console.ReadLine();
            isStay = message2.ToLower() != "exit" ? true : false;
            pubSub.PublishAsync(client.Channel2, message2, StackExchange.Redis.CommandFlags.FireAndForget);
            ChatModel chatModel = new ChatModel() { From = "Joe", To = "Elie", Message = message2, PostDate = DateTime.Now };
            elastic.CheckExistsAndInsertLog(chatModel, ConfigurationManager.AppSettings["ElasticIndexName"]);
        });

        Console.Write("Write Message : ");
        var message = Console.ReadLine();
        isStay = message.ToLower() != "exit" ? true : false;
        await pubSub.PublishAsync(client.Channel2, message, StackExchange.Redis.CommandFlags.FireAndForget);
        ChatModel chatModel = new ChatModel() { From = "Joe", To = "Elie", Message = message, PostDate = DateTime.Now };
        elastic.CheckExistsAndInsertLog(chatModel, ConfigurationManager.AppSettings["ElasticIndexName"]);
    }
}
