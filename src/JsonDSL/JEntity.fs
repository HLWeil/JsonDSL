namespace JsonDSL

open System.Text.Json

[<AutoOpen>]
type Message = 
    | Text of string
    | Exception of exn

    static member message (s : string) = Text s

    static member message (e : #exn) = Exception e

    member this.MapText(m : string -> string) =
        match this with
        | Text s -> Text (m s)
        | Exception e -> this

    member this.AsException() = 
        match this with
        | Text s -> exn(s)
        | Exception e -> e

    member this.AsString() = 
        match this with
        | Text s -> s
        | Exception e -> e.Message

    member this.TryText() = 
        match this with
        | Text s -> Some s
        | _ -> None

    member this.TryException() = 
        match this with
        | Exception e -> Some e
        | _ -> None

    member this.IsTxt = 
        match this with
        | Text s -> true
        | _ -> false

    member this.IsExc = 
        match this with
        | Text s -> true
        | _ -> false


module Messages =
    
    let format (ms : Message list) =
        ms
        |> List.map (fun m -> m.AsString())
        |> List.reduce (fun a b -> a + ";" + b)

    let fail (ms : Message list) =
        let s = format ms
        if ms |> List.exists (fun m -> m.IsExc) then
            printfn "s"
            raise (ms |> List.pick (fun m -> m.TryException()))
        else
            failwith s



type JEntity<'T> =

    | Some of 'T * Message list
    | NoneOptional of Message list
    | NoneRequired of Message list

    static member map (f : 'T -> 'U) (entity : JEntity<'T>) =
        match entity with 
        | Some (v,messages)         -> Some (f v,messages)
        | NoneOptional (messages)   -> NoneOptional (messages)
        | NoneRequired (messages)   -> NoneRequired (messages)

    /// Get messages
    member this.Messages =

        match this with 
        | Some (f,errs) -> errs
        | NoneOptional errs -> errs
        | NoneRequired errs -> errs

    static member inline ofGeneric (v : 'T) =
        match box v with
        | :? JEntity<Nodes.JsonArray>   as entity -> JEntity.map node entity    
        | :? JEntity<Nodes.JsonValue>   as entity -> JEntity.map node entity       
        | :? JEntity<Nodes.JsonObject>  as entity -> JEntity.map node entity    
        | :? JEntity<Nodes.JsonNode>    as entity -> JEntity.map node entity    
        | :? JEntity<string>            as entity -> JEntity.map (JsonNode.ofGeneric<string>) entity
        | :? JEntity<int>               as entity -> JEntity.map (JsonNode.ofGeneric<int>) entity    
        | :? JEntity<float>             as entity -> JEntity.map (JsonNode.ofGeneric<float>) entity    
        | :? JEntity<char>              as entity -> JEntity.map (JsonNode.ofGeneric<char>) entity    
        | :? JEntity<single>            as entity -> JEntity.map (JsonNode.ofGeneric<single>) entity    
        | :? JEntity<byte>              as entity -> JEntity.map (JsonNode.ofGeneric<byte>) entity  
        | :? JEntity<System.DateTime>   as entity -> JEntity.map (JsonNode.ofGeneric<System.DateTime>) entity  
        | v -> JEntity.Some (JsonNode.ofGeneric v,[])

[<AutoOpen>]
module JEntityExtensions =


    type JEntity<'T> with

        static member ok (v : 'T) : JEntity<'T> = JEntity.Some (v,[])

        member this.Value =
            match this with 
            | Some (f,errs) -> f
            | NoneOptional ms | NoneRequired ms when ms = [] -> 
                failwith $"SheetEntity of type {typeof<'T>.Name} does not contain Value."
            | NoneOptional ms | NoneRequired ms -> 
                let appendedMessages = Messages.format ms
                failwith $"SheetEntity of type {typeof<'T>.Name} does not contain Value: \n\t{appendedMessages}"