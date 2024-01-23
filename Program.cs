using HoneyRaesAPI.Models;

List<Customer> customers = new List<Customer> 
{ 
    new Customer
    {
        Id = 1, 
        Name = "Johnny Saniat",
        Address = "coolsville, Nashville"
    },
    new Customer 
    {
        Id = 2, 
        Name = "Brandon Schnurbusch",
        Address = " Murfreesboro, Somewhere"
     },
    new Customer
    {
        Id = 3,
        Name = "Ryan Shore",
        Address = "somewhere, over the rainbow"
    }
};
List<Employee> employees = new List<HoneyRaesAPI.Models.Employee> 
{
    new Employee 
    {
        Id = 1,
        Name = "Jim Carrey",
        Description = "Cool as heck"
    },
    new Employee
    {
        Id = 2,
        Name = "James Acaster",
        Description = "Neato"
    },
};
List<ServiceTicket> serviceTickets = new List<HoneyRaesAPI.Models.ServiceTicket> 
{
      new ServiceTicket
    {
        Id = 1,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "not good",
        Emergency = true,
    },
      new ServiceTicket
    {
        Id = 2,
        CustomerId = 3,
        EmployeeId = 2,
        Description = "this is an easy fix",
        Emergency = false,
        DateCompleted = new DateTime(2023,12,2)
    }, 
      new ServiceTicket
    {
        Id = 3,
        CustomerId = 1,
        Description = "proabably okay",
        Emergency = false,
    },  
      new ServiceTicket
    {
        Id = 4,
        CustomerId = 2,
        EmployeeId = 1,
        Description = "needs attention immediately",
        Emergency = true,
        DateCompleted = new DateTime(2023,12,20)
    }, 
    new ServiceTicket
    {
        Id = 5,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "standard checkup",
        Emergency = false,

    },
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    return serviceTickets.FirstOrDefault(t => t.Id == id);
});

app.MapGet("/employee", () =>
{
    return employees;
});

app.MapGet("/employee/{id}", (int id) =>
{
    return employees.FirstOrDefault(e => e.Id == id);
});

app.MapGet("/customer", () =>
{
    return customers;
});

app.MapGet("/customer/{id}", (int id) =>
{
    return customers.FirstOrDefault(e => e.Id == id);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
