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

The MIT License (MIT)
Copyright © 2022 Khalid Abuhakmeh

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.