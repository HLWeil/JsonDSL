module StringCreation

open System.Text.Json
open Expecto

// These tests use the objects in TestObjects and create an idented string via object.ToJsonString(), comparing them with the string from the ReferenceObjects module

let options = JsonSerializerOptions(WriteIndented = true)

[<Tests>]
let ``json string creation tests`` =
    testList "json string creation" [
        testCase "string_value" (fun _ ->
            Expect.equal
                (TestObjects.string_value.ToJsonString(options))
                ReferenceObjects.JsonStrings.string_value
                "json strings were not equal."
        )
        testCase "number_value" (fun _ ->
            Expect.equal
                (TestObjects.number_value.ToJsonString(options))
                ReferenceObjects.JsonStrings.number_value
                "json strings were not equal."
        )
        testCase "boolean_value" (fun _ ->
            Expect.equal
                (TestObjects.boolean_value.ToJsonString(options))
                ReferenceObjects.JsonStrings.boolean_value
                "json strings were not equal."
        )
        testCase "array_value" (fun _ ->
            Expect.equal
                (TestObjects.array_value.ToJsonString(options))
                ReferenceObjects.JsonStrings.array_value
                "json strings were not equal."
        )
        testCase "object_value" (fun _ ->
            Expect.equal
                (TestObjects.object_value.ToJsonString(options))
                ReferenceObjects.JsonStrings.object_value
                "json strings were not equal."
        )
        testCase "all_values" (fun _ ->
            Expect.equal
                (TestObjects.all_values.ToJsonString(options))
                ReferenceObjects.JsonStrings.all_values
                "json strings were not equal."
        )
    ]
