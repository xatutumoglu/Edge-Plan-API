using EdgePlan.Data.Entity;
using EdgePlan.Data.Enums;
using EdgePlan.Data.Models;
using EdgePlan.Data.Postgre;
using Microsoft.EntityFrameworkCore;

namespace EdgePlan.API.Services;

public class TargetService
{
    private readonly ApplicationPostgreContext _context;
    
    public TargetService(ApplicationPostgreContext context)
    {
        _context = context;
    }
    public async Task<Target> CreateAsync(TargetCreateModel model, Guid id, CancellationToken cancellationToken = default)
    {
        var target = new Target
        {
            Id = Guid.NewGuid(),
            Text = model.Text,
            DeadLine = model.DeadLine.ToUniversalTime(),
            Status = TargetStatus.ToDo,
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
            UserId = id
        };
        
        try
        {
             _context.Targets.Add(target);
             await _context.SaveChangesAsync(cancellationToken);
            return target;
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Error.Try.Again.Or.Contact.Administrator");
        }
    }

    public async Task<List<Target>> GetAllAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Targets
            .AsNoTracking()
            .Where(x => x.UserId == id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Target> GetByIdAsync(Guid userId, Guid targetId, CancellationToken cancellationToken = default)
    {
        var result = await _context.Targets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == targetId && x.UserId == userId, cancellationToken);
        
        return result;
    }

    public async Task<Target> UpdateAsync(TargetUpdateModel model, Guid id, Guid sessionId, CancellationToken cancellationToken = default)
    {
        return null;
    }

    public async Task<Target> DeleteAsync(Guid userId, Guid targetId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Targets.FirstOrDefaultAsync(x => x.Id == targetId && x.UserId == userId, cancellationToken);
        
        if (entity == null)
            throw new Exception("Target.Not.Found");
        
        _context.Targets.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}