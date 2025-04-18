﻿using Application.Features.Lessons.Dto;
using Application.Features.Lessons.Models.Gains;
using Application.Features.Lessons.Rules;
using DataAccess.EF;

namespace Application.Services.GainService;

public class GainManager(IMapper mapper) : IGainService
{
    public async Task<GetGainModel?> GetOrAddGain(AddGainDto dto)
    {
        if (dto.GainName.IsEmpty()) return null;

        var isNull = dto.Context == null;
        var context = dto.Context ?? ServiceTools.GetService<IDbContextFactory<HamsteraiDbContext>>().CreateDbContext();

        try
        {
            var lessonName = await context.Lessons
                .AsNoTracking()
                .Where(x => x.Id == dto.LessonId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();
            await LessonRules.LessonShouldExists(lessonName);

            var gainNames = dto.GainName?.Trim("\r").Split("\n") ?? [];
            var gainName = string.Empty;

            for (int i = 0; i < gainNames.Length; i++)
            {
                if (gainNames[i].IsEmpty()) continue;
                gainName = gainNames[i].Trim().FixTextLenght(100);
                break;
            }

            var gain = await context.Gains
                .AsNoTracking()
                .Where(x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(gainName) && x.LessonId == dto.LessonId)
                .FirstOrDefaultAsync();

            var date = DateTime.Now;

            if (gain == null)
            {
                gain = new Gain
                {
                    Id = await context.Gains.AnyAsync() ? (await context.Gains.MaxAsync(x => x.Id) + 1) : 1,
                    IsActive = true,
                    CreateUser = dto.UserId,
                    CreateDate = date,
                    UpdateUser = dto.UserId,
                    UpdateDate = date,
                    Name = gainName,
                    LessonId = dto.LessonId
                };
                await context.Gains.AddAsync(gain);
                await context.SaveChangesAsync();
            }

            var result = mapper.Map<GetGainModel>(gain);
            return result;
        }
        finally
        {
            if (isNull) context.Dispose();
        }
    }
}