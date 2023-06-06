namespace JsonDSL

open System.Text.Json
open Microsoft.FSharp.Quotations
open System.Collections.Generic
open Expression

type Properties = JEntity<(string*Nodes.JsonNode) list>

type ObjectBuilder() =

    let append (fields : Properties) (name : string) (value : JEntity<Nodes.JsonNode>) =
        match fields, value with
        | Some (fields,messages1),Some(value,messages2) ->
            Some (List.append fields [name,value]
            ,List.append messages1 messages2)

        // If any of the two is missing and was required, return a missing required
        | _, NoneRequired messages2 ->
            NoneRequired (List.append fields.Messages messages2)

        | NoneRequired messages1, _ ->
            NoneRequired (List.append messages1 value.Messages)

        // If only one of the two is missing and was optional, take the content of the functioning one
        | Some (f1,messages1), NoneOptional messages2 ->
            Some (f1
            ,List.append messages1 messages2)

        | NoneOptional messages1, Some (f2,messages2) ->
            Some ([name,f2]
            ,List.append messages1 messages2)

        // If both are missing and were optional, return a missing optional
        | NoneOptional messages1, NoneOptional messages2 ->
            NoneOptional (List.append messages1 messages2)

        
    member x.Yield (()) : Properties = JEntity.NoneOptional []

    member _.Quote  (quotation: Quotations.Expr<'T>) =
        quotation

    /// Yielding Properties
    [<CustomOperation("property")>]
    member x.Property<'T> (fields : Properties, name: string, value: 'T) : Properties =
        JEntity.ofGeneric value
        |> append fields name   

    [<CustomOperation("property")>]
    member x.Property<'T> (fields : RequiredSource<Properties>, name: string, value: 'T) : RequiredSource<Properties> =
        JEntity.ofGeneric value
        |> append fields.Source name 
        |> RequiredSource

    [<CustomOperation("property")>]
    member x.Property<'T> (fields : OptionalSource<Properties>, name: string, value: 'T) : OptionalSource<Properties> =
        JEntity.ofGeneric value
        |> append fields.Source name 
        |> OptionalSource

    /// Select output
    [<CustomOperation("requiredObject")>]
    member x.Required<'T> (fields : Properties) : RequiredSource<Properties> =
        RequiredSource(fields)

    [<CustomOperation("optionalObject")>]
    member x.Optional<'T> (fields : Properties) : OptionalSource<Properties> =
        OptionalSource(fields)

    [<CustomOperation("expression")>]
    member x.Expression<'T> (fields : Properties) : ExpressionSource<Properties> =
        ExpressionSource(fields)

    member x.Run(fields : Expr<Properties>)  =
        match eval<Properties> fields with
        | Some (v, messages) -> 
            v
            |> List.map (fun (name,value) -> KeyValuePair(name,value))
            |> JsonObject.ofProperties
        | NoneOptional (messages) ->
            Nodes.JsonObject()
        | NoneRequired (messages) ->   
            if messages.IsEmpty then 
                failwith "Could not create Json Object, as required elements were missing"
            messages
            |> List.map (fun m ->  m.AsString())
            |> List.reduce (fun m1 m2 ->  $"{m1}\n{m2}")
            |> failwithf "Could not create Json Object, as required elements were missing: %s"
            
    member x.Run(fields : Expr<RequiredSource<Properties>>) : JEntity<Nodes.JsonNode> =
        try
            match (eval<RequiredSource<Properties>> fields).Source with
            | Some (v, messages) -> 
                v
                |> List.map (fun (name,value) -> KeyValuePair(name,value))
                |> JsonObject.ofProperties
                |> fun o -> Some(o,messages)
            | NoneOptional messages ->
                NoneRequired messages
            | NoneRequired messages ->   
                NoneRequired messages
        with
        | err -> NoneRequired([message $"Failed to create Json Object: {err.Message}" ])

    member x.Run(fields : Expr<OptionalSource<Properties>>) : JEntity<Nodes.JsonNode> =
        try
            match (eval<OptionalSource<Properties>> fields).Source with
            | Some (v, messages) -> 
                v
                |> List.map (fun (name,value) -> KeyValuePair(name,value))
                |> JsonObject.ofProperties
                |> fun o -> Some(o,messages)
            | NoneOptional messages ->
                NoneOptional messages
            | NoneRequired messages ->   
                NoneOptional messages
        with
        | err -> NoneOptional([message $"Failed to create Json Object: {err.Message}" ])

    member x.Run(fields : Expr<ExpressionSource<Properties>>) : Expr<ExpressionSource<Properties>> =
        fields

