using ConfigTesting.Helper;
using ConfigTesting.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configure section to be used via DI.
builder.Services.Configure<SecretsOptions>(builder.Configuration.GetSection(SecretsOptions.Secrets));

builder.Services.AddTransient<IConfigurationUpdater>(c => {
    var configRoot = builder.Configuration as IConfigurationRoot;
    return new ConfigurationUpdater(configRoot, "appsettings.json");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPut("/updateconfig/{newValue}", (
    string newValue,
     IOptionsMonitor<SecretsOptions> iSecretsOptions,
     IConfiguration config, 
     IConfigurationUpdater updater) =>
{
    //Bind section to the SecretsOptions class
    // IConfigurationSection section = config.GetSection(SecretsOptions.Secrets);
    // var secretsOptions = new SecretsOptions();
    // section.Bind(secretsOptions);

    //getting SecretsOption loaded in the DI with IOptions
    var secretsOptions = iSecretsOptions.CurrentValue;

    // Get the current value from the binded property
    var bindBeforeChange = secretsOptions.SecretA;

    // Get the current value from the configuration
    var oldConfigValue = config["Secrets:SecretA"];

    // Update the configuration
    updater.UpdateAppSettings("Secrets", "SecretA", newValue);

    // Get the new value from the configuration
    var newConfigValue = config["Secrets:SecretA"];

    // Get the new value from the binded property
    secretsOptions = iSecretsOptions.CurrentValue;
    var bindAfterChange = secretsOptions.SecretA;

    if (oldConfigValue == newConfigValue)
    {
        return Results.BadRequest("Failed to update configuration");
    }

    return Results.Ok(new ConfigChangeResponse
    {
        OldValue = oldConfigValue ?? string.Empty,
        NewValue = newConfigValue ?? string.Empty,
        BindBeforeChange = bindBeforeChange ?? string.Empty,
        BindAfterChange = bindAfterChange ?? string.Empty,
    });
})
.WithName("ConfigUpdater")
.WithOpenApi();

app.MapGet("/getsecreta", (IConfiguration configuration) => {
    return configuration["Secrets:SecretA"];
})
.WithName("GetCurrentValue")
.WithOpenApi();

app.Run();
