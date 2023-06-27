using GptInvoke.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Domain.Entities.Catalog;
using Coworkee.Application.Features.Products.Commands.AddEdit;
using Coworkee.Application.Common.Models;
using Nextended.Core.Extensions;
using Coworkee.Application.Features.Brands.Commands.AddEdit;


namespace Coworkee.Application.AssistantFeatures;

public class AddProduct: IGptInvokableService
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork<int> _unitOfWork;

    public AddProduct(IMediator mediator, IUnitOfWork<int> unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }
    
    public string Name { get; set; } = "Add Product service";
    public string Description { get; set; } = "I can add products";
    public GptInvokableServiceParameter[] Parameters => new[]
    {
        new GptInvokableServiceParameter("Name", "Name of the product to add", typeof(string), true),
        new GptInvokableServiceParameter("Barcode", "Barcode of product", typeof(string), true),
        new GptInvokableServiceParameter("Description", "Description of product", typeof(string), false),
        new GptInvokableServiceParameter("Brand", "Brand for this product", typeof(string), true),
        new GptInvokableServiceParameter("Rate", "Description of product", typeof(decimal), true),
    };

    public async Task<bool> ExecuteAsync(IDictionary<string, object> parameters)
    {
        
        var name = parameters["Name"].ToString();
        
        var barcode = parameters["Barcode"].ToString();
        var description = (parameters.ContainsKey("Description") ? parameters["Description"]?.ToString() : null)  ?? name;
        if (string.IsNullOrEmpty(description))
            description = name;
        var brandName = parameters["Brand"].ToString();
        var rate = parameters["Rate"].ToString();
       
        var brand = _unitOfWork.Repository<Brand>().Entities.FirstOrDefault(b => b.Name == brandName);
        BrandDto brandDto = brand?.MapTo<BrandDto>();
        if (brand == null)
        {
            brandDto = new BrandDto {Name = brandName, Tax = 2, Description = brandName};
            var x = await _mediator.Send(new AddEditBrandsCommand(brandDto));
            brandDto.Id = x.Added.First().Id;
        }


        AddEditProductsCommand command = new()
        {
            Items = new[] { new ProductDto { Name = name, Barcode = barcode, Description = description, Rate = decimal.Parse(rate), Brand = brandDto } }
        };
        await _mediator.Send(command);
        
        return true;
    }
}