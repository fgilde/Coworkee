namespace CleanArchitectureBase.Application.Dtos
{
    public class BrandDto: DtoBase<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Tax { get; set; }
    }
}