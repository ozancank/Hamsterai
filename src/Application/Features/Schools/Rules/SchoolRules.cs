using Application.Features.Schools.Models.Schools;
using DataAccess.EF;

namespace Application.Features.Schools.Rules;

public class SchoolRules(ISchoolDal schoolDal) : IBusinessRule
{
    internal static Task SchoolShouldExists(object school)
    {
        if (school == null) throw new BusinessException(Strings.DynamicNotFound, Strings.School);
        return Task.CompletedTask;
    }

    internal static Task SchoolShouldExists(GetSchoolModel schoolModel)
    {
        if (schoolModel == null) throw new BusinessException(Strings.DynamicNotFound, Strings.School);
        return Task.CompletedTask;
    }

    internal static Task SchoolShouldExists(School school)
    {
        if (school == null) throw new BusinessException(Strings.DynamicNotFound, Strings.School);
        return Task.CompletedTask;
    }

    internal static Task SchoolShouldExistsAndActive(bool isActive)
    {
        if (!isActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.School);
        return Task.CompletedTask;
    }

    internal async Task SchoolShouldExists(int id)
    {
        var exists = await schoolDal.IsExistsAsync(x => x.Id == id, enableTracking: false);
        if (exists) throw new BusinessException(Strings.DynamicNotFound, Strings.School);
    }

    internal async Task SchoolShouldExistsAndActive(int id)
    {
        var exists = await schoolDal.IsExistsAsync(x => x.Id == id && x.IsActive, enableTracking: false);
        if (!exists) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.School);
    }

    internal async Task SchoolShouldExists(string taxNumber)
    {
        var exists = await schoolDal.IsExistsAsync(x => PostgresqlFunctions.TrLower(x.TaxNumber) == PostgresqlFunctions.TrLower(taxNumber), enableTracking: false);
        if (!exists) throw new BusinessException(Strings.DynamicExists, Strings.School);
    }

    internal async Task SchoolNameAndCityCanNotBeDuplicated(string name, string city, int? schoolId = null)
    {
        name = name.Trim().ToLower();
        city = city.Trim().ToLower();
        var school = await schoolDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(name)
                                                           && PostgresqlFunctions.TrLower(x.City) == PostgresqlFunctions.TrLower(city), enableTracking: false);
        if (schoolId == null && school != null) throw new BusinessException(Strings.DynamicExists, name);
        if (schoolId != null && school != null && school.Id != schoolId) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal async Task SchoolTaxNumberCanNotBeDuplicated(string taxNumber, int? schoolId = null)
    {
        taxNumber = taxNumber.Trim().ToLower();
        var school = await schoolDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.TaxNumber) == PostgresqlFunctions.TrLower(taxNumber), enableTracking: false);
        if (schoolId == null && school != null) throw new BusinessException(Strings.DynamicExists, taxNumber);
        if (schoolId != null && school != null && school.Id != schoolId) throw new BusinessException(Strings.DynamicExists, taxNumber);
    }

    internal static async Task AccessStudentEnabled(bool accessStundents, UserTypes userType)
    {
        if (!accessStundents && userType == UserTypes.Student) throw new BusinessException(Strings.AccessDeniedBySchool);
        await Task.CompletedTask;
    }
}