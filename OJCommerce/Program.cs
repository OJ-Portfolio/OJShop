using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OJCommerce.Data;
using OJCommerce.Mapper;
using OJCommerce.Middlewares;
using OJCommerce.Repositories.Categories;
using OJCommerce.Repositories.Products;
using OJCommerce.Repositories.Users;
using OJCommerce.Services.Categories;
using OJCommerce.Services.Products;
using OJCommerce.Services.Users;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using OJCommerce.Services.Roles;
using OJCommerce.Repositories.Vendors;
using OJCommerce.Services.Vendors;
using OJCommerce.Helpers;
using OJCommerce.Repositories.Carts;
using OJCommerce.Services.Carts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(builder.Configuration.GetConnectionString("Default"),
new MySqlServerVersion(new Version(9, 1, 0))

)
);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromDays(7)   // Token expires every 7 days
    };

    // CUSTOM ERROR RESPONSES
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse(); // Prevent default 401 response
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var json = "{\"success\": false, \"message\": \"You must login to access this resource.\"}";
            return context.Response.WriteAsync(json);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var json = "{\"success\": false, \"message\": \"You do not have permission to perform this action.\"}";
            return context.Response.WriteAsync(json);
        }
    };
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<VendorDomainService>();
builder.Services.AddScoped<ICartRepository,CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add JWT Bearer authorization
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
}); builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Enable Swagger in development
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
