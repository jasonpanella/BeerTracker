using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDb.Repository;
using MongoDb.Repository.Interfaces;
using MongoDBLoader.Domain.CSVModel;

namespace MongoDBLoader
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var serviceProvider = AppStartUp();

            await serviceProvider.GetService<IMyApplication>().GetAll();

            // Read input data file
            var inputDataCsvList = await serviceProvider.GetService<IMyApplication>().ReadInputFile();

            //
            // List<Beverage> beverages = new List<Beverage>();
            //inputDataCsvList.CopyTo(beverages);
            //inputDataCsvList.ForEach(x=>x.);
            await serviceProvider.GetService<IMyApplication>().InsertIntoMongoDb(inputDataCsvList);


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
                .AddSingleton<IBeverageRepository, BeverageRepository>()
                .AddSingleton(m =>
                {
                    return new MongoClient(mongoSettingConfiguration.ConnectionString);
                })
                .AddSingleton(m =>
                {
                    return new MongoDBContext(mongoSettingConfiguration.ConnectionString, mongoSettingConfiguration.DatabaseName);
                })
                //.AddSingleton<IMongoDBContext, MongoDBContext>()
                .AddSingleton<Mongosettings>(mongoSettingConfiguration)
                .BuildServiceProvider();
            
            return serviceProvider;
        }
    }
}
