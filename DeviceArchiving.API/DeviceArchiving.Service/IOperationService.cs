using DeviceArchiving.Data.Dto;
using DeviceArchiving.Data.Entities;

namespace DeviceArchiving.Service
{
    public interface IOperationService
    {
        Task AddOperations(CreateOperation operation);
        Task<List<Operation>> GetAllOperations(int driveId);

    }

}
