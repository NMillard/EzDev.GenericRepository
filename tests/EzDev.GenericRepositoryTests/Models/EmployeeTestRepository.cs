using System;
using EzDev.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace EzDev.GenericRepositoryTests.Models; 

public class EmployeeTestRepository : EntityRepository<Employee> {
    public EmployeeTestRepository(DbContext context) : base(context) { }
    public EmployeeTestRepository(DbContext context, RepositoryEvents<Employee> events) : base(context, events) { }
}

public class TestDbContext : DbContext {
    public DbSet<Employee> Employees { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
    }
}