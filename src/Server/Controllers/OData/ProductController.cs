//using System;
//using System.Linq;
//using Coworkee.Application.Contracts.Repositories;
//using Coworkee.Domain.Entities.Catalog;
//using Nextended.Web.Controller;

//namespace Coworkee.Server.Controllers.OData;

//public class ProductController: GenericODataController<Product>
//{
//    private readonly IRepositoryAsync<Product, int> _repository;

//    public ProductController(IRepositoryAsync<Product, int> repository)
//    {
//        _repository = repository;
//    }

//    protected override IQueryable<Product> Queryable()
//    {
//        return _repository.Entities;
//    }
//}