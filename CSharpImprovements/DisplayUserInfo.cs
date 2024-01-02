namespace CSharpImprovements
{
    public class DisplayUserInfo
    {

        public string Details { get; set; }

        [JsonIgnore]
        public string SensitiveInfo { get; set; }

        public DisplayUserInfo()
        {
            Details = "Name: Adam Julius";
            SensitiveInfo = "This key is sensitive";
        }
    }
}
