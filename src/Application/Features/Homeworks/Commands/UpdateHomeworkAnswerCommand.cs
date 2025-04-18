﻿using Application.Features.Homeworks.Models;
using Application.Features.Homeworks.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Homeworks.Commands;

public class UpdateHomeworkAnswerCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required HomeworkAnswerRequestModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Student];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [$"{nameof(Model)}.{nameof(Model.AnswerPictureBase64)}"];
}

public class UpdateHomeworkAnswerCommandHandler(ICommonService commonService,
                                                IHomeworkStudentDal homeworkStudentDal) : IRequestHandler<UpdateHomeworkAnswerCommand, bool>
{
    public async Task<bool> Handle(UpdateHomeworkAnswerCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;
        var connectionId = commonService.HttpConnectionId;
        var date = DateTime.Now;

        var homework = await homeworkStudentDal.GetAsync(
            predicate: x => x.Id == request.Model.HomeworkStudentId && x.StudentId == connectionId,
            cancellationToken: cancellationToken);

        await HomeworkRules.HomeworkStudentShouldExists(homework);

        var fileName = $"HWA_{request.Model.HomeworkStudentId}{Path.GetExtension(request.Model.AnswerPictureFileName)}";
        await commonService.PictureConvert(request.Model.AnswerPictureBase64, fileName, AppOptions.HomeworkAnswerFolderPath);

        homework.UpdateUser = userId;
        homework.UpdateDate = date;
        homework.AnswerPath = fileName;
        homework.Status = HomeworkStatus.Answered;

        await homeworkStudentDal.UpdateAsync(homework, cancellationToken: cancellationToken);

        return true;
    }
}

public class UpdateHomeworkAnswerCommandValidator : AbstractValidator<UpdateHomeworkAnswerCommand>
{
    public UpdateHomeworkAnswerCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.HomeworkStudentId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Homework]);

        RuleFor(x => x.Model.AnswerPictureBase64).MustBeValidBase64().WithMessage(Strings.DynamicNotEmpty, [Strings.Answer]);

        RuleFor(x => x.Model.AnswerPictureFileName).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.FileName]);
        RuleFor(x => x.Model.AnswerPictureFileName).Must(x => x.EmptyOrTrim().Contains('.')).WithMessage(Strings.FileNameExtension);
    }
}