# Easy Generic Repository

EzDev.GenericRepository is a very simplistic, lightweight generic repository based on EntityFramework Core, that doesn't lock you into a certain way of working. You're provided a single base class with simple CRUD-based operations, that you may override if you have other requirements.

## How do I get started?
Create a class that inherits from `EntityRepositoryBase` and implement its constructor.  
In its simplest form, you can have a repository such as below.

`````c#
public class SimpleEmployeeRepository : EntityRepositoryBase<Employee> {
    public SimpleEmployeeRepository(DbContext context) : base(context) { }
}
`````

That's honestly it.

The `SimpleEmployeeRepository` now has default implementations for getting, adding, updating, and deleting `Employee` entities.

### More advanced options
Say you have a `Company` type acting as an aggregate root with a list of employees, and you want to retrieve all employees whenever you query a company.

In this case, you may want to override the default `Entities` property on the `EntityRepositoryBase`, as demonstrated below.

````c#
public class CompanyRepository : EntityRepositoryBase<Company> {
    public CompanyRepository(DbContext context) : base(context) {
        Entities = context.Set<Company>().Include(c => c.Employees).AsNoTracking();
    }
}
````

### Extension and listening points
Take advantage of events to plug in your own code without having to override methods. This is great for implementing cross-cutting concerns such as logging.

You can listen to repository events in two ways: implement the methods directly in the repository, or, register them with the dependency injection framework.

````c#
public class EmployeeRepositoryWithEvents : EntityRepositoryBase<Employee> {
    public EmployeeRepositoryWithEvents(DbContext context, ILogger logger) : base(context) {
        Events = new RepositoryEvents<Employee> {
            OnBeforeSaving = async employee => logger.LogInformation("Before saving employee {Id}", employee.Id),
            OnSaved = async employee => logger.LogInformation("Saved employee {Id}", employee.Id),
            OnSavingFailed = async (employee, exception) => logger.LogError("Saving employee {Id} failed with message {Message}", employee.Id, exception.Message)
        };
    }
}
````

If you don't want to pollute your repository with logging statements, then you can register the `RepositoryEvents<T>` with your dependency container framework, such as below.

````c#
public class EmployeeRepositoryWithEvents : EntityRepositoryBase<Employee> {
    public EmployeeRepositoryWithEvents(DbContext context, RepositoryEvents<Employee> events) :
        base(context, events) { }
}

// In Startup.cs (or elsewhere)

   services.AddScoped<RepositoryEvents<Employee>>(provider => {
              var logger = provider.GetRequiredService<ILogger<RepositoryEvents<Employee>>>();
              return new RepositoryEvents<Employee> {
                  OnBeforeSaving = async employee => logger.LogInformation("Before saving employee {Id}", employee.Id),
                 OnSaved = async employee => logger.LogInformation("Saved employee {Id}", employee.Id),
                  OnSavingFailed = async (employee, exception) => logger.LogError("Saving employee {Id} failed with message {Message}", employee.Id, exception.Message)
              };
            })
           .AddScoped<EmployeeRepositoryWithEvents>()
````