# JsonDSL
Small FSharp based DSL for writing json.


## Simple Object creation

`object` CE is for creating json objects.
`array` CE is for creating json arrays.
`property` operation is for creating properties. Properties can be Json element, primitives, or DateTime.


```fsharp
object{
    property "name" "John"
    property "age" 30
    property "married" true
    property "birthday" (System.DateTime.Now)
    property "children" (array{
        "Jane"
        "Mark"
    })
    property "Job" (object {
        property "title" "Developer"
        property "company" "a company"
    })
}
```

## Required and optional properties

You can mark properties as required by using the `+.` operator. The json creation will fail when the expression creating the property returns `None`:

```fsharp
object {property "i am required" (+. (Option.None))} // System.Exception: Could not create Json Object, as required elements were missing: Value was missing
object {property "i am required" (+. Option.Some 3))} // val it: Nodes.JsonObject = seq [[hi, 3] {Key = "hi"; Value = 3;}]
```

Vice versa, mark properties as optional by using the `-.` operator. The property will be in the resulting json object when the expression creating the property returns `Some value`, and be ommitted when it returns `None`:

```fsharp
object {property "i am optional" (-. (Option.None))} // val it: Nodes.JsonObject = seq []
object {property "i am optional" (-. Option.Some 3))} // val it: Nodes.JsonObject = seq [[hi, 3] {Key = "hi"; Value = 3;}]
```

## Required and optional objects 

You can either use the `+.` and `-.` operators in combination with the object creation CE by wrapping the result in an `Option`:

```fsharp
object {property "innerObject" (-. Option.None))} 
object {property "i am optional" (+. Option.Some (object { property "inner" "yes"})))} 
```

or you can use the `required` and `optional` Custom Operations:

```fsharp
```

## Using required/optional logic for conversion APIs

The main reason we provide the required/optional logic is to facilitate conversion between formats.

As an example, lets define a type `Person`, which has a name and an optional Email address:

```fsharp

type Person = {
    Name: string
    Email: string option
}

```

While in our source format, the email is optional, we want to convert it to a format where the email is required.

We can convert the `Person` type to a json object with the email when it is present, and fail otherwise:

```fsharp
let convertRequired (p:Person) = 
    object {
        property "name" p.Name
        property "email" (+. p.Email)
    }

convert {Name = "Duderich"; Email = Option.None} // System.Exception: Could not create Json Object, as required elements were missing: Value was missing
convert {Name = "Duderich"; Email = Option.Some "yes@yes.yup"} //val it: Nodes.JsonObject =seq[[name, Duderich] {Key = "name";Value = Duderich;}; [email, yes@yes.yup] {Key = "email"; Value = yes@yes.yup;}]
```

or we can convert the `Person` type to a json object without the email when it is not present, and include it when it is present:

```fsharp

let convertOptional (p:Person) = 
    object {
        property "name" p.Name
        property "email" (-. p.Email)
    }

convertOptional {Name = "Duderich"; Email = Option.None} //val it: Nodes.JsonObject = seq [[name, Duderich] {Key = "name"; Value = Duderich;}]
convertOptional {Name = "Duderich"; Email = Option.Some "yes@yes.yup"} //val it: Nodes.JsonObject =seq[[name, Duderich] {Key = "name";Value = Duderich;}; [email, yes@yes.yup] {Key = "email"; Value = yes@yes.yup;}]
```