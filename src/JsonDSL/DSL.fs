namespace JsonDSL

open System.Text.Json
open JsonDSL.Expression

[<AutoOpen>]
module DSL =
    let object = ObjectBuilder()
    let array = ArrayBuilder()

    /// Optional operators for object and array expressions
    let optionaL = OptionalSource()

    /// Required operators for object and array expressions
    let requireD = RequiredSource()

    /// Required value operator
    ///
    /// If expression does fail, returns a missing required value
    let inline (!!) (v : Result<'T,exn>) : JEntity<Nodes.JsonNode> =
        match v with
        | Ok v -> JEntity.ofGeneric v
        | Error err -> NoneRequired([Exception err])

    /// Optional value operator
    ///
    /// If expression does fail, returns a missing optional value
    let inline (!?) (v : Result<'T,exn>) : JEntity<Nodes.JsonNode> =
        match v with
        | Ok v -> JEntity.ofGeneric v
        | Error err -> NoneOptional([Exception err])