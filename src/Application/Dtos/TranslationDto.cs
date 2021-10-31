namespace CleanArchitectureBase.Application.Dtos
{
    public class TranslationDto: DtoBase<int>
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string CultureCode { get; set; }
    }
}