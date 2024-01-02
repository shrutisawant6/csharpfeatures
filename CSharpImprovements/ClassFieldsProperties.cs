namespace CSharpImprovements
{
    public class ClassFieldsProperties
    {
        //fields
        private int _id;
        private string _name;
        private string Description = "Constant description";

        //properties
        public int Id
        {
            get { return _id; }
        }

    }
}
