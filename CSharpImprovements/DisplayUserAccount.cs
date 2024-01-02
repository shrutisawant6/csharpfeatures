namespace CSharpImprovements
{
    public class DisplayUserAccount
    {
        public string Details { get; set; }

        [JsonIgnore]
        public string AccountNumber { get; set; }

        public DisplayUserAccount()
        {
            Details = "Account: Adam Julius";
            AccountNumber = "XXXXXXX123569";
        }
    }
}
