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

    member _.YieldHelper (value: 'T) =
        match box value with
        | :? Nodes.JsonArray as array -> array :> Nodes.JsonNode        
        | :? Nodes.JsonValue as value -> value :> Nodes.JsonNode        
        | :? Nodes.JsonObject as object -> object :> Nodes.JsonNode     
        | v -> JsonValue.create v :> Nodes.JsonNode                     

    member this.Yield (value: 'T) : Items = 
        this.YieldHelper(value) |> List.singleton

    member inline this.YieldFrom(vals: seq<'T>) : Items = 
        vals
        |> Seq.map this.YieldHelper
        |> Seq.toList

    member inline this.For(vs : seq<'T>, f : 'T -> 'U) : Items =
        vs
        |> Seq.map f
        |> this.YieldFrom

    member x.Combine (i1 : Items, i2 : Items) : Items = 
        List.append i1 i2




    member x.Run(fields : Expr<Items>) =
        eval<Items> fields
        |> JsonArray.ofNodes
            
    member inline _.Delay(n: unit -> 'T) = n()
