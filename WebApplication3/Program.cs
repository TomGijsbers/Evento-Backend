using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace WebApplication3;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
       // builder.Services.AddAuthentication().AddJwtBearer();
       builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               // == jouw Auth0 tenant ==
               options.Authority = "https://dev-6cbzmmad8bpcv3o6.eu.auth0.com/";

               // == IDENTIFIER (Audience) van je API in Auth0 ==
               options.Audience  = "https://localhost:7100";

               // Alleen voor local dev: self-signed cert toestaan
               if (builder.Environment.IsDevelopment())
                   options.RequireHttpsMetadata = false;

               // (niet verplicht, wel handig voor policies/claims)
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   NameClaimType = ClaimTypes.NameIdentifier
               };
           });

        builder.Services.AddAuthorization(options =>
        {
            //We create different policies where each policy contains the permissions required to fulfill them
            options.AddPolicy("CanReadOwnProfile", p =>
                p.RequireClaim("permissions", "read:profile:own"));
            
            options.AddPolicy("CanReadEvents", p => 
                p.RequireClaim("permissions", "read:events"));

            options.AddPolicy("CanCreateEvent", p =>
                    p.RequireClaim("permissions", "create:event"));
            
            options.AddPolicy("CanUpdateEvents", p =>
                p.RequireClaim("permissions", "update:event:own"));
            
            options.AddPolicy("CanDeleteEvents", p => 
                p.RequireClaim("permissions", "delete:event:own"));

            options.AddPolicy("CanReadLocations", p =>
                p.RequireClaim("permissions", "read:locations"));
            
            options.AddPolicy("CanCreateLocations", p =>
                p.RequireClaim("permissions", "create:locations"));
            options.AddPolicy("CanDeleteLocations", p =>
                p.RequireClaim("permissions", "delete:locations"));
            
            options.AddPolicy("CanCreateRegistration", p =>
                p.RequireClaim("permissions", "create:registration:own"));
            
            options.AddPolicy("CanDeleteRegistration", p =>
                p.RequireClaim("permissions", "delete:registration:own"));
            
            options.AddPolicy("CanUpdateOwnProfile", p => 
                p.RequireClaim("permissions", "update:profile:own"));   
            options.AddPolicy("CanReadOwnProfile", policy =>
                policy.RequireClaim("permissions", "read:profile:own"));
                
            
            options.AddPolicy("CanReadGroups", policy =>
                policy.RequireClaim("permissions", "read:groups"));

            options.AddPolicy("CanCreateGroups", policy =>
                policy.RequireClaim("permissions", "create:groups"));

            options.AddPolicy("CanUpdateGroups", policy =>
                policy.RequireClaim("permissions", "update:groups"));

            options.AddPolicy("CanDeleteGroups", policy =>
                policy.RequireClaim("permissions", "delete:groups"));
            
                
        });        
        
        builder.Services.AddHttpClient();   
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddSwaggerService();
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            // Or for SQLite: options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

        var app = builder.Build();
        
        // Seed database data
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            DbInitializer.Initialize(context);
        }


        app.UseCors(options =>
        {
            options.AllowAnyHeader();
            options.AllowAnyMethod();
            options.WithOrigins("https://localhost:4200", "http://localhost:4200");
        });
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication(); 
        app.UseAuthorization();
        


        app.MapControllers();

        app.Run();
    }
}