using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Profiles;
using DopamineDetoxAPI.Services;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenAI.Chat;
using OpenAI.Images;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddMemoryCache();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ApplicationUserMappingProfile>();
    cfg.AddProfile<ContentTypeMappingProfile>();
    cfg.AddProfile<TopicMappingProfile>();
    cfg.AddProfile<SubTopicMappingProfile>();
    cfg.AddProfile<ChannelMappingProfile>();
    cfg.AddProfile<HistorySearchResultMappingProfile>();
    cfg.AddProfile<SearchResultMappingProfile>();
    cfg.AddProfile<NoteMappingProfile>();
    cfg.AddProfile<TopSearchResultMappingProfile>();
    cfg.AddProfile<SearchResultReportMappingProfile>();
    cfg.AddProfile<WeeklySearchResultReportMappingProfile>();
    cfg.AddProfile<DefaultTopicMappingProfile>();
    cfg.AddProfile<QuoteMappingProfile>();
    cfg.AddProfile<LearnMoreDetailsMappingProfile>();
});

builder.Services.Configure<NotificationSettings>(
    builder.Configuration.GetSection("NotificationSettings"));

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IContentTypeService, ContentTypeService>();
builder.Services.AddScoped<ISearchResultService, SearchResultService>();
builder.Services.AddScoped<ISearchResultReportService, SearchResultReportService>();
builder.Services.AddScoped<IWeeklySearchResultReportService, WeeklySearchResultReportService>();
builder.Services.AddScoped<ITopSearchResultService, TopSearchResultService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<ISubTopicService, SubTopicService>();
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<IHistorySearchResultService, HistorySearchResultService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddScoped<CacheService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDefaultTopicService, DefaultTopicService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// In Program.cs, modify the OpenAI client registration
builder.Services.AddSingleton<ChatClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var settings = config.GetSection("AppSettings").Get<AppSettings>();
    var apiKey = settings?.OPENAI_API_KEY ?? throw new InvalidOperationException("OpenAI API key not configured");
    
    return new ChatClient("gpt-4", apiKey);
});

builder.Services.AddSingleton<ImageClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var settings = config.GetSection("AppSettings").Get<AppSettings>();
    var apiKey = settings?.OPENAI_API_KEY ?? throw new InvalidOperationException("OpenAI API key not configured");
    
    return new ImageClient("dall-e-3", apiKey);
});

// Configure HTTP client with timeout from settings
builder.Services.AddHttpClient("OpenAI", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var settings = config.GetSection("AppSettings").Get<AppSettings>();
    client.Timeout = TimeSpan.FromSeconds(settings?.OpenAIImageTimeoutSeconds ?? 120);
});

builder.Services.AddScoped<IQuoteService, QuoteService>();


var twitterEmbedUrl = configuration["TwitterAPI:timelineEmbedUrl"] ?? "";

// python web scraper to get twitter results
builder.Services.AddHttpClient<IChannelService, ChannelService>(client =>
{
    client.BaseAddress = new Uri(twitterEmbedUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
})
.AddGoogle(options =>
{
    options.ClientId = configuration["Authentication:Google:ClientId"] ?? "";
    options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? "";
});

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Setting up database connection... {connectionString}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Ensure roles are created
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await CreateRoles(roleManager);
}

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Add detailed error handling
//app.Use(async (context, next) =>
//{
//    try
//    {
//        await next();
//    }
//    catch (Exception ex)
//    {
//        var logger = context.RequestServices
//            .GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "Unhandled exception");
//        throw;
//    }
//});
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();


async Task CreateRoles(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Admin" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}