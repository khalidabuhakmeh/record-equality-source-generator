using RecordEquality;
using static System.Console;

var items = new List<CartItem>
{
    new(1, "The Witcher 3", 60m),
    new(2, "Elden Ring", 60m),
    new(3, "King of Fighters XV", 50m),
    new(4, "Street Fighter V", 30m)
};

WriteLine($"Items in Cart (before): {items.Count}");

// remove record with value equality
// not with reference equality
var item = new CartItem(1, "The Witcher 3");
items.Remove(item);

// before records, you'd have to do this
// items.RemoveAll(i => i.Name == item.Name);

WriteLine($"Items in Cart (after): {items.Count}");

public partial record CartItem(
    [Equality] int Id,
    [Equality] string Name = "",
    decimal Price = 0m);