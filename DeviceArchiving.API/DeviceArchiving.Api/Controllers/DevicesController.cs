using ClosedXML.Excel;
using DeviceArchiving.Data.Dto;
using DeviceArchiving.Data.Dto.Devices;
using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


    [HttpPost("upload")]
    [ProducesResponseType(typeof(BaseResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<int>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<int>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(BaseResponse<int>.Failure("·„ Ì „ —›⁄ √Ì „·›"));

        if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
            return BadRequest(BaseResponse<int>.Failure(" ‰”Ìﬁ «·„·› €Ì— ’«·Õ. Ì—ÃÏ —›⁄ „·› Excel (.xlsx √Ê .xls)"));

        var errors = new List<string>();
        var devices = new List<CreateDeviceDto>();

        try
        {
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed().Skip(1); // Skip table header
            int rowNumber = 2;

            foreach (var row in rows)
            {
                try
                {
                    var device = new CreateDeviceDto
                    {
                        Source = row.Cell(2).GetString(),
                        BrotherName = row.Cell(3).GetString(),
                        LaptopName = row.Cell(4).GetString(),
                        SystemPassword = row.Cell(5).GetString(),
                        WindowsPassword = row.Cell(6).GetString(),
                        HardDrivePassword = row.Cell(7).GetString(),
                        FreezePassword = row.Cell(8).GetString(),
                        Code = row.Cell(9).GetString(),
                        Type = row.Cell(10).GetString(),
                        SerialNumber = row.Cell(11).GetString(),
                        Card = row.Cell(12).GetString(),
                        Comment = row.Cell(13).IsEmpty() ? null : row.Cell(13).GetString(), // Fixed: Use correct cell
                        ContactNumber = row.Cell(14).IsEmpty() ? null : row.Cell(14).GetString() // Fixed: Use correct cell
                    };

                    // Optional: Add validation for required fields
                    if (string.IsNullOrWhiteSpace(device.SerialNumber))
                        throw new Exception("—ﬁ„ «· ”·”· „ÿ·Ê»");

                    devices.Add(device);
                }
                catch (Exception ex)
                {
                    errors.Add($"«·”ÿ— {rowNumber}: Œÿ√ ›Ì „⁄«·Ã… «·»Ì«‰«  - {ex.Message}");
                }
                rowNumber++;
            }

            if (errors.Any())
            {
                return BadRequest(BaseResponse<int>.Failure($" „ «·⁄ÀÊ— ⁄·Ï √Œÿ«¡ ›Ì «·„·›: {string.Join("; ", errors)}"));
            }

            await _deviceService.AddDevicesAsync(devices);

            return Ok(BaseResponse<int>.SuccessResult(devices.Count, " „ —›⁄ «·„·› »‰Ã«Õ"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<int>.Failure($"Œÿ√ ⁄«„ ›Ì „⁄«·Ã… «·„·›: {ex.Message}"));
        }
    }


}
