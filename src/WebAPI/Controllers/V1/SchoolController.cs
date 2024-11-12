using Application.Features.Schools.Commands.ClassRooms;
using Application.Features.Schools.Commands.Schools;
using Application.Features.Schools.Models.ClassRooms;
using Application.Features.Schools.Models.Schools;
using Application.Features.Schools.Queries.ClassRooms;
using Application.Features.Schools.Queries.Schools;
using Application.Features.Teachers.Commands;
using Application.Features.Teachers.Models;
using Application.Features.Teachers.Queries;
using Asp.Versioning;

namespace WebAPI.Controllers.V1;

[ApiController]
[Route(ApiVersioningConfig.ControllerRouteWithoutApi)]
[ApiVersion("1")]
public class SchoolController : BaseController
{
    #region School

    [HttpGet("GetSchoolById/{id}")]
    public async Task<IActionResult> GetSchoolById([FromRoute] byte id)
    {
        var command = new GetSchoolByIdQuery { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetSchoolByTaxNumber/{taxNumber}")]
    public async Task<IActionResult> GetSchoolByTaxNumber(string taxNumber)
    {
        var command = new GetSchoolByTaxNumberQuery { TaxNumber = taxNumber };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetSchoolForDashboard")]
    public async Task<IActionResult> GetSchoolForDashboard()
    {
        var command = new GetSchoolDashboardQuery();
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetSchools")]
    public async Task<IActionResult> GetSchools([FromQuery] PageRequest model)
    {
        var command = new GetSchoolsQuery { PageRequest = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("GetSchoolsDynamic")]
    public async Task<IActionResult> GetSchoolsDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamicModel)
    {
        var command = new GetSchoolsByDynamicQuery { PageRequest = model, Dynamic = dynamicModel };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddSchool")]
    public async Task<IActionResult> AddSchool([FromBody] AddSchoolModel model)
    {
        var command = new AddSchoolCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateSchool")]
    public async Task<IActionResult> UpdateSchool([FromBody] UpdateSchoolModel model)
    {
        var command = new UpdateSchoolCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveSchool")]
    public async Task<IActionResult> PassiveSchool([FromBody] int schoolId)
    {
        var command = new PassiveSchoolCommand { Id = schoolId };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("ActiveSchool")]
    public async Task<IActionResult> ActiveSchool([FromBody] int schoolId)
    {
        var command = new ActiveSchoolCommand { Id = schoolId };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpGet("GetPackageSchools")]
    public async Task<IActionResult> GetPackageSchools()
    {
        var command = new GetPackageSchoolsQuery();
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion School

    #region ClassRoom

    [HttpGet("GetClassRoomById/{id}")]
    public async Task<IActionResult> GetClassRoomById([FromRoute] int id)
    {
        var command = new GetClassRoomByIdQuery { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetClassRooms")]
    public async Task<IActionResult> GetClassRooms([FromQuery] PageRequest model)
    {
        var command = new GetClassRoomsQuery { PageRequest = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("GetClassRoomsDynamic")]
    public async Task<IActionResult> GetClassRoomsDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamicModel)
    {
        var command = new GetClassRoomsByDynamicQuery { PageRequest = model, Dynamic = dynamicModel };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddClassRoom")]
    public async Task<IActionResult> AddClassRoom([FromBody] AddClassRoomModel model)
    {
        var command = new AddClassRoomCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateClassRoom")]
    public async Task<IActionResult> UpdateClassRoom([FromBody] UpdateClassRoomModel model)
    {
        var command = new UpdateClassRoomCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveClassRoom")]
    public async Task<IActionResult> PassiveClassRoom([FromBody] int classRoomId)
    {
        var command = new PassiveClassRoomCommand { Id = classRoomId };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("ActiveClassRoom")]
    public async Task<IActionResult> ActiveClassRoom([FromBody] int classRoomId)
    {
        var command = new ActiveClassRoomCommand { Id = classRoomId };
        await Mediator.Send(command);
        return Ok();
    }

    #endregion ClassRoom

    #region Teacher

    [HttpGet("GetTeacherById/{id}")]
    public async Task<IActionResult> GetTeacherById([FromRoute] byte id)
    {
        var command = new GetTeacherByIdQuery { Id = id };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetTeacherForDashboard")]
    public async Task<IActionResult> GetTeacherForDashboard()
    {
        var command = new GetTeacherDashboardQuery();
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetTeachers")]
    public async Task<IActionResult> GetTeachers([FromQuery] PageRequest model)
    {
        var command = new GetTeachersQuery { PageRequest = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("GetTeachersDynamic")]
    public async Task<IActionResult> GetTeachersDynamic([FromQuery] PageRequest model, [FromBody] Dynamic dynamicModel)
    {
        var command = new GetTeachersByDynamicQuery { PageRequest = model, Dynamic = dynamicModel };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("AddTeacher")]
    public async Task<IActionResult> AddTeacher([FromBody] AddTeacherModel model)
    {
        var command = new AddTeacherCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("UpdateTeacher")]
    public async Task<IActionResult> UpdateTeacher([FromBody] UpdateTeacherModel model)
    {
        var command = new UpdateTeacherCommand { Model = model };
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("PassiveTeacher")]
    public async Task<IActionResult> PassiveTeacher([FromBody] int teacherId)
    {
        var command = new PassiveTeacherCommand { Id = teacherId };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("ActiveTeacher")]
    public async Task<IActionResult> ActiveTeacher([FromBody] int teacherId)
    {
        var command = new ActiveTeacherCommand { Id = teacherId };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("AssignClassRoom")]
    public async Task<IActionResult> AssignClassRoom([FromBody] AssignTeacherClassRoomModel model)
    {
        var command = new AssignClassRoomCommand { Id = model.TeacherId, ClassRoomIds = model.ClassRoomIds };
        await Mediator.Send(command);
        return Ok();
    }

    [HttpPost("AssignLesson")]
    public async Task<IActionResult> AssignLesson([FromBody] AssignTeacherLessonModel model)
    {
        var command = new AssignLessonCommand { Id = model.TeacherId, LessonIds = model.LessonIds };
        await Mediator.Send(command);
        return Ok();
    }

    #endregion Teacher
}