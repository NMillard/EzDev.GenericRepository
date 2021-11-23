using System;
using EzDev.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EzDev.Examples.GenericRepository.SimpleImplementations;

public class SimpleEmployeeRepository : EntityRepositoryBase<Employee> {
    public SimpleEmployeeRepository(DbContext context) : base(context) { }
}

public class EmployeeRepositoryWithEvents : EntityRepositoryBase<Employee> {
    public EmployeeRepositoryWithEvents(DbContext context, ILogger logger) : base(context) {
        Events = new RepositoryEvents<Employee> {
            OnBeforeSaving = async employee => logger.LogInformation("Before saving employee {Id}", employee.Id),
            OnSaved = async employee => logger.LogInformation("Saved employee {Id}", employee.Id),
            OnSavingFailed = async (employee, exception) => logger.LogError("Saving employee {Id} failed with message {Message}", employee.Id, exception.Message)
        };
    }

    public EmployeeRepositoryWithEvents(DbContext context, RepositoryEvents<Employee> events) :
        base(context, events) { }
}

public class CompanyRepository : EntityRepositoryBase<Company> {
    public CompanyRepository(DbContext context) : base(context) { }

    public CompanyRepository(DbContext context, RepositoryEvents<Company> events) : base(context, events) {
        Entities = context.Set<Company>().Include(c => c.Employees).AsNoTracking();
    }
}

// Demo in memory db context
public class InMemoryDbContext : DbContext {
    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee>? Employees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
    }
}