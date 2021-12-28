using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EzDev.GenericRepository;
using EzDev.GenericRepositoryTests.Models;
using Xunit;

namespace EzDev.GenericRepositoryTests; 

public class EntityRepositoryShould {

    [Fact]
    public async Task QueryAllEntries() {
        // Arrange
        var returnData = new List<Employee> { new(), new(), new() };
        var testDbContext = new TestDbContext();
        await testDbContext.AddRangeAsync(returnData);
        await testDbContext.SaveChangesAsync();
        
        var sut = new EmployeeTestRepository(testDbContext);

        // Act
        IEnumerable<Employee> result = await sut.GetAllAsync();
        
        Assert.Equal(returnData.Count, result.Count());
    }

    [Fact]
    public async Task QueryForSingleEntry() {
        // Arrange
        var entryToFind = new Employee();
        
        var returnData = new List<Employee> { entryToFind, new(), new() };
        var testDbContext = new TestDbContext();
        await testDbContext.AddRangeAsync(returnData);
        await testDbContext.SaveChangesAsync();
        
        var sut = new EmployeeTestRepository(testDbContext);

        Employee? result = await sut.GetAsync(e => e.Id == entryToFind.Id);
        
        Assert.NotNull(result);
        Assert.Equal(entryToFind.Id, result!.Id);
    }
    
    [Fact]
    public async Task CallEvents() {
        // Arrange
        var onBeforeSaveCalledTimes = 0;
        var onSavedCalledTimes = 0;
        var repositoryEvents = new RepositoryEvents<Employee> {
            OnBeforeSaving = _ => Task.FromResult(onBeforeSaveCalledTimes++),
            OnSaved = _ => Task.FromResult(onSavedCalledTimes++)
        };
        var sut = new EmployeeTestRepository(new TestDbContext(), repositoryEvents);

        var entry = new Employee();
        // Act
        await sut.AddAsync(entry);
        await sut.UpdateAsync(entry);
        
        Assert.Equal(2, onBeforeSaveCalledTimes);
        Assert.Equal(2, onSavedCalledTimes);
    }
}