using Ecommerce.API.Data;
using Ecommerce.API.DTOs;
using Ecommerce.API.DTOs.Cloudinary;
using Ecommerce.API.Middleware;
using Ecommerce.API.Services;
using Ecommerce.API.Services.Conversations;
using Ecommerce.API.Services.Messages;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Cloudinary
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));
    
// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// Add Authorization
builder.Services.AddAuthorization();

// Register Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IOrderEmailService, OrderEmailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICouponService, CouponService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessagesService, MessagesService>();
builder.Services.AddScoped<IWithdrawService, WithdrawService>();

// Add CORS
builder.Services.AddCors(options =>
{
   
    options.AddPolicy("AllowFrontend",
       policy =>
       {
           policy.WithOrigins("http://localhost:3000")
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials();
       });
});

// Add Logging
builder.Services.AddLogging();

// register and validate EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //we are not use this for now we use swagger
   // app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        // Configure the default expansion level:
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Collapses all operations and tags
                                                                            // Or use:
                                                                            // c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // Expands only the tags, collapses operations
                                                                            // c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.Full); // Expands everything
    });
}


app.UseHttpsRedirection();

// Add CORS
//app.UseCors("AllowAll");
app.UseCors("AllowFrontend");


// Add Error Handling Middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
