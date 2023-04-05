using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace StreamApi
{
    public class Program
    {

        public static void Main(string[] args) =>
               new WebHostBuilder()
            //#if DEBUG
            //            .UseKestrel(config =>
            //            {
            //                config.ConfigureHttpsDefaults(config =>
            //                {
            //                    config.ServerCertificate = new X509Certificate2(@"D:\certificate.pfx", "cosminR32", X509KeyStorageFlags.DefaultKeySet);
            //                    config.SslProtocols = SslProtocols.Tls12;
            //                });
            //            })
            //            .UseContentRoot(Directory.GetCurrentDirectory())
            //            .UseConfiguration(new ConfigurationBuilder()
            //                        .SetBasePath(Directory.GetCurrentDirectory())
            //                        .AddJsonFile("appsettings.json")
            //                        .Build())
            //            .UseIISIntegration()
            //            .UseStartup<Startup>()
            //            .UseUrls("https://0.0.0.0:5000/")
            //#else
            .UseKestrel(config =>
            {
                config.ConfigureHttpsDefaults(config =>
                {
                    //    config.ServerCertificate = new X509Certificate2(ApplicationEnvironment.ApplicationBasePath + @"Resources/certificate.pfx", "cosminR32", X509KeyStorageFlags.DefaultKeySet);
                    //    config.SslProtocols = SslProtocols.Tls12;
                });
            })
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseConfiguration(new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build())
            .UseIISIntegration()
            .UseStartup<Startup>()
            //.UseUrls("https://0.0.0.0:80")
            //#endif
            .Build()
            .Run();
    }
}
