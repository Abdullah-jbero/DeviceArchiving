using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceArchiving.Service
{
    public class OperationService(DeviceArchivingContext context) : IOperationService
    {

        public async Task AddOperations(Operation operation)
        {
            context.Operations.Add(operation);
            await context.SaveChangesAsync();
        }

        public Task<List<Operation>> GetAllOperations(int deviceId)
        {
            return context.Operations.Where(o=>o.DeviceId == deviceId).ToListAsync();
        }


    }

}
