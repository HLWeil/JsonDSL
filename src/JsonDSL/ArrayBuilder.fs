namespace JsonDSL

open System.Text.Json
open Microsoft.FSharp.Quotations
open System.Collections.Generic
open Expression

type Items = Nodes.JsonNode list


type ArrayBuilder() =

    member inline this.Zero() : Items = []

    member _.Quote  (quotation: Quotations.Expr<'T>) =
        quotation

    member x.Yield (value: 'T) : Items = 
        match box value with
        | :? Nodes.JsonArray as array -> array :> Nodes.JsonNode        |> List.singleton
        | :? Nodes.JsonValue as value -> value :> Nodes.JsonNode        |> List.singleton
        | :? Nodes.JsonObject as object -> object :> Nodes.JsonNode     |> List.singleton
        | v -> JsonValue.create v :> Nodes.JsonNode                     |> List.singleton

    member x.Combine (i1 : Items, i2 : Items) : Items = 
        List.append i1 i2

    member x.Run(fields : Expr<Items>) =
        eval<Items> fields
        |> JsonArray.ofNodes
            
    member inline _.Delay(n: unit -> 'T) = n()
