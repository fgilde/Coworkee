namespace Coworkee.Application.Common.Models
{
    public class BrandDto: HashableDtoBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Tax { get; set; }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Deconstruction for Brand. Usable with var (name, desc, tax) = instanceOfBrandDto;
        /// </summary>
        public void Deconstruct(out string name, out string description, out decimal tax)
        {
            name = Name;
            description = Description;
            tax = Tax;
        }
    }
}