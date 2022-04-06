using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            // configure app
            var serviceProvider = AppStartUp();

            List<InputDataCsv> inputDataCsvList = new List<InputDataCsv>();

            // retrieve all documents from collection
            var allBeverages = await serviceProvider.GetService<IMyApplication>().GetAll();
            
            //grab the first document
            var beverage = allBeverages.FirstOrDefault();

            if (beverage == null)
            {
                Console.WriteLine("No Documents found in Mongo DB collection. Let's load them....");
                // Read input data file
                inputDataCsvList = await serviceProvider.GetService<IMyApplication>().ReadInputFile();

                List<Beverage> beverageList = new List<Beverage>();

                inputDataCsvList.ForEach(x =>
                {
                    beverageList.Add(new Beverage { Description = x.Description, ABV = x.ABV, Category = x.Category, BeverageName = x.BeverageName });
                });
                
                await serviceProvider.GetService<IMyApplication>().InsertIntoMongoDb(beverageList);

                Console.WriteLine(await serviceProvider.GetService<IMyApplication>().GetDocumentCollectionCount(Builders<Beverage>.Filter.Empty));
            }

            var firstBeverageFromDb = await serviceProvider.GetService<IMyApplication>().GetDetails(beverage.Id.ToString());
            //OR
            //var firstBeverageFromDb =  await serviceProvider.GetService<IMyApplication>().FindOneDocumentAsync(beverage.Id.ToString());

            // let's make an update; maybe change name
            Console.WriteLine($"Going to update document with new name. Previous name is {firstBeverageFromDb.BeverageName}");
            firstBeverageFromDb.BeverageName = "Updated Beverage";
            await serviceProvider.GetService<IMyApplication>().UpdateDocument(firstBeverageFromDb);

            // Did we really update, let's check
            var updatedBeverageFromDb = await serviceProvider.GetService<IMyApplication>().FindOneDocumentAsync(beverage.Id.ToString());
            Console.WriteLine($"Document Updated. New document name is {updatedBeverageFromDb.BeverageName}");
            
            //Now let's delete all from Mongo
            var documentCount = await serviceProvider.GetService<IMyApplication>().GetDocumentCollectionCount(Builders<Beverage>.Filter.Empty);
            Console.WriteLine($"Getting ready to Delete {documentCount} documents");
            await serviceProvider.GetService<IMyApplication>().DeleteAllDocuments(Builders<Beverage>.Filter.Empty);
            Console.WriteLine($"{await serviceProvider.GetService<IMyApplication>().GetDocumentCollectionCount(Builders<Beverage>.Filter.Empty)} Documents exist.");

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
