namespace CSharpImprovements
{
    public class CSharpFeature
    {
        public string Name { get; set; }

        public CSharpFeatureUsage Usage { get; set; }
    }

    public class CSharpFeatureUsage
    {
        public string Use { get; set; }

        public string AdditionalInfo { get; set; }
    }
 }
