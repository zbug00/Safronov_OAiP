using WebApplication7;
class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        string? url = builder.Configuration["SupabaseSetting:ApiUrl"];
        string? key = builder.Configuration["SupabaseSetting:ApiKey"];
        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };
        Supabase.Client supabase = new Supabase.Client(url, key, options);
        SupaBaseContext supabaseContext = new();
        builder.Services.AddSingleton(supabase);
        builder.Services.AddSingleton(supabaseContext);
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}

