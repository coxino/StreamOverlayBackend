using DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Youtube_Contractor;

internal class Program
{
    //public static int version = 10001;
    private static YoutubeChatWriter youtubeChatWriter;
    public static YoutubeChatWriter YoutubeChatWriter
    {
        get
        {
            if (youtubeChatWriter != null)
            {
                return youtubeChatWriter;
            }
            else
            {
                youtubeChatWriter = new YoutubeChatWriter();
                return youtubeChatWriter;
            }
        }
    }

    private static void Main(string[] args)
    {
        //bool updateDB = true;

        //System.IO.DirectoryInfo di = new DirectoryInfo(Settings.ProjectSettings.DatabaseFolder);

        //foreach (FileInfo file in di.GetFiles())
        //{
        //    file.Delete();
        //}
        //foreach (DirectoryInfo dir in di.GetDirectories())
        //{
        //    dir.Delete(true);
        //}

        //if (!Directory.Exists(Settings.ProjectSettings.DatabaseFolder) || updateDB)
        //{
        //    Directory.CreateDirectory(Settings.ProjectSettings.DatabaseFolder);
        
        //    DirectoryInfo diSource = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"/Resources/database/");
        //    DirectoryInfo diTarget = new DirectoryInfo(Settings.ProjectSettings.DatabaseFolder);

        //    CopyAll(diSource, diTarget);
        //}

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),ServiceLifetime.Transient);

        
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
            app.UseSwagger();
            app.UseSwaggerUI();
        //}


        app.UseCors(cpb =>
        {
            cpb.AllowAnyHeader();
            cpb.AllowAnyMethod();
            cpb.AllowAnyOrigin();
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        Directory.CreateDirectory(target.FullName);

        // Copy each file into the new directory.
        foreach (FileInfo fi in source.GetFiles())
        {
            Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }

        // Copy each subdirectory using recursion.
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir =
                target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }
    }
}