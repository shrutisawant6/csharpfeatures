//File - scoped Namespace Declaration
//format
//To format a selection: Ctrl+K, Ctrl+F
//To format a document: Ctrl + K, Ctrl + D
namespace CSharpImprovements;

public class Food
{
    public string Name { get; set; }
    public FoodUsage Usage { get; set; }
}

public class FoodUsage
{
    public string Name { get; set; }

    public FoodIngredient Description { get; set; }
}

public class FoodIngredient
{
    public List<string> Ingredients { get; set; }
}

