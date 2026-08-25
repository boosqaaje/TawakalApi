using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TawakalApi.app.Models.Dtos.ResponseDtos;

namespace TawakalApi.app.Controllers;


[Authorize]
[ApiController]
[Produces("application/json")]

[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public abstract class BaseApiController : ControllerBase
{

    protected IActionResult OkCommonResponse(CommonRes data)
    {

        if (!data.Success)
        {
            return BadRequestResponse(data);
        }

        return OkResponse(data);
    }
    protected IActionResult LoginResponse(CommonRes data)
    {

        if (!data.Success)
        {
            return Unauthorized(data);
        }

        return OkResponse(data);
    }

    protected IActionResult CreatedCommonResponse(CommonRes data)
    {

        if (!data.Success)
        {
            return BadRequestResponse(data);
        }

        return CreatedResponse(data);
    }

    protected IActionResult OkResponse<T>(T data)
    {
        return Ok(data);
    }
    protected IActionResult CreatedResponse<T>(T data)
    {
        return StatusCode(StatusCodes.Status201Created, data);
    }

    protected IActionResult BadRequestResponse<T>(T data)
    {
        return BadRequest(data);
    }
}