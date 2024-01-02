namespace CSharpImprovements
{
    public class LimitedAccessClass
    {
        private string PrivateField = "Calling a private field";

        private string PrivateMethod()
        {
            return "Calling a private method";
        }
    }
}
