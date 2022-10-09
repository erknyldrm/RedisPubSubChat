using RedisCore;

using (RedisClient client = new RedisClient())
{
    var pubSub = client.GetSubscriber();
    bool isStay = true;
    while (isStay)
    {
        //Subscriber
        await pubSub.SubscribeAsync(client.Channel2, (channel, message) =>
        {
            Console.WriteLine(Environment.NewLine + "Elie: " + message);

            //Publisher
            Console.WriteLine("Write message: ");
            var message2 = Console.ReadLine();
            isStay = message2.ToLower() != "exit";
            pubSub.PublishAsync(client.Channel, message2, StackExchange.Redis.CommandFlags.FireAndForget);
        });

        //Publisher
        Console.WriteLine("Write message: ");
        var message = Console.ReadLine();
        isStay = message.ToLower() != "exit";
        pubSub.PublishAsync(client.Channel, message, StackExchange.Redis.CommandFlags.FireAndForget);
    }
}
