using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace RateLimitedAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RateLimitedController : ControllerBase
{
    private readonly IDatabase _database;
    
    public RateLimitedController(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetValueOfKey(string key)
    {
        var script = Scripts.RateLimiterScript;
        var result =
            await _database.ScriptEvaluateAsync(script, new { key = new RedisKey(key), expiry = 60, maxRequests = 2 });
        if((int) result == 1)
            return new StatusCodeResult(429);
        
        return Ok("Value of key is: " + _database.StringGet(key));
    }
}