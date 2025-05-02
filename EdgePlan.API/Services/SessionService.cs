using Microsoft.Extensions.Caching.Memory;

public class SessionService
{
    private readonly IMemoryCache _memoryCache;

    public SessionService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void BlacklistToken(string token, TimeSpan duration)
    {
        _memoryCache.Set("blacklisted_" + token, true, duration);
    }

    public bool IsTokenBlacklisted(string token)
    {
        return _memoryCache.TryGetValue("blacklisted_" + token, out _);
    }
}