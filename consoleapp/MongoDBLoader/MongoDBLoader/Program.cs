using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBLoader
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var serviceProvider = AppStartUp();

            // Get mongo db loader service
            await serviceProvider.GetService<IMyApplication>().GetDetails();
        }

        static Mongosettings getConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var section = configuration.GetSection(nameof(Mongosettings));

            return section.Get<Mongosettings>();

        }

        static ServiceProvider AppStartUp()
        {
            // App configurations
            Mongosettings mongoSettingConfiguration = getConfiguration();
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IMyApplication, MyApplication>()
                .AddSingleton(m =>
                {
                    return new MongoClient(mongoSettingConfiguration.ConnectionString);
                })
                //.AddSingleton<IMongoDBContext, MongoDBContext>()
                .AddSingleton<Mongosettings>(mongoSettingConfiguration)
                .BuildServiceProvider();
            
            return serviceProvider;
        }
    }
}
