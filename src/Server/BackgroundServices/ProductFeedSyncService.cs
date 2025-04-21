using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Features.Brands.Commands.AddEdit;
using Coworkee.Application.Features.Brands.Queries.GetAll;
using Coworkee.Application.Features.Products.Commands.AddEdit;
using Coworkee.Infrastructure.Contexts;
using CsvHelper;
using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nextended.Core.Attributes;

namespace Coworkee.Server.BackgroundServices;

[RegisterAs(typeof(IHostedService), ServiceLifetime = ServiceLifetime.Singleton, Enabled = false)]
public class ProductFeedSyncService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    public readonly IServiceScopeFactory ScopeFactory = scopeFactory;
    private const string FeedUrl = "https://transport.productsup.io/9749eccfe150b21a58b0/channel/377786/pdsfeed.csv";

    public async Task RunImport(BrandDto targetBrand, CancellationToken stoppingToken)
    {
        using var scope = await ScopeFactory.CreateScope().AsSystemUserAsync();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await ReadRemoteFeedAsync(mediator, targetBrand, db, stoppingToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var brand = await EnsureBrand();
        while (!stoppingToken.IsCancellationRequested)
        {
            BackgroundJob.Enqueue(() => RunImport(brand, stoppingToken));
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task<BrandDto> EnsureBrand()
    {
        using var scope = await ScopeFactory.CreateScope().AsSystemUserAsync();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var brands = await mediator.Send(new GetAllBrandsQuery() { Force = true });
        if (brands.Count <= 0)
        {
            var cmd = new AddEditBrandsCommand(new BrandDto()
            {
                Name = "Default",
                Description = "Default brand",
            });
            var res = await mediator.Send(cmd);
            return res.Added.First();
        }

        return brands.First();
    }

    private Task ReadRemoteFeedAsync(IMediator mediator, BrandDto targetBrand, ApplicationDbContext db, CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            try
            {
                int index = 0;
                Uri uri = new Uri(FeedUrl);

                var client = new HttpClient();
                var httpResponseMessage = await client.GetAsync(uri, stoppingToken);
                if ((httpResponseMessage.IsSuccessStatusCode))
                {

                    StreamReader strReader = new StreamReader(await httpResponseMessage.Content.ReadAsStreamAsync(stoppingToken));
                    CsvReader csv = new CsvReader(strReader, CultureInfo.InvariantCulture);
                    var products = new List<ProductDto>();
                    while (await csv.ReadAsync())
                    {
                        if (index > 0) // csv header we don't want
                        {
                            var product = CreateProduct(csv, targetBrand);
                            products.Add(product);
                        }

                        index++;
                    }
                    strReader.Close();

                    try
                    {
                        var cmd = new AddEditProductsCommand(products.ToArray());
                        var result = await mediator.Send(cmd, stoppingToken);
                        var c = result.Added.Length;
                        Console.WriteLine($"Added {c} products");
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
            { }
        }, stoppingToken);
    }


    private ProductDto? CreateProduct(CsvReader csv, BrandDto targetBrand)
    {
        return new ProductDto()
        {
            Name = csv.GetField<string>(0),
            Barcode = csv.GetField<string>(1),
            ImageDataURL = csv.GetField<string>(2),
            Brand = targetBrand,
            Description = csv.GetField<string>(6),
            Rate = 3,
        };
    }

}