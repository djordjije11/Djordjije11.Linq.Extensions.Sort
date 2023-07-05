# Sorter

## Djordjije11.Linq.Extensions.Sort

This small library is designed to facilitate the creation of sorting queries, building upon the original input query or enumerable.
It can be used for building SQL queries with EntityFramework, adding properites to ORDER BY clause.

### Installation

You can find the latest NuGet package [here](https://www.nuget.org/packages/Djordjije11.Linq.Extensions.Sort).

### Imports

```c#
using Djordjije11.Linq.Extensions.Sort
```

### Usage

Let's take as an example a domain model class Employee:

```c#
public class Employee
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Gender Gender { get; set; }
    public Status Status { get; set; }
}
```

As an example, if we have variable \_employees implementing IEnumerable<Employee> and we want to sort them by Status ascending firstly, then FirstName descending and then Id ascending, we could write a method like this:

```c#
public ICollection<Employee> GetEmployeesOrderedByStatusAscThenFirstNameDescThenIdAsc()
{
    query = new Sorter<Employee>(_employees)
          .Add(new(e => e.Status, true))
          .Add(new(e => e.FirstName, false))
          .Add(new(e => e.Id, true))
          .Build();
    return query.ToList();
}
```

Or:

```c#
public ICollection<Employee> GetEmployeesOrderedByStatusAscThenFirstNameDescThenIdAsc()
{
    query = new Sorter<Employee>()
          .Add(new(e => e.Status, true))
          .Add(new(e => e.FirstName, false))
          .Add(new(e => e.Id, true))
          .BuildOnEnumerable(_employees);
    return query.ToList();
}
```

The following example demonstrates how to use it with EntityFramework. In this example, the \_context variable represents an instance of EntityFramework's DbContext implementation.
So if we want to get all Employees from the database ordered by Status ascending firstly, then FirstName descending and then Id ascending, we could write a method like this:

```c#
public ICollection<Employee> GetEmployeesOrderedByStatusAscThenFirstNameDescThenIdAsc()
{
    IQueryable<Employee> query = _context.Employee;
    query = new Sorter<Employee>(query)
          .Add(new(e => e.Status, true))
          .Add(new(e => e.FirstName, false))
          .Add(new(e => e.Id, true))
          .Build();
    return query.ToList();
}
```

Or:

```c#
public ICollection<Employee> GetEmployeesOrderedByStatusAscThenFirstNameDescThenIdAsc()
{
    IQueryable<Employee> query = _context.Employee;
    query = new Sorter<Employee>()
          .Add(new(e => e.Status, true))
          .Add(new(e => e.FirstName, false))
          .Add(new(e => e.Id, true))
          .BuildOnQuery(query);
    return query.ToList();
}
```

It would be the same as if we wrote:

```c#
public ICollection<Employee> GetEmployeesOrderedByStatusAscThenFirstNameDescThenIdAsc()
{
    return _context.Employee.OrderBy(e => e.Status).ThenByDescending(e => e.FirstName).ThenBy(e => e.Id).ToList();
}
```

However, this approach lacks flexibility when attempting to create generic ways for constructing sorting queries.
As an example, we could create a class EmployeeSortOptions:

```c#
public class EmployeeSortOptions
{
    public bool? IdAsc { get; set; }
    public bool? FirstNameAsc { get; set; }
    public bool? LastNameAsc { get; set; }
    public bool? EmailAsc { get; set; }
    public bool? GenderAsc { get; set; }
    public bool? StatusAsc { get; set; }
}
```

And then in the SortEmployees method just establish the ordering priorities for properties, so it is left to properties of EmployeeSortOptions object to specify which properties are used for sorting and whether the sorting order is ascending or descending. If the value of the property is null, the property will not be ordered at all and will be ignored.

```c#
public IQueryable<Employee> SortEmployees(IQueryable<Employee> query, EmployeeSortOptions options)
{
    if(options != null)
    {
        query = new Sorter<Employee>(query)
          .Add(new(e => e.Status, options.StatusAsc))
          .Add(new(e => e.FirstName, options.FirstNameAsc))
          .Add(new(e => e.Gender, options.GenderAsc))
          .Add(new(e => e.LastName, options.LastNameAsc))
          .Add(new(e => e.Email, options.EmailAsc))
          .Add(new(e => e.Id, options.IdAsc))
          .Build();
    }
    return query;
}
```
