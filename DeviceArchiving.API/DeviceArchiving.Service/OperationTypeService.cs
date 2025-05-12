using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Entities;

namespace DeviceArchiving.Service
{
    public class OperationTypeService(DeviceArchivingContext context) : IOperationTypeService
    {
        public void AddOperationType(OperationType operationType)
        {
            context.OperationsTypes.Add(operationType);
            context.SaveChanges();
        }

        public IEnumerable<OperationType> GetAllOperationsTypes(string? searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                return context.OperationsTypes
                    .Where(x =>( x.Name != null && x.Name.Contains(searchTerm)) || (x.Description != null && x.Description.Contains(searchTerm)))
                    .ToList();
            }
            return context.OperationsTypes.ToList();
        }

        public void UpdateOperationType(OperationType operationType)
        {
            var existingOperationType = context.OperationsTypes.Find(operationType.Id);
            if (existingOperationType == null) throw new Exception("نوع العملية غير موجود");
            existingOperationType.Name = operationType.Name;
            existingOperationType.Description = operationType.Description;
            context.OperationsTypes.Update(existingOperationType);
            context.SaveChanges();
        }

        public void DeleteOperationType(int id)
        {
            var operationType = context.OperationsTypes.Find(id);
            if (operationType != null)
            {
                context.OperationsTypes.Remove(operationType);
                context.SaveChanges();
            }
        }
    }
}
