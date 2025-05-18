using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceArchiving.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        this.deviceService = deviceService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Device>> GetDevices()
    {
        var devices = deviceService.GetAllDevices();
        return Ok(devices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Device>> GetDevice(int id)
    {
        var device = deviceService.GetAllDevices().FirstOrDefault(d => d.Id == id);
        if (device == null) return NotFound();
        return device;
    }

    [HttpPost]
    public ActionResult<Device> PostDevice(Device device)
    {
        deviceService.AddDevice(device);
        return CreatedAtAction(nameof(GetDevice), new { id = device.Id }, device);
    }

    [HttpPut("{id}")]
    public IActionResult PutDevice(int id, Device device)
    {
        if (id != device.Id) return BadRequest();

        try
        {
            deviceService.UpdateDevice(device);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteDevice(int id)
    {
        try
        {
            deviceService.DeleteDevice(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        return NoContent();
    }
}