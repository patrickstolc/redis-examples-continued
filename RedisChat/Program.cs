using CommandLine;
using RedisChat;

public class Program
{
    public class Options
    {
        [Option('c', "channel", Required = true, HelpText = "Channel name")]
        public string Channel { get; set; }
        
        [Option('n', "name", Required = true, HelpText = "Name of the client")]
        public string Name { get; set; }
    }
    
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                // Create a new client
                var client = new ChatClient("localhost", 6379, "");
                client.Connect();
                
                client.Join(o.Channel, (message =>
                {
                    Console.WriteLine(message.Message);
                }));
                
                // tell the channel we have joined
                client.SendMessage(o.Channel, $"Client {o.Name} joined");
                
                // loop
                while (true)
                {
                    var message = Console.ReadLine();
                    client.SendMessage(o.Channel, $"[{o.Name}]: {message}");
                }
            });
    }
}