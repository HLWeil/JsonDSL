namespace System.Text.Json

open System.Collections.Generic

[<AutoOpen>]
/// FSharp style syntactic sugar helpers for System.Text.Json
module FSharpExtensions = 

    /// Nodes.JsonNode match case if it is a Nodes.JsonObject
    let (|Object|_|) (input : Nodes.JsonNode) = 
        try input.AsObject() |> Some with 
        | _ -> None

    /// Nodes.JsonNode match case if it is a Nodes.JsonValue
    let (|Value|_|) (input : Nodes.JsonNode) = 
        try input.AsValue() |> Some with 
        | _ -> None

    /// Nodes.JsonNode match case if it is a Nodes.JsonArray
    let (|Array|_|) (input : Nodes.JsonNode) = 
        try input.AsArray() |> Some with 
        | _ -> None

    /// Helper functions for JsonObject
    module JsonObject =
    
        /// Returns true, if the given JsonObject has no properties
        let isEmpty (o : Nodes.JsonObject) = 
            o.Count = 0

        /// Returns true, if the given JsonObject has a property with the given key
        let hasProperty (k : string) (o : Nodes.JsonObject) =
            o.ContainsKey k

        /// Returns a list of the properties of JsonObject as KeyValuePairs
        let getProperties (o : Nodes.JsonObject) =
            o
            |> Seq.toList

        /// Returns the value, if the given JsonObject has a property with the given key, else returns None
        let tryGetProperty (k : string) (o : Nodes.JsonObject) =
            let b,v = o.TryGetPropertyValue(k)
            if b then Some v else None
    
        /// Creates a JsonObject from a list of properties
        let ofProperties (properties : list<KeyValuePair<string,Nodes.JsonNode>>) =
            properties
            |> List.map (fun kv ->
                let v = Nodes.JsonNode.Parse(kv.Value.ToJsonString())
                KeyValuePair(kv.Key,v))
            |> Nodes.JsonObject

        /// Creates a new JsonObject, containing the union of the properties of both given JsonObjects, but with any key appearing at most once
        let mergeDistinct (o1 : Nodes.JsonObject) (o2 : Nodes.JsonObject) =
            getProperties o1 
            |> List.append (getProperties o2)
            |> List.distinctBy (fun kv -> kv.Key)
            |> ofProperties

        /// Creates a new JsonObject, with the mapping operation applied on each property value of the given JsonObject
        let mapPropertyValues (mapping : Nodes.JsonNode -> Nodes.JsonNode) (o : Nodes.JsonObject) =
            getProperties o 
            |> List.map (fun kv -> 
                let v = 
                    mapping kv.Value
                    |> fun v -> v.ToJsonString()
                    |> Nodes.JsonNode.Parse
                KeyValuePair(kv.Key,v))
            |> Nodes.JsonObject

        /// Creates a new JsonObject, with the mapping operation applied on only those property value of the given JsonObject for whichs keys the predicate returned true 
        let mapPropertyValuesIf (predicate : string -> bool) (mapping : Nodes.JsonNode -> Nodes.JsonNode) (o : Nodes.JsonObject) =
            getProperties o 
            |> List.map (fun kv -> 
                let v = 
                    if predicate kv.Key then
                        mapping kv.Value                    
                    else 
                        kv.Value
                    |> fun v -> v.ToJsonString()
                    |> Nodes.JsonNode.Parse
                KeyValuePair(kv.Key,v))
            |> Nodes.JsonObject

    /// Helper functions for JsonValue
    module JsonValue = 
    
        /// Returns true, if the given JsonValue has no properties
        let isEmpty (v : Nodes.JsonValue) = 
            v.ToJsonString() = "\"\""

        /// Returns the value as 'T if possible, else returns None
        let tryAs<'T> (v : Nodes.JsonValue) =
            let b,v = v.TryGetValue<'T>()
            
            if b then Some v else None

        let castAs<'T> (v : Nodes.JsonValue) = v.Deserialize<'T>()

        /// Returns the value as string if possible, else returns None
        let tryAsString (v : Nodes.JsonValue) = tryAs<string> v

        /// Returns the value as int if possible, else returns None
        let tryAsInt (v : Nodes.JsonValue) = tryAs<int> v

        /// Returns the value as float if possible, else returns None
        let tryAsFloat (v : Nodes.JsonValue) = tryAs<float> v

        /// Returns the value as DateTime if possible, else returns None
        let tryAsDateTime (v : Nodes.JsonValue) = tryAs<System.DateTime> v

        /// Returns the value as string
        let asString (v : Nodes.JsonValue) = castAs<string> v

        /// Returns the value as int
        let asInt (v : Nodes.JsonValue) = castAs<int> v

        /// Returns the value as float
        let asFloat (v : Nodes.JsonValue) = castAs<float> v

        /// Returns the value as DateTime
        let asDateTime (v : Nodes.JsonValue) = castAs<System.DateTime> v

        let inline create (v : 'T) =
            Nodes.JsonValue.Create(v)

    /// Helper functions for JsonArray
    module JsonArray = 

        /// Returns true, if the given JsonArray has no values
        let isEmpty (a : Nodes.JsonArray) = 
            a.Count = 0
            
        let tryAs<'T> (a : Nodes.JsonArray) = 
            try Some (a.Deserialize<'T []>()) with _ -> None

        let castAs<'T> (a : Nodes.JsonArray) = a.Deserialize<'T []>()

        /// Returns all the elements of the JsonArray
        let getElements (a : Nodes.JsonArray) =
            a
            |> Seq.toList

        /// Returns a new JsonArray with the mapping applied to all the elements of the given JsonArray
        let map (mapping : Nodes.JsonNode -> Nodes.JsonNode) (a : Nodes.JsonArray) = 
            a
            |> Seq.map mapping
            |> Seq.toArray
            |> Nodes.JsonArray

        /// Creates a JsonObject from a list of properties
        let ofNodes (nodes : Nodes.JsonNode seq)  =
            nodes
            |> Seq.toArray
            |> Nodes.JsonArray

    /// Helper functions for JsonNode
    module JsonNode =

        /// Returns true, if the given JsonNode is any of its default empty values
        let inline isEmpty (n : Nodes.JsonNode) : bool =
            match n with 
            | Object o -> JsonObject.isEmpty o
            | Array a -> JsonArray.isEmpty a
            | Value v ->  JsonValue.isEmpty v
            | _ -> false

        /// Parses a string to a JsonNode
        let ofString (s : string) : Nodes.JsonNode =
            Nodes.JsonObject.Parse(s)

        /// Parses a JsonNode to a string
        let toString (n : #Nodes.JsonNode) =
            n.ToJsonString()

        let inline ofGeneric<'T> (v : 'T) =

            match box v with
            | :? Nodes.JsonArray as array -> array :> Nodes.JsonNode
            | :? Nodes.JsonValue as value -> value :> Nodes.JsonNode
            | :? Nodes.JsonObject as object -> object :> Nodes.JsonNode
            | v -> JsonValue.create v :> Nodes.JsonNode

    [<AutoOpen>]
    module JsonNodeExtensions =
        /// Cast any child object of JsonNode to a JsonNode
        let inline node (n : #Nodes.JsonNode) : Nodes.JsonNode =
            n :> Nodes.JsonNode

