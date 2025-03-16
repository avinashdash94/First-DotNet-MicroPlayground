using AutoMapper;
using Mango.Services.CouponAPI;
using Mango.Services.CouponAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Register the mapping class for the application between the classes like Coupon and CouponDto
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper); //this line tells how it should inject the automapper in the application
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); //this line is used to inject the automapper in the application

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.Run();

//Bellow method is used to automatically apply the migration to the database if any migration is pending. In this way, we don't need to run the update-database command manually.
void ApplyMigration()
{
    using (var scop = app.Services.CreateScope())
    {
        var _db = scop.ServiceProvider.GetRequiredService<AppDbContext>();
        if(_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }          
     }
}
