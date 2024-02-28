namespace CSharpImprovements
{
    //record Point
    //{
    //    public int X { get; init; }
    //    public int Y { get; init; }
    //}

    //class Rectangle
    //{
    //    public Point TopLeft { get; set; }
    //    public Point BottomRight { get; set; }
    //}

    class ClassPerson
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    struct StructPerson
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public record Record1Person(string Name, int Age);

    class RecordWithinClassPerson
    {
        public string Name { get; init; }
        public int Age { get; init; }
    }
}
