using System;
using System.Collections.Generic;

namespace EzDev.Examples.GenericRepository.SimpleImplementations; 

/*
 * Simple models used with the demo repository classes.
 * Don't implement your models in this way, with public setters, etc.
 */

public class Company {
    public Guid Id { get; set; }
    public List<Employee> Employees { get; set; }
}

public record Employee(Guid Id, string Name, DateOnly Birthday);
