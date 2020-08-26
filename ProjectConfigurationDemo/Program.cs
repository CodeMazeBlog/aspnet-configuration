using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ProjectConfigurationDemo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				})
				.ConfigureAppConfiguration((context, builder) =>
				{
					if (context.HostingEnvironment.IsProduction())
					{
						var builtConfig = builder.Build();

						using (var store = new X509Store(StoreLocation.CurrentUser))
						{
							store.Open(OpenFlags.ReadOnly);
							var certs = store.Certificates
								.Find(X509FindType.FindByThumbprint,
									builtConfig["Azure:CertificateThumb"], false);

							builder.AddAzureKeyVault(
								builtConfig["Azure:KeyVault:DNS"],
								builtConfig["Azure:ApplicationId"],
								certs.OfType<X509Certificate2>().Single());

							store.Close();
						}
					}
				});
	}
}
