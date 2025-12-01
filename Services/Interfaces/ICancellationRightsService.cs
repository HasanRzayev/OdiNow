using OdiNow.Contracts.Responses;

namespace OdiNow.Services.Interfaces;

public interface ICancellationRightsService
{
    Task<CancellationRightsResponse> GetRightsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UseRightAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default);
    Task EnsureRightsAsync(Guid userId, CancellationToken cancellationToken = default);
}




