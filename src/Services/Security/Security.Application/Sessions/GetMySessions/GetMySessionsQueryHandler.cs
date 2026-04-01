using MediatR;
using Security.Application.Abstractions.Persistence;
using Security.Application.Common.Results;
using Security.Application.Sessions.Dtos;

namespace Security.Application.Sessions.GetMySessions;

public sealed class GetMySessionsQueryHandler(
    IRefreshSessionRepository refreshSessionRepository)
    : IRequestHandler<GetMySessionsQuery, Result<IReadOnlyCollection<SessionDto>>>
{
    public async Task<Result<IReadOnlyCollection<SessionDto>>> Handle(
        GetMySessionsQuery request,
        CancellationToken cancellationToken)
    {
        var sessions = await refreshSessionRepository.GetByUserIdAsync(
            request.UserId,
            cancellationToken);

        var response = sessions
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new SessionDto(
                x.Id,
                x.DeviceName,
                x.IpAddress,
                x.CreatedAtUtc,
                x.Revoked,
                x.RevokedAtUtc,
                request.CurrentSessionId.HasValue && x.Id == request.CurrentSessionId.Value))
            .ToArray();

        return Result<IReadOnlyCollection<SessionDto>>.Success(response);
    }
}