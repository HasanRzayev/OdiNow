using OdiNow.Contracts.Responses.Tickets;

namespace OdiNow.Services.Interfaces;

public interface ITicketService
{
    Task<TicketSummaryResponse> GetSummaryAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> ConsumeTicketForOfferAsync(Guid userId, Guid offerId, CancellationToken cancellationToken = default);
}





