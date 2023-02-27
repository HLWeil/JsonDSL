namespace JsonDSL

open System.Text.Json
open Microsoft.FSharp.Quotations
open System.Collections.Generic
open Expression

type Items = JEntity<Nodes.JsonNode list>


type ArrayBuilder() =

    member inline this.Zero() : Items = JEntity.NoneOptional []

    member _.Quote  (quotation: Quotations.Expr<'T>) =
        quotation

    member inline this.Yield(n: RequiredSource<unit>) = 
        n

    member inline this.Yield(n: RequiredSource<Items>) = 
        n

    member inline this.Yield(n: OptionalSource<unit>) = 
        n

    //[<CustomOperation("required")>]
    //member this.Required (fields : Items) =
    //    RequiredSource(fields)
    member this.Yield (value: 'T) : Items = 
        JEntity.ofGeneric value
        |> JEntity.map List.singleton

    member inline this.YieldFrom(vals: seq<'T>) : Items = 
        vals
        |> Seq.map (JEntity.ofGeneric >> JEntity.map List.singleton)
        |> Seq.toList
        |> List.reduce (fun a b -> this.Combine(a,b))

    member inline this.For(vs : seq<'U>, f : 'U -> Items) : Items =
        vs
        |> Seq.map f
        |> Seq.toList
        |> List.reduce (fun a b -> this.Combine(a,b))


    member this.Combine(wx1: RequiredSource<unit>, wx2: Items) =
        RequiredSource (wx2)
        
    member this.Combine(wx1: Items, wx2: RequiredSource<unit>) =
        RequiredSource (wx1)

    member this.Combine(wx1: OptionalSource<unit>, wx2: Items) =
        OptionalSource wx2

    member this.Combine(wx1: Items, wx2: OptionalSource<unit>) =
        OptionalSource wx1

    member this.Combine(wx1: RequiredSource<Items>, wx2: Items) =
        this.Combine(wx1.Source,wx2) 
        |> RequiredSource

    member this.Combine(wx1: Items, wx2: RequiredSource<Items>) =
        this.Combine(wx1,wx2.Source) 
        |> RequiredSource

    member this.Combine(wx1: OptionalSource<Items>, wx2: Items) =
        this.Combine(wx1.Source,wx2) 
        |> OptionalSource

    member this.Combine(wx1: Items, wx2: OptionalSource<Items>) =
        this.Combine(wx1,wx2.Source) 
        |> OptionalSource

    member x.Combine (i1 : Items, i2 : Items) : Items = 
        match i1,i2 with
        // If both contain content, combine the content
        | Some (l1,messages1), Some (l2,messages2) ->
            Some (List.append l1 l2
            ,List.append messages1 messages2)

        // If any of the two is missing and was required, return a missing required
        | _, NoneRequired messages2 ->
            NoneRequired (List.append i1.Messages messages2)

        | NoneRequired messages1, _ ->
            NoneRequired (List.append messages1 i2.Messages)

        // If only one of the two is missing and was optional, take the content of the functioning one
        | Some (f1,messages1), NoneOptional messages2 ->
            Some (f1
            ,List.append messages1 messages2)

        | NoneOptional messages1, Some (f2,messages2) ->
            Some (f2
            ,List.append messages1 messages2)

        // If both are missing and were optional, return a missing optional
        | NoneOptional messages1, NoneOptional messages2 ->
            NoneOptional (List.append messages1 messages2)

    member x.Run(fields : Expr<Items>)  =
        match eval<Items> fields with
        | Some (v, messages) -> 
            v
            |> JsonArray.ofNodes
        | NoneOptional (messages) ->
            Nodes.JsonArray()
        | NoneRequired (messages) ->   
            if messages.IsEmpty then 
                failwith "Could not create Json Assay, as required elements were missing"
            messages
            |> List.map (fun m ->  m.AsString())
            |> List.reduce (fun m1 m2 ->  $"{m1}\n{m2}")
            |> failwithf "Could not create Json Array, as required elements were missing: %s"


    member x.Run(fields : Expr<RequiredSource<Items>>) : JEntity<Nodes.JsonNode> =
        try
            match (eval<RequiredSource<Items>> fields).Source with
            | Some (v, messages) -> 
                v
                |> JsonArray.ofNodes
                |> fun o -> Some(o,messages)
            | NoneOptional messages ->
                NoneRequired messages
            | NoneRequired messages ->   
                NoneRequired messages
        with
        | err -> NoneRequired([message $"Failed to create Json Array: {err.Message}" ])

    member x.Run(fields : Expr<OptionalSource<Items>>) : JEntity<Nodes.JsonNode> =
        try
            match (eval<OptionalSource<Items>> fields).Source with
            | Some (v, messages) -> 
                v
                |> JsonArray.ofNodes
                |> fun o -> Some(o,messages)
            | NoneOptional messages ->
                NoneOptional messages
            | NoneRequired messages ->   
                NoneOptional messages
        with
        | err -> NoneOptional([message $"Failed to create Json Array: {err.Message}" ])

    member inline _.Delay(n: unit -> 'T) = n()
