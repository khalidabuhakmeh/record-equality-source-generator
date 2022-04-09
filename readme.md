# Record Equality Source Generator

This is a sample where I use an attribute to modify a records
default equality operator using an `Equality` attribute and the
use of the `partial` keyword.

## Reasoning

Records use value equality, but often domain entities have identifiers
that denote the `identity` of a record, and that's what we want to compare.
Other values on the entity might be different, but logically they may be
the same entity.

## Use Case

The common use case is we want to look at an `Id` property. You'd use
the source generator

```c#
public partial record CartItem(
    [Equality] int Id,
    string Name = "",
    decimal Price = 0m);
```

Now when you compare two `CartItem` instances, they will compare based
on the value of `Id` alone.
You may also add the equality attribute on multiple properties for composite keys.

```c#
public partial record CartItem(
    [Equality] int Id,
    [Equality] string Name = "",
    decimal Price = 0m);
```

## Possible Improvements

If you're interested in using this in your project, some other improvements might include:

- Supporting Comparison types like `Oridinal` or `Ignore Case` on properties that support such comparers i.e. `string`.
- Overriding the `EqualityContract` to support inheritance comparison properly.

## License

Cheers, don't sue me.