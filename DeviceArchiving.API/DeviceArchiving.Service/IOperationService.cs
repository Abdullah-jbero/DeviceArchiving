using DeviceArchiving.Data.Entities;

namespace DeviceArchiving.Service
{
    public interface IOperationService
    {
        Task AddOperations(Operation operation);
        Task<List<Operation>> GetAllOperations(int driveId);

    }

}
