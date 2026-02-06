using dotnet05_api_base.Models;

var builder = WebApplication.CreateBuilder(args);

//DI Services swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DI Service EF-context
//connection string : Mỗi hệ quản trị csdl khác nhau sẽ có connectionstring khác nhau

builder.Services.AddDbContext<QuanLySanPhamContext>();



//DI Service entity framework core CybersoftMarketplaceContext
builder.Services.AddDbContext<CybersoftMarketplaceContext>();



//DI service controller 
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//Sử dụng middleware map controller
app.MapControllers();



app.Run();
