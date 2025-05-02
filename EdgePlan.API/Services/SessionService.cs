using EdgePlan.API.Services;
using EdgePlan.Data.Entity;
using EdgePlan.Data.Models;
using EdgePlan.Data.Postgre;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class SessionService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ApplicationPostgreContext _context;
    private readonly TokenService _tokenService;
    
    public SessionService(IMemoryCache memoryCache, ApplicationPostgreContext context, TokenService tokenService)
    {
        _memoryCache = memoryCache;
        _context = context;
        _tokenService = tokenService;
    }
    
    public async Task<UserRegisterResponseModel> RegisterAsync(UserRegisterRequestModel model, CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(x => x.Email == model.Email))
            throw new Exception();

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PasswordHash = model.Password,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, model.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user);
        return new UserRegisterResponseModel
        {
            Token = token
        };
    }
    
    public async Task<UserLoginResponseModel> LoginAsync(UserLoginRequestModel model, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
                       .FirstOrDefaultAsync(x => x.Email == model.Email)
                   ?? throw new Exception();

        var hasher = new PasswordHasher<User>();
        var verifyResult = hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (verifyResult == PasswordVerificationResult.Failed)
            throw new Exception();

        var token = _tokenService.GenerateToken(user);
        return new UserLoginResponseModel
        {
            Token = token
        };
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