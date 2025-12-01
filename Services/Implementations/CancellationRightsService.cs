using Microsoft.EntityFrameworkCore;
using OdiNow.Contracts.Responses;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Services.Interfaces;

namespace OdiNow.Services.Implementations;

public class CancellationRightsService : ICancellationRightsService
{
    private readonly ApplicationDbContext _dbContext;
    private const int MaxRights = 5;
    private const int RenewalIntervalMinutes = 15;

    public CancellationRightsService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CancellationRightsResponse> GetRightsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await EnsureRightsAsync(userId, cancellationToken);

        var rights = await _dbContext.CancellationRights
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.GeneratedAt)
            .Take(MaxRights)
            .ToListAsync(cancellationToken);

        var availableCount = rights.Count(r => !r.IsUsed);
        var latestRight = rights.OrderByDescending(r => r.GeneratedAt).FirstOrDefault();
        var nextRenewal = latestRight != null && availableCount < MaxRights
            ? latestRight.GeneratedAt.AddMinutes(RenewalIntervalMinutes)
            : (DateTimeOffset?)null;

        return new CancellationRightsResponse
        {
            AvailableRights = availableCount,
            MaxRights = MaxRights,
            NextRenewalAt = nextRenewal,
            Rights = rights.Select(r => new CancellationRightItem
            {
                Id = r.Id,
                IsUsed = r.IsUsed,
                GeneratedAt = r.GeneratedAt,
                UsedAt = r.UsedAt
            }).ToList()
        };
    }

    public async Task<bool> UseRightAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default)
    {
        var availableRight = await _dbContext.CancellationRights
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsUsed, cancellationToken);

        if (availableRight == null)
        {
            return false;
        }

        availableRight.IsUsed = true;
        availableRight.UsedAt = DateTimeOffset.UtcNow;
        availableRight.OrderId = orderId;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task EnsureRightsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var rights = await _dbContext.CancellationRights
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.GeneratedAt)
            .Take(MaxRights)
            .ToListAsync(cancellationToken);

        var availableCount = rights.Count(r => !r.IsUsed);

        if (availableCount >= MaxRights)
        {
            return;
        }

        var latestRight = rights.OrderByDescending(r => r.GeneratedAt).FirstOrDefault();
        if (latestRight != null && latestRight.GeneratedAt.AddMinutes(RenewalIntervalMinutes) > now)
        {
            return;
        }

        var newRight = new CancellationRight
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IsUsed = false,
            GeneratedAt = now
        };

        await _dbContext.CancellationRights.AddAsync(newRight, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}




