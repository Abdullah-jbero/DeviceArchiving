using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeviceArchiving.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetAllDevicesDto>>> GetAll()
    {
        var devices = await _deviceService.GetAllDevicesAsync();
        return Ok(devices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetDeviceDto>> GetById(int id)
    {
        var device = await _deviceService.GetDeviceByIdAsync(id);
        if (device == null)
            return NotFound();

        return Ok(device);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeviceDto dto)
    {
        await _deviceService.AddDeviceAsync(dto);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDeviceDto dto)
    {
        try
        {
            await _deviceService.UpdateDeviceAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _deviceService.DeleteDeviceAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}
