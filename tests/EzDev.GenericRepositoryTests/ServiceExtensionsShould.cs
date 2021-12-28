using System.Threading.Tasks;
using EzDev.GenericRepository;
using EzDev.GenericRepositoryTests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EzDev.GenericRepositoryTests;

public class ServiceExtensionsShould {
    private readonly IServiceCollection services;
    
    public ServiceExtensionsShould() {
        services = new ServiceCollection();
        services.AddDbContext<DbContext, TestDbContext>();
    }
    
    [Fact]
    public void CanAddRepositoryBaseToServiceCollection() {
        // Act
        services.AddRepository<Employee, EmployeeTestRepository>();
        
        // Assert
        var repository = services.BuildServiceProvider().GetRequiredService<EntityRepository<Employee>>();
        Assert.NotNull(repository);
    }

    [Fact]
    public void CanAddRepositoryEvents() {
        // Arrange
        services.AddRepository<Employee, EmployeeTestRepository>()
            // Act
            .WithEvents<Employee>(_ => { });
        
        // Assert
        var sut = services.BuildServiceProvider().GetService<RepositoryEvents<Employee>>();
        Assert.NotNull(sut);
    }
}