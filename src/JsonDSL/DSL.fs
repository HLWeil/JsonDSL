namespace JsonDSL

open System.Text.Json
open JsonDSL.Expression
open Microsoft.FSharp.Quotations

[<AutoOpen>]
module DSL =
    let object = ObjectBuilder()
    let array = ArrayBuilder()

    /// Optional operators for object and array expressions
    let optionaL = OptionalSource()

    /// Required operators for object and array expressions
    let requireD = RequiredSource()

    let inline parseExpression (def : exn -> JEntity<Nodes.JsonNode>) (s : Expr<'a>) : JEntity<Nodes.JsonNode> =
        try 
            eval<'a> s |> JEntity.ofGeneric     
        with
        | err -> def err

    let inline parseOption (def : exn -> JEntity<Nodes.JsonNode>) (s : Option<'a>) : JEntity<Nodes.JsonNode> =
        match s with
        | Option.Some value ->
            JEntity.ofGeneric value
        | None -> def (exn "Value was missing")
    
    let inline parseResult (def : exn -> JEntity<Nodes.JsonNode>) (s : Result<'a,exn>) : JEntity<Nodes.JsonNode> =
        match s with
        | Result.Ok value ->
            JEntity.ofGeneric value
        | Result.Error exn -> def exn

    let inline parseAny (f : exn -> JEntity<Nodes.JsonNode>) (v: 'T) : JEntity<Nodes.JsonNode> =
        match box v with
        | :? Expr<string> as e ->           parseExpression f e
        | :? Expr<int> as e ->              parseExpression f e
        | :? Expr<float> as e ->            parseExpression f e
        | :? Expr<single> as e ->           parseExpression f e
        | :? Expr<byte> as e ->             parseExpression f e
        | :? Expr<System.DateTime> as e ->  parseExpression f e

        | :? Option<string> as o ->             parseOption f o
        | :? Option<int> as o ->                parseOption f o
        | :? Option<float> as o ->              parseOption f o
        | :? Option<single> as o ->             parseOption f o
        | :? Option<byte> as o ->               parseOption f o
        | :? Option<System.DateTime> as o ->    parseOption f o

        | :? Result<string,exn> as r -> parseResult f r
        | :? Result<int,exn> as r -> parseResult f r
        | :? Result<float,exn> as r -> parseResult f r
        | :? Result<single,exn> as r -> parseResult f r
        | :? Result<byte,exn> as r -> parseResult f r
        | :? Result<System.DateTime,exn> as r -> parseResult f r

        | v -> failwith $"Could not parse value {v}. Only string,int,float,single,byte,System.DateTime allowed."

    /// Required value operator
    ///
    /// If expression does fail, returns a missing required value
    let inline (+.) (f : 'T -> 'U) (v : 'T) : JEntity<Nodes.JsonNode> =
        try JEntity.ofGeneric(f v) with
        | err -> NoneRequired([Exception err])


    /// Optional value operator
    ///
    /// If expression does fail, returns a missing optional value
    let inline (-.) (f : 'T -> 'U) (v : 'T) : JEntity<Nodes.JsonNode> =
        try JEntity.ofGeneric(f v) with
        | err -> NoneOptional([Exception err])

    /// Required value operator
    ///
    /// If expression does fail, returns a missing required value
    let inline (~+.) (v : 'T) : JEntity<Nodes.JsonNode> =
        let f = fun s -> NoneRequired([Exception s])
        parseAny f v 


    /// Optional value operator
    ///
    /// If expression does fail, returns a missing optional value
    let inline (~-.) (v : 'T) : JEntity<Nodes.JsonNode> =
        let f = fun s -> NoneOptional([Exception s])
        parseAny f v 