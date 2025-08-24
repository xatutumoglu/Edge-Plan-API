using EdgePlan.API.Services;
using EdgePlan.Data.Entity;
using EdgePlan.Data.Models;
using EdgePlan.Data.Postgre;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class SessionService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ApplicationPostgreContext _context;
    private readonly TokenService _tokenService;
    private readonly IBackgroundJobClient _jobs;
    
    public SessionService(IMemoryCache memoryCache, ApplicationPostgreContext context, TokenService tokenService, IBackgroundJobClient jobs)
    {
        _memoryCache = memoryCache;
        _context = context;
        _tokenService = tokenService;
        _jobs = jobs;
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
        
        _jobs.Enqueue<NotificationService>(svc => 
            svc.SendWelcomeEmailAsync(user.Id, CancellationToken.None));
        
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
                   ?? throw new UnauthorizedAccessException("Invalid.Password.Or.Email.Or.Inactive.User.Try.Again.Or.Contact.Administrator.");

        var hasher = new PasswordHasher<User>();
        var verifyResult = hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (verifyResult == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid.Password.Or.Email.Or.Inactive.User.Try.Again.Or.Contact.Administrator.");

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