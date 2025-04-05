using Mango.Service.AuthAPI.Models;
using Mango.Services.AuthAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//Below line is used to register the identity service in the application with the user and role for that we  we use default IdentityUser, IdentityRole. This line is used to add the identity service to the application.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>() // This line indicates that we are using the identity data in the database int AppDbContext class.
    .AddDefaultTokenProviders(); //this line is used to add the default token providers to the application
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
app.UseAuthentication(); //this line is used to add the authentication middleware to the application
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
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}