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
        Description = "Cool as heck",
        Specialty = "Facial expressions"
    },
    new Employee
    {
        Id = 2,
        Name = "James Acaster",
        Description = "Neato",
        Specialty = "Talking fast"
    },
    new Employee 
    { 
        Id = 3,
        Name = "Finn",
        Description = "Mathematical",
        Specialty = "Has a dog"
    },
    new Employee
    {
        Id = 4,
        Name = "Gothos",
        Description = "Jaghut",
        Specialty = "Not without folly"
    }
};
List<ServiceTicket> serviceTickets = new List<HoneyRaesAPI.Models.ServiceTicket> 
{
      new ServiceTicket
    {
        Id = 1,
        CustomerId = 2,
        EmployeeId = null,
        Description = "not good",
        Emergency = true,
    },
      new ServiceTicket
    {
        Id = 2,
        CustomerId = 3,
        EmployeeId = 1,
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
        DateCompleted = new DateTime(2022,12,20)
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

app.MapGet("/api/servicetickets", () =>
{ 
    if (serviceTickets == null)
    {
        return Results.NotFound("no tickets");
    }
    else
    {

    return Results.Ok(serviceTickets);
    }
});

app.MapGet("/api/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    serviceTicket.Customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/api/employee", () =>
{

    return employees;
    
});

app.MapGet("/api/employee/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if(employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/api/customer", () =>
{
    return customers;

});

app.MapGet("/api/customer/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(e => e.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(customer);
});

app.MapPost("/api/servicetickets", (ServiceTicket serviceTicket) =>
{  if (serviceTickets.Count() == 0)
    {
        serviceTicket.Id = 1;
        return Results.Ok(serviceTicket);
    } 
    else
    {
    serviceTicket.Id = serviceTickets.Max(st =>  st.Id) + 1;
    serviceTickets.Add(serviceTicket);

    }
    return Results.Ok(serviceTicket);
}
);

app.MapDelete("/api/servicetickets/{id}", (int id) =>
{
    ServiceTicket ticketToRemove = serviceTickets.Where(st => st.Id == id).FirstOrDefault();
    serviceTickets.Remove(ticketToRemove);
});

app.MapPut("/api/servicetickets/{id}", (int id, ServiceTicket updatedServiceTicket) =>
{
    ServiceTicket existingServiceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    existingServiceTicket.EmployeeId = updatedServiceTicket.EmployeeId;

    if (existingServiceTicket == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(existingServiceTicket);

});

app.MapPost("/api/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    ticketToComplete.DateCompleted = DateTime.Today;
    return Results.Ok("Ticket marked complete");
});

app.MapGet("/api/servicetickets/incompleteEmergencies", () =>
{
    List<ServiceTicket> IncompleteEmergencies = serviceTickets.Where(st => st.DateCompleted == DateTime.MinValue && st.Emergency == true).ToList();
    if(IncompleteEmergencies.Count == 0)
    {
        return Results.NotFound("All emergencies have been completed");
    }
    return Results.Ok(IncompleteEmergencies);
}
);

app.MapGet("/api/servicetickets/unassigned", () =>
{
    List<ServiceTicket> UnassignedTickets = serviceTickets.Where(st => st.EmployeeId == null).ToList();
    if(UnassignedTickets.Count == 0)
    {
        return Results.NotFound("All tickets have been assigned");
    }
    return Results.Ok(UnassignedTickets);
    
}
);

app.MapGet("/api/employee/available", () =>
{
    List<Employee> unassignedEmployees = employees.Where(e => !serviceTickets.Any(st => st.EmployeeId == e.Id && st.DateCompleted == DateTime.MinValue)).ToList();
    return unassignedEmployees;
    
});

app.MapGet("/api/customer/inactive", () =>
{
    List<Customer> inactiveCustomers = customers.Where(c => serviceTickets.Any(st => st.CustomerId == c.Id && (st.DateCompleted == DateTime.MinValue || st.DateCompleted == DateTime.Now.AddYears(-1)))).ToList();

    if (inactiveCustomers != null)
    {
        return Results.Ok(inactiveCustomers);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapGet("/api/employee/{id}/customers", (int id) =>
{
    
    Employee employee = employees.FirstOrDefault(e => e.Id == id);

    if (employee == null)
    {
        return Results.NotFound("Employee not found");
    }
    
    List<ServiceTicket> employeeServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();

    if (employeeServiceTickets.Count == 0)
    {
        return Results.Ok("This employee has no customers.");
    }
    
    List<int> customerIds = employeeServiceTickets.Select(st => st.CustomerId).Distinct().ToList();

    List<Customer> customersAssignedToEmployee = customers.Where(c => customerIds.Contains(c.Id)).ToList();

    return Results.Ok(customersAssignedToEmployee);
});

app.MapGet("/api/employee/topEmployee", () =>
{
    DateTime lastMonth = DateTime.Now.AddMonths(-1);
    Employee employeeOfMonth = employees.OrderByDescending(e => serviceTickets.Count(st => st.EmployeeId == e.Id && st.DateCompleted.HasValue && st.DateCompleted.Value.Month == lastMonth.Month)).FirstOrDefault();

    return Results.Ok(employeeOfMonth);
});

app.MapGet("/api/servicetickets/review", () =>
{
    List<ServiceTicket> completedTickets = serviceTickets.Where(st => st.DateCompleted.HasValue).OrderBy(st => st.DateCompleted).ToList();

    foreach (var ticket in completedTickets)
    {
        ticket.Customer = customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
        ticket.Employee = employees.FirstOrDefault(e => e.Id == ticket.EmployeeId);
    }

    return Results.Ok(completedTickets);
});

app.MapGet("/api/servicetickets/prioritized", () =>
{
    var prioritizedTickets = serviceTickets.Where(st => !st.DateCompleted.HasValue).OrderByDescending(st => st.Emergency).ThenBy(st => st.EmployeeId.HasValue) .ToList();

    foreach (var ticket in prioritizedTickets)
    {
        ticket.Customer = customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
        ticket.Employee = employees.FirstOrDefault(e => e.Id == ticket.EmployeeId);
    }

    return Results.Ok(prioritizedTickets);
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
