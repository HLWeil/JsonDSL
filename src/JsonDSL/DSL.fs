namespace JsonDSL

[<AutoOpen>]
module DSL =
    let object = ObjectBuilder()
    let array = ArrayBuilder()