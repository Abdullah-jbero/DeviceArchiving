using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DeviceArchiving.Data;

public class UserIdInterceptor : ISaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserIdInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateUserIds(eventData.Context);
        return result;
    }

    public ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateUserIds(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void UpdateUserIds(DbContext? context)
    {
        if (context == null) return;

        // Get the current user's ID from the HTTP context (e.g., from JWT or authentication)
        var userId = GetCurrentUserId();
        if (userId == 0) return; // No user ID available, skip

        // Track entities that implement IAuditableEntity
        var entries = context.ChangeTracker
            .Entries<IAuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.UserId = userId;
            }
        }
    }

    private int GetCurrentUserId()
    {
        // Example: Extract UserId from ClaimsPrincipal (adjust based on your authentication setup)
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}