using Application.Features.Packages.Commands.PackageCategories;
using Application.Features.Packages.Commands.Packages;
using Application.Features.Packages.Models.PackageCategories;
using Application.Features.Packages.Models.Packages;
using Application.Features.Packages.Queries.PackageCategories;
using Application.Features.Packages.Queries.Packages;
using Asp.Versioning;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class PackageController() : BaseController
{
    #region Package

    [HttpGet("GetPackageById/{id}")]
    public async Task<IActionResult> GetPackageById([FromRoute] short id)
    {
        var query = new GetPackageByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetPackageByIdForWeb/{id}")]
    public async Task<IActionResult> GetPackageByIdForWeb([FromRoute] short id)
    {
        var query = new GetPackageByIdQuery { Id = id, ForWeb = true };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetPackageBySlug/{slug}")]
    public async Task<IActionResult> GetPackageById([FromRoute] string slug)
    {
        var query = new GetPackageBySlugQuery { Slug = slug };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetPackages")]
    public async Task<IActionResult> GetPackages([FromQuery] PageRequest model)
    {
        var query = new GetPackagesQuery { PageRequest = model };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetPackagesDynamic")]
    public async Task<IActionResult> GetPackagesDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamic)
    {
        var query = new GetPackagesByDynamicQuery { PageRequest = model, Dynamic = dynamic };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddPackage")]
    public async Task<IActionResult> AddPackage([FromForm] AddPackageModel model)
    {
        var command = new AddPackageCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdatePackage")]
    public async Task<IActionResult> UpdatePackage([FromForm] UpdatePackageModel model)
    {
        var command = new UpdatePackageCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassivePackage")]
    public async Task<IActionResult> PassivePackage([FromBody] short packageId)
    {
        var command = new PassivePackageCommand { Id = packageId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("ActivePackage")]
    public async Task<IActionResult> ActivePackage([FromBody] short packageId)
    {
        var command = new ActivePackageCommand { Id = packageId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetPackagesForWeb")]
    public async Task<IActionResult> GetPackagesForWeb([FromQuery] PageRequest model)
    {
        var query = new GetPackagesQuery { PageRequest = model, ForWeb = true };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetPackagesForWebByIds")]
    public async Task<IActionResult> GetPackagesForWebByIds([FromBody] List<short> ids)
    {
        var query = new GetPackagesForWebByIdsQuery { PackageIds = ids };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion Package

    #region Category

    [HttpGet("GetCategoryById/{id}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] byte id)
    {
        var query = new GetPackageCategoryByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetCategoryByIdForWeb/{id}")]
    public async Task<IActionResult> GetCategoryByIdForWeb([FromRoute] byte id)
    {
        var query = new GetPackageCategoryByIdQuery { Id = id, ForWeb = true };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("GetCategoryBySlug/{slug}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] string slug)
    {
        var query = new GetPackageCategoryBySlugQuery { Slug = slug };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetCategories")]
    public async Task<IActionResult> GetCategories([FromQuery] PageRequest model)
    {
        var query = new GetPackageCategoriesQuery { PageRequest = model };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("GetCategoriesDynamic")]
    public async Task<IActionResult> GetCategoriesDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamic)
    {
        var query = new GetPackageCategoriesByDynamicQuery { PageRequest = model, Dynamic = dynamic };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddCategory")]
    public async Task<IActionResult> AddCategory([FromForm] AddPackageCategoryModel model)
    {
        var command = new AddPackageCategoryCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateCategory")]
    public async Task<IActionResult> UpdateCategory([FromForm] UpdatePackageCategoryModel model)
    {
        var command = new UpdatePackageCategoryCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveCategory")]
    public async Task<IActionResult> PassiveCategory([FromBody] byte categoryId)
    {
        var command = new PassivePackageCategoryCommand { Id = categoryId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("ActiveCategory")]
    public async Task<IActionResult> ActiveCategory([FromBody] byte categoryId)
    {
        var command = new ActivePackageCategoryCommand { Id = categoryId };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetCategoriesForWeb")]
    public async Task<IActionResult> GetCategoriesForWeb([FromQuery] PageRequest model)
    {
        var query = new GetPackageCategoriesQuery { PageRequest = model, ForWeb = true };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion Category
}