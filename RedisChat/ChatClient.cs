using StackExchange.Redis;

namespace RedisChat;

public class ChatClient
{
    private readonly string _hostname;
    private readonly int _port;
    private readonly string _password;
    
    private ConnectionMultiplexer _connection;
    private IDatabase _database;
    private ISubscriber _subscriber;
    
    public ChatClient(string hostname, int port, string password)
    {
        _hostname = hostname;
        _port = port;
        _password = password;
    }

    public void Connect()
    {
        _connection = ConnectionMultiplexer.Connect($"{_hostname}:{_port},password={_password}");
        _database = _connection.GetDatabase();
    }

    public void Join(string channel, Action<ChannelMessage> handler)
    {
        _subscriber = _connection.GetSubscriber();
        _subscriber.Subscribe(RedisChannel.Literal(channel)).OnMessage(handler);
    }

    public void SendMessage(string channel, string message)
    {
        _subscriber.Publish(RedisChannel.Literal(channel), message);
    }
}