using System;
using System.Threading.Tasks;
using EzDev.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EzDev.Examples.GenericRepository.SimpleImplementations;


public class Samples {
    private readonly ITestOutputHelper outputHelper;

    public Samples(ITestOutputHelper outputHelper) => this.outputHelper = outputHelper;

    [Fact]
    public async Task SimpleInstantiation() {
        var repository = new SimpleEmployeeRepository(new InMemoryDbContext());
        
        var id = Guid.NewGuid();
        await repository.AddAsync(new Employee(id, "Faxe Kondi", new DateOnly(1991, 11, 26)));
        Employee? employee = await repository.GetAsync(emp => emp.Id == id);
        
        Assert.NotNull(employee);
    }

    [Fact]
    public async Task InstantiateWithEventsFromDependencyContainer() {
        // Just like how you'd register services in an aspnetcore app.
        ServiceProvider provider = new ServiceCollection()
            .AddScoped<DbContext, InMemoryDbContext>()
            .AddScoped<RepositoryEvents<Employee>>(provider => {
                var logger = provider.GetRequiredService<ILogger<RepositoryEvents<Employee>>>();
                return new RepositoryEvents<Employee> {
                    OnBeforeSaving = async employee => logger.LogInformation("Before saving employee {Id}", employee.Id),
                    OnSaved = async employee => logger.LogInformation("Saved employee {Id}", employee.Id),
                    OnSavingFailed = async (employee, exception) => logger.LogError("Saving employee {Id} failed with message {Message}", employee.Id, exception.Message)
                };
            })
            .AddScoped<EmployeeRepositoryWithEvents>()
            .BuildServiceProvider();

        var repository = provider.GetRequiredService<EmployeeRepositoryWithEvents>();
        
        var id = Guid.NewGuid();
        await repository.AddAsync(new Employee(id, "Faxe Kondi", new DateOnly(1991, 11, 26)));
        Employee? employee = await repository.GetAsync(emp => emp.Id == id);
        
        Assert.NotNull(employee);
    }
}