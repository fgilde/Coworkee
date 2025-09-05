using lib.Coworkee.Application.Requests;

namespace Coworkee.Application.Common.Models
{
    public class ProductDto : HashableDtoBase
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public BrandDto Brand { get; set; }
        public string ImageDataURL { get; set; }
        public UploadRequest UploadRequest { get; set; }
    }
}