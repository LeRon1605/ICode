using ICode.CodeExecutor.Models;
using ICode.CodeExecutor.Runner;
using ICode.CodeExecutor.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(config =>
{
    config.AddPolicy("ICode", builder => builder.WithOrigins("*")
                                                .AllowAnyHeader()
                                                .WithMethods("PUT", "DELETE", "GET", "POST")
                    );
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("ICode");

Runner runner = new Runner();

app.MapPost("/execute", async (CodeExecutorRequest data) =>
{
    IDictionary<string, string[]> validationResult = data.IsValid();
    if (validationResult.Count > 0) 
    {
        return Results.ValidationProblem(validationResult);
    }   
    return Results.Ok(await runner.Execute(data.Code, data.Lang, data.Input));
});

Task.Run(async () => {
    const int INTERVAL_TIME = 1 * 60 * 1000;
    while (true) {
        await Cleaner.Execute();
        await Task.Delay(INTERVAL_TIME);
    }
});

app.Run();