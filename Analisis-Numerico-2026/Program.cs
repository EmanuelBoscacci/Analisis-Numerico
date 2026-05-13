using Analisis_Numerico_2026.Services.Unit1;
using Analisis_Numerico_2026.Services.Unit2;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Analisis_Numerico_2026
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Forzar InvariantCulture globalmente: punto como separador decimal
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddScoped<FunctionEvaluatorService>();
            builder.Services.AddTransient<RootFindingService>();
            builder.Services.AddTransient<GaussJordanService>();
            builder.Services.AddTransient<GaussSeidelService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
