using Business.Features.Schools.Models.Schools;

namespace Business.Features.Schools.Rules;

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

    internal static Task SchoolShouldExistsAndIsActive(bool isActive)
    {
        if (!isActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.School);
        return Task.CompletedTask;
    }

    internal async Task SchoolShouldExists(int id)
    {
        var exists = await schoolDal.IsExistsAsync(x => x.Id == id, enableTracking: false);
        if (exists) throw new BusinessException(Strings.DynamicNotFound, Strings.School);
    }

    internal async Task SchoolShouldExists(string taxNumber)
    {
        var exists = await schoolDal.IsExistsAsync(x => x.TaxNumber == taxNumber, enableTracking: false);
        if (!exists) throw new BusinessException(Strings.DynamicExists, Strings.School);
    }

    internal async Task SchoolNameAndCityCanNotBeDuplicated(string name, string city, int? schoolId = null)
    {
        name = name.Trim().ToLower();
        city = city.Trim().ToLower();
        var school = await schoolDal.GetAsync(predicate: x => x.Name == name && x.City == city, enableTracking: false);
        if (schoolId == null && school != null) throw new BusinessException(Strings.DynamicExists, name);
        if (schoolId != null && school != null && school.Id != schoolId) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal async Task SchoolTaxNumberCanNotBeDuplicated(string taxNumber, int? schoolId = null)
    {
        taxNumber = taxNumber.Trim().ToLower();
        var school = await schoolDal.GetAsync(predicate: x => x.TaxNumber == taxNumber, enableTracking: false);
        if (schoolId == null && school != null) throw new BusinessException(Strings.DynamicExists, taxNumber);
        if (schoolId != null && school != null && school.Id != schoolId) throw new BusinessException(Strings.DynamicExists, taxNumber);
    }
}