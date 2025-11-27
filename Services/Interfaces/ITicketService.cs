using OdiNow.Contracts.Requests.Tickets;
using OdiNow.Contracts.Responses.Tickets;

namespace OdiNow.Services.Interfaces;

public interface ITicketService
{
    Task<IEnumerable<TicketDropResponse>> GetActiveDropsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<TicketClaimResponse> ClaimTicketAsync(Guid userId, Guid dropId, CancellationToken cancellationToken = default);

    Task<IEnumerable<TicketClaimResponse>> GetHistoryAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> RedeemTicketAsync(Guid userId, Guid claimId, RedeemTicketRequest request, CancellationToken cancellationToken = default);
}


