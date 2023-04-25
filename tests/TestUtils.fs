module TestUtils

open System.Text.Json
open System.Reflection
open System.IO
open JsonDSL
open Expecto

let getEmbeddedJsonString (filename:string) =
    let assembly = Assembly.GetExecutingAssembly()
    use str = assembly.GetManifestResourceStream($"JsonDSLTests.{filename}")
    use r = new StreamReader(str)
    r.ReadToEnd()

module JEntity =
    let inline isSome (x : JEntity<'T>) message = 
        match x with
        | Some _ -> ()
        | NoneOptional _  ->
            failtestf "%s. Expected Some _, was NoneOptional." message
        | NoneRequired _  ->
            failtestf "%s. Expected Some _, was NonRequired." message

    let inline isNoneOptional (x : JEntity<'T>) message = 
        match x with
        | Some _ -> failtestf "%s. Expected Some _, was Some." message
        | NoneOptional _  ->
            ()          
        | NoneRequired _  ->
            failtestf "%s. Expected Some _, was NonRequired." message

    let inline isNoneRequired (x : JEntity<'T>) message = 
        match x with
        | Some _ -> failtestf "%s. Expected Some _, was Some." message
        | NoneOptional _  ->
            failtestf "%s. Expected Some _, was NoneOptional." message
        | NoneRequired _  ->
            ()


module JsonNode =

    let inline isValue (x : Nodes.JsonNode) message = 
        match x with
        | Value _ -> ()
        | Array _  ->
            failtestf "%s. Expected Json Value _, was Json Array." message
        | Object _  ->
            failtestf "%s. Expected Json Value _, was Json Object." message
        | _ -> failtestf "%s. Expected Json Value _, was Unknown Object." message  

    let inline isObject (x : Nodes.JsonNode) message = 
        match x with
        | Value _ -> 
            failtestf "%s. Expected Json Object _, was Json Value." message
        | Array _  ->
            failtestf "%s. Expected Json Object _, was Json Array." message
        | Object _  ->
            ()
        | _ -> failtestf "%s. Expected Json Object _, was Unknown Object." message  

    let inline isArray (x : Nodes.JsonNode) message = 
        match x with
        | Value _ -> 
            failtestf "%s. Expected Json Array _, was Json Value." message
        | Array _  ->
            ()
        | Object _  ->
            failtestf "%s. Expected Json Array _, was Json Object." message
        | _ -> failtestf "%s. Expected Json Array _, was Unknown Object." message   