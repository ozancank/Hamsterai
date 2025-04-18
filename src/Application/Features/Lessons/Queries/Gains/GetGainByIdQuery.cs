﻿using Application.Features.Lessons.Models.Gains;
using Application.Features.Lessons.Rules;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Lessons.Queries.Gains;

public class GetGainByIdQuery : IRequest<GetGainModel>, ISecuredRequest<UserTypes>
{
    public short Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetGainByIdQueryHandler(IMapper mapper,
                                     IGainDal gainDal) : IRequestHandler<GetGainByIdQuery, GetGainModel>
{
    public async Task<GetGainModel> Handle(GetGainByIdQuery request, CancellationToken cancellationToken)
    {
        var gain = await gainDal.GetAsyncAutoMapper<GetGainModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await GainRules.GainShouldExists(gain);
        return gain;
    }
}