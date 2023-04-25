module Operators


open System.Text.Json
open Expecto
open JsonDSL
open TestUtils
// These tests use the objects in TestObjects and create an idented string via object.ToJsonString(), comparing them with the string from the ReferenceObjects module

[<Tests>]
let ``optional operator tests`` =
    testList "optional operator" [
        testCase "int_value" (fun _ ->
            let result = -. 5
            JEntity.isSome result "Int was not parsed correctly by optional operator"
            JsonNode.isValue result.Value "Int was not parsed correctly by optional operator"
            Expect.equal (JsonValue.tryAsInt (result.Value.AsValue())) (Option.Some 5) "Int was not parsed correctly by optional operator"
        )
        testCase "string_value" (fun _ ->
            let result = -. "5"
            JEntity.isSome result "String was not parsed correctly by optional operator"
            JsonNode.isValue result.Value "String was not parsed correctly by optional operator"
            Expect.equal (JsonValue.asString (result.Value.AsValue())) "5" "String was not parsed correctly by optional operator"
        )
        testCase "string_option" (fun _ ->
            let result = -. (Option.Some "5")
            JEntity.isSome result "String option was not parsed correctly by optional operator"
            JsonNode.isValue result.Value "String option was not parsed correctly by optional operator"
            Expect.equal (JsonValue.asString (result.Value.AsValue())) "5" "String option was not parsed correctly by optional operator"
        )
        testCase "option_none" (fun _ ->
            let v : option<string> = Option.None
            let result = -. v
            JEntity.isNoneOptional result "String option was not parsed correctly by optional operator"
        )
    ]

[<Tests>]
let ``required operator tests`` =
    testList "required operator" [
        testCase "int_value" (fun _ ->
            let result = +. 5
            JEntity.isSome result "Int was not parsed correctly by required operator"
            JsonNode.isValue result.Value "Int was not parsed correctly by required operator"
            Expect.equal (JsonValue.tryAsInt (result.Value.AsValue())) (Option.Some 5) "Int was not parsed correctly by required operator"
        )
        testCase "string_value" (fun _ ->
            let result = +. "5"
            JEntity.isSome result "String was not parsed correctly by required operator"
            JsonNode.isValue result.Value "String was not parsed correctly by required operator"
            Expect.equal (JsonValue.asString (result.Value.AsValue())) "5" "String was not parsed correctly by required operator"
        )
        testCase "string_option" (fun _ ->
            let result = +. (Option.Some "5")
            JEntity.isSome result "String option was not parsed correctly by required operator"
            JsonNode.isValue result.Value "String option was not parsed correctly by required operator"
            Expect.equal (JsonValue.asString (result.Value.AsValue())) "5" "String option was not parsed correctly by required operator"
        )
        testCase "option_none" (fun _ ->
            let v : option<string> = Option.None
            let result = +. v
            JEntity.isNoneRequired result "String option was not parsed correctly by required operator"
        )
    ]
    