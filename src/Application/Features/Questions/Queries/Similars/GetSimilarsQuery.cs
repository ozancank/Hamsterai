﻿using Application.Features.Questions.Models.Questions;
using Application.Features.Questions.Models.Similars;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Questions.Queries.Similars;

public class GetSimilarsQuery : IRequest<PageableModel<GetSimilarModel>>, ISecuredRequest<UserTypes>
{
    public required PageRequest PageRequest { get; set; }
    public required SimilarRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class GetSimilarsQueryHandler(IMapper mapper,
                                     ISimilarDal similarDal,
                                     ICommonService commonService) : IRequestHandler<GetSimilarsQuery, PageableModel<GetSimilarModel>>
{
    public async Task<PageableModel<GetSimilarModel>> Handle(GetSimilarsQuery request, CancellationToken cancellationToken)
    {
        request.PageRequest ??= new PageRequest();
        request.Model ??= new SimilarRequestModel();

        if (request.Model.StartDate == null) request.Model.StartDate = DateTime.Today.AddDays(-7);
        if (request.Model.EndDate == null) request.Model.EndDate = DateTime.Today;

        var similars = await similarDal.GetPageListAsyncAutoMapper<GetSimilarModel>(
            predicate: x => x.CreateUser == commonService.HttpUserId && x.IsActive
                            && (request.Model.LessonId <= 0 || x.LessonId == request.Model.LessonId)
                            && x.CreateDate.Date >= request.Model.StartDate.Value.Date
                            && x.CreateDate.Date <= request.Model.EndDate.Value.Date.AddDays(1).AddSeconds(-1),
            enableTracking: false,
            include: x => x.Include(u => u.Lesson),
            orderBy: x => x.OrderByDescending(u => u.CreateDate),
            configurationProvider: mapper.ConfigurationProvider,
            index: request.PageRequest.Page, size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);
        var result = mapper.Map<PageableModel<GetSimilarModel>>(similars);
        return result;
    }
}