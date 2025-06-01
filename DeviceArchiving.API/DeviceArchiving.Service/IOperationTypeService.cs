using DeviceArchiving.Data.Entities;

namespace DeviceArchiving.Service;
public interface IOperationTypeService
{
    void AddOperationType(OperationType operationType);
    IEnumerable<OperationType> GetAllOperationsTypes(string? searchTerm);
    void UpdateOperationType(OperationType operationType);
    void DeleteOperationType(int id);

}
