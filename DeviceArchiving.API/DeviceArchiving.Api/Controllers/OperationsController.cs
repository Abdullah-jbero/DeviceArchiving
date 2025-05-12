using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using Microsoft.AspNetCore.Mvc;

namespace DeviceArchiving.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OperationsController : ControllerBase
{
    private readonly IOperationService operationService;

    public OperationsController(IOperationService operationService)
    {
        this.operationService = operationService;
    }

    [HttpPost]
    public async Task<ActionResult<Operation>> PostOperation(Operation operation)
    {
        await operationService.AddOperations(operation);
        return CreatedAtAction(nameof(GetAllOperations), new { deviceId = operation.DeviceId }, operation);
    }

    [HttpGet("drive/{deviceId}")]
    public async Task<ActionResult<List<Operation>>> GetAllOperations(int deviceId)
    {
        var operations = await operationService.GetAllOperations(deviceId);
        return Ok(operations);
    }
}