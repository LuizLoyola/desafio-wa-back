using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// get connection string from uncommitted file
var connectionString = File.ReadAllText("connectionString.txt");

// add EF
builder.Services.AddDbContext<Context>(options => { options.UseSqlServer(connectionString); });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.MapGet("/", async (HttpContext context, Context dbContext) =>
{
    // validate header
    var header = context.Request.Headers["Authorization"].FirstOrDefault();

    if (header == null || !header.StartsWith("Basic "))
    {
        context.Response.StatusCode = 401;

        // return www authenticate
        context.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Authentication Required\"");

        return;
    }

    // get credentials
    var credentialsEncoded = header.Substring("Basic ".Length).Trim();
    var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(credentialsEncoded));
    var parts = credentials.Split(':');
    var username = parts[0];
    var password = parts[1];

    // simple check
    if (username != "admin" || password != "1234abcd")
    {
        context.Response.StatusCode = 401;
        return;
    }

    // get page index
    var pageIdx = int.Parse(context.Request.Query["pageIndex"].FirstOrDefault() ?? "0");

    const int pageSize = 20;

    // get orders including related products and delivery team
    var orders = await dbContext.Orders
        .Include(o => o.DeliveryTeam)
        .Include(o => o.Products)
        .ThenInclude(p => p.Product)
        .Skip(pageIdx * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // get total count
    var totalCount = await dbContext.Orders.CountAsync();

    var ordersSerializable = new
    {
        Orders = orders
            .Select(o => new
            {
                o.Number,
                o.CreationDate,
                o.DeliveryDate,
                o.Address,
                o.DeliveryTeam,
                Products = o.Products.Select(p => new
                {
                    p.Id,
                    p.Product.Name,
                    p.Product.Description,
                    p.Product.Price,
                    p.Quantity
                })
            })
            .ToList(),
        OrderCount = totalCount
    };

    // serialize to json using camelcase and return
    var json = JsonSerializer.Serialize(ordersSerializable, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(json);
});

app.Run("http://*:5050");