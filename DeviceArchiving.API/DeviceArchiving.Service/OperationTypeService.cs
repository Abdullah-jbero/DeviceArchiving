using DeviceArchiving.Data.Contexts;
using DeviceArchiving.Data.Entities;
using DeviceArchiving.Service;
using Microsoft.EntityFrameworkCore;

public class OperationTypeService : IOperationTypeService
{
    private readonly IDbContextFactory<DeviceArchivingContext> _contextFactory;

    public OperationTypeService(IDbContextFactory<DeviceArchivingContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public void AddOperationType(OperationType operationType)
    {
        using var context = _contextFactory.CreateDbContext();
        context.OperationsTypes.Add(operationType);
        context.SaveChanges();
    }

    public IEnumerable<OperationType> GetAllOperationsTypes(string? searchTerm)
    {
        using var context = _contextFactory.CreateDbContext();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            return context.OperationsTypes
                .Where(x => (x.Name != null && x.Name.Contains(searchTerm)) || (x.Description != null && x.Description.Contains(searchTerm)))
                .ToList();
        }
        return context.OperationsTypes.ToList();
    }

    public void UpdateOperationType(OperationType operationType)
    {
        using var context = _contextFactory.CreateDbContext();
        var existingOperationType = context.OperationsTypes.Find(operationType.Id);
        if (existingOperationType == null) throw new Exception("نوع العملية غير موجود");
        existingOperationType.Name = operationType.Name;
        existingOperationType.Description = operationType.Description;
        context.OperationsTypes.Update(existingOperationType);
        context.SaveChanges();
    }

    public void DeleteOperationType(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var operationType = context.OperationsTypes.Find(id);
        if (operationType != null)
        {
            context.OperationsTypes.Remove(operationType);
            context.SaveChanges();
        }
    }
}
