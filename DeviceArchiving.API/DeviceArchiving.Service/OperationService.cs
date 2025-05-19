using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Dto;
using DeviceArchiving.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceArchiving.Service
{
    public class OperationService(DeviceArchivingContext context) : IOperationService
    {

        public async Task AddOperations(CreateOperation createOperation)
        {

            var operation = new Operation()
            {
                Comment = createOperation.Comment,
                NewValue = createOperation.NewValue,
                OldValue = createOperation.OldValue,
                OperationName = createOperation.OperationName,
                DeviceId = createOperation.DeviceId,
                CreatedAt = DateTime.Now

            };
            context.Operations.Add(operation);
            await context.SaveChangesAsync();
        }

        public Task<List<Operation>> GetAllOperations(int deviceId)
        {
            return context.Operations.Where(o=>o.DeviceId == deviceId).ToListAsync();
        }


    }

}
