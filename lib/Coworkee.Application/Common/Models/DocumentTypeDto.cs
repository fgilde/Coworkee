namespace lib.Coworkee.Application.Common.Models
{
    public class DocumentTypeDto: DtoBase<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}