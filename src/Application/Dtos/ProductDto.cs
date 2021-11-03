using CleanArchitectureBase.Application.Requests;

namespace CleanArchitectureBase.Application.Dtos
{
    public class UpdateProductDto : DtoBase<int>
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public int BrandId { get; set; }
        public string ImageDataURL { get; set; }
        public UploadRequest UploadRequest { get; set; }
    }

    public class ProductDto : DtoBase<int>
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public string Brand { get; set; }
        public int BrandId { get; set; }
        public string ImageDataURL { get; set; }
    }
}