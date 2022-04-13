using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Attributes;
using CleanArchitectureBase.Application.Features.Products.Commands.AddEdit;
using CleanArchitectureBase.Domain.Entities.Catalog;
using CleanArchitectureBase.Infrastructure.Contexts;
using CsvHelper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Server.BackgroundServices;

//[RegisterAs(typeof(IHostedService), ServiceLifetime = ServiceLifetime.Singleton)]
public class ProductFeedSyncService : BackgroundService
{
    public readonly IServiceScopeFactory ScopeFactory;
    // private const string FeedUrl = "https://transport.productsup.io/9749eccfe150b21a58b0/channel/378317/pdsfeed.csv";
    private const string FeedUrl = "https://transport.productsup.io/9749eccfe150b21a58b0/channel/377786/pdsfeed.csv";


    public ProductFeedSyncService(IServiceScopeFactory scopeFactory)
    {
        ScopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = ScopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator> ();
                await ReadRemoteFeedAsync(mediator, db, stoppingToken);
            }
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private Task ReadRemoteFeedAsync(IMediator mediator, ApplicationDbContext db, CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            try
            {
                return;
                int index = 0;
                Uri uri = new Uri(FeedUrl);

                var client = new HttpClient();
                var httpResponseMessage = await client.GetAsync(uri, stoppingToken);
                if ((httpResponseMessage.IsSuccessStatusCode))
                {

                    StreamReader strReader = new StreamReader(await httpResponseMessage.Content.ReadAsStreamAsync(stoppingToken));
                    CsvReader csv = new CsvReader(strReader, CultureInfo.InvariantCulture);
                    var products = new List<Product>();
                    while (await csv.ReadAsync())
                    {
                        if (index > 0) // csv header we don't want
                        {
                            var product = CreateProduct(csv);
                            products.Add(product);
                           // AddOrUpdateProduct(product, db);
                        }

                        index++;
                    }
                    strReader.Close();

                    try
                    {
                        var cmd = new AddEditProductsCommand(products.MapElementsTo<ProductDto>().ToArray());
                        var result = await mediator.Send(cmd, stoppingToken);
                        var c = result.Added.Length;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    //await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (System.Net.WebException)
            {}
        }, stoppingToken);
    }

    private void AddOrUpdateProduct(Product? product, ApplicationDbContext db)
    {
        return;
        if (product == null)
            return;
        var productInDb = db.Products.FirstOrDefault(x => x.Id == product.Id);
        if (productInDb != null)
        {
            if (productInDb.NeedsUpdate(product))
            {
                // Update
                product.LastModifiedOn = DateTime.UtcNow;
                db.Entry(productInDb).CurrentValues.SetValues(product);
            }
        }
        else
        {
            db.Products.Add(product);
        }
    }


    private Product? CreateProduct(CsvReader csv)
    {
        return new Product()
        {
            Name = csv.GetField<string>(0),
            Barcode = csv.GetField<string>(1),
            ImageDataURL = csv.GetField<string>(2),
            BrandId = 1,
            //TargetUrl = csv.GetField<string>(4),
            //Thumbnail = csv.GetField<string>(5),
            //Category = csv.GetField<string>(6),
            Description = csv.GetField<string>(6),
            Rate = 3,
            //SalePercentage = csv.GetField<string>(8),
            //Price = decimal.ToDouble(csv.GetField<string>(9).ParsePrice()),
            //SalePrice = decimal.ToDouble(csv.GetField<string>(10).ParsePrice()),
            //Price = csv.GetField<string>(9),
            //SalePrice = csv.GetField<string>(10),
            //Shop = csv.GetField<string>(11),
            //Subcategory = csv.GetField<string>(12),
            //DateUpdated = DateTime.UtcNow.Ticks,
            //DateCreated = DateTime.UtcNow.Ticks
        };
    }

}