module ObjectCreationTests


open Expecto
open System.Text.Json
open System.Text.Json.Nodes
// These tests use the objects in TestObjects and compare them to the objects parsed from ReferenceObjects
// a `DeepEquals` method is curtrently not available for System.text.Json, so we have to either wait or write such a function on our own.
// i'll mark these tests as pending for now.
// see also: https://stackoverflow.com/questions/60580743/what-is-equivalent-in-jtoken-deepequals-in-system-text-json

[<PTests>]
let ``json string creation tests`` =
    testList "json object creation" [
        testCase "string_value" (fun _ ->
            Expect.sequenceEqual
                (TestObjects.string_value)
                ReferenceObjects.JsonObjects.string_value
                "json objects were not equal."
        )
        testCase "number_value" (fun _ ->
            Expect.sequenceEqual
                (TestObjects.number_value)
                ReferenceObjects.JsonObjects.number_value
                "json objects were not equal."
        )
        testCase "boolean_value" (fun _ ->
            Expect.sequenceEqual
                (TestObjects.boolean_value)
                ReferenceObjects.JsonObjects.boolean_value
                "json objects were not equal."
        )
        testCase "array_value" (fun _ ->
            Expect.sequenceEqual
                (TestObjects.array_value)
                ReferenceObjects.JsonObjects.array_value
                "json objects were not equal."
        )
        testCase "object_value" (fun _ ->
            Expect.sequenceEqual
                (TestObjects.object_value)
                ReferenceObjects.JsonObjects.object_value
                "json objects were not equal."
        )
        testCase "all_values" (fun _ ->
            Expect.sequenceEqual
                (TestObjects.all_values)
                ReferenceObjects.JsonObjects.all_values
                "json objects were not equal."
        )
    ]