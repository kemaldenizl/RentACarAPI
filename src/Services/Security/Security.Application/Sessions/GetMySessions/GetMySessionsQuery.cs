using MediatR;
using Security.Application.Common.Results;
using Security.Application.Sessions.Dtos;

namespace Security.Application.Sessions.GetMySessions;

public sealed record GetMySessionsQuery(
    Guid UserId,
    Guid? CurrentSessionId
) : IRequest<Result<IReadOnlyCollection<SessionDto>>>;