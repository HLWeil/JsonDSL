namespace JsonDSL

open System.Text.Json
open Microsoft.FSharp.Quotations
open System.Collections.Generic
open Expression

type Properties = (string*Nodes.JsonNode) list


type ObjectBuilder() =
    member x.Yield (()) : Properties = []

    member _.Quote  (quotation: Quotations.Expr<'T>) =
        quotation

    [<CustomOperation("property")>]
    member x.Property (fields : Properties, name: string, value: 'T) =
        match box value with
        | :? Nodes.JsonArray as array -> (name,array :> Nodes.JsonNode) :: fields 
        | :? Nodes.JsonValue as value -> (name,value :> Nodes.JsonNode) :: fields 
        | :? Nodes.JsonObject as object -> (name,object :> Nodes.JsonNode) :: fields 
        | v -> (name,JsonValue.create v :> Nodes.JsonNode) :: fields 


    member x.Run(fields : Expr<Properties>) =
        eval<Properties> fields
        |> List.rev
        |> List.map (fun (name,value) -> KeyValuePair(name,value))
        |> JsonObject.ofProperties
            
