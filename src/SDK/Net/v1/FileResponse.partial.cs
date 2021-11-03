namespace SDK
{
    public partial class FileResponse
    {
        public bool IsSuccessStatusCode => StatusCode is >= 200 and <= 299;
    }
}