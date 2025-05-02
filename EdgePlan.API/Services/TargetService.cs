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
        var entity = await _context.Targets
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == sessionId, cancellationToken);
        
        if (entity == null)
            throw new Exception("Target.Not.Found");
        
        entity.Text = model.Text;
        entity.DeadLine = model.DeadLine.ToUniversalTime();
        entity.Status = model.Status;
        entity.ChangedAt = DateTime.UtcNow.ToUniversalTime();
        
        try
        {
            _context.Targets.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        catch (Exception)
        {
            throw new Exception("Error.Try.Again.Or.Contact.Administrator");
        }
        
        return entity;
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