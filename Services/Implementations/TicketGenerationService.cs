using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OdiNow.Data;
using OdiNow.Models;
using OdiNow.Options;

namespace OdiNow.Services.Implementations;

public class TicketGenerationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TicketGenerationService> _logger;
    private readonly TicketOptions _options;

    public TicketGenerationService(IServiceScopeFactory scopeFactory, IOptions<TicketOptions> options, ILogger<TicketGenerationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await GenerateDropIfNeededAsync(context, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate ticket drop.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(_options.GenerationIntervalMinutes), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private async Task GenerateDropIfNeededAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var hasActiveDrop = await context.TicketDrops
            .AnyAsync(td => td.IsActive && td.TicketsRemaining > 0 && td.ExpiresAt > now, cancellationToken);

        if (hasActiveDrop)
        {
            return;
        }

        var eligibleOffers = await context.Offers
            .Where(o => o.IsActive && o.StartAt <= now && o.EndAt >= now)
            .ToListAsync(cancellationToken);

        if (!eligibleOffers.Any())
        {
            return;
        }

        var offer = eligibleOffers[Random.Shared.Next(eligibleOffers.Count)];
        var drop = new TicketDrop
        {
            Id = Guid.NewGuid(),
            OfferId = offer.Id,
            TicketsTotal = _options.TicketsPerInterval,
            TicketsRemaining = _options.TicketsPerInterval,
            AvailableFrom = now,
            ExpiresAt = now.AddMinutes(_options.DropDurationMinutes),
            IsActive = true,
            CreatedAt = now
        };

        await context.TicketDrops.AddAsync(drop, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Generated ticket drop {DropId} for offer {OfferId}", drop.Id, offer.Id);
    }
}


