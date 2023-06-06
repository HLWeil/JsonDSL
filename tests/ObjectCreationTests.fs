module ObjectCreationTests


open Expecto
open JsonDSL
open System.Text.Json
open TestUtils

// These tests use the objects in TestObjects and compare them to the objects parsed from ReferenceObjects
// a `DeepEquals` method is curtrently not available for System.text.Json, so we have to either wait or write such a function on our own.
// i'll mark these tests as pending for now.
// see also: https://stackoverflow.com/questions/60580743/what-is-equivalent-in-jtoken-deepequals-in-system-text-json

[<Tests>]
let ``yield optional JEntity tests`` =
    testList "yield optional properties via (-.)" [
        testCase "jEntity_noneOptional_value" (fun _ ->
            let result = 
                object {
                    property "myProperty" (-. Option.None)                    
                }
               
            Expect.isTrue (JsonObject.isEmpty result) "Object should have no properties"
        )
        testCase "jEntity_string_value" (fun _ ->
            let result = 
                object {
                    property "myProperty" (-. (Option.Some "5"))                    
                }
               
            let v = result |> JsonObject.tryGetProperty "myProperty"
            Expect.isSome v "did not yield correct property"
            JsonNode.isValue (v.Value) "did not yield correct value"
            Expect.equal (v.Value.AsValue() |> JsonValue.asString) "5" "yielded value was not correct"
        )
        testCase "jEntity_int_value" (fun _ ->
            let result = 
                object {
                    property "myProperty" (-. (Option.Some 5))                    
                }
               
            let v = result |> JsonObject.tryGetProperty "myProperty"
            Expect.isSome v "did not yield correct property"
            JsonNode.isValue (v.Value) "did not yield correct value"
            Expect.equal (v.Value.AsValue() |> JsonValue.asInt) 5 "yielded value was not correct"
        )        
        testCase "jEntity_float_value" (fun _ ->
            let result = 
                object {
                    property "myProperty" (-. (Option.Some 5.))                    
                }
               
            let v = result |> JsonObject.tryGetProperty "myProperty"
            Expect.isSome v "did not yield correct property"
            JsonNode.isValue (v.Value) "did not yield correct value"
            Expect.equal (v.Value.AsValue() |> JsonValue.asFloat) 5. "yielded value was not correct"
        )
        testCase "jEntity_datetime_value" (fun _ ->
            let time = System.DateTime.Parse("10/10/2010")
            let result = 
                object {
                    property "myProperty" (-. (Option.Some time))                    
                }
               
            let v = result |> JsonObject.tryGetProperty "myProperty"
            Expect.isSome v "did not yield correct property"
            JsonNode.isValue (v.Value) "did not yield correct value"
            Expect.equal (v.Value.AsValue() |> JsonValue.asDateTime) time "yielded value was not correct"
        )
        testCase "jEntity_JsonArray_value" (fun _ ->
            let arr = [|1; 2|]
            let result = 
                object {
                    property "myProperty" (-. (Option.Some (array {1; 2})))                    
                }
               
            let v = result |> JsonObject.tryGetProperty "myProperty"
            Expect.isSome v "did not yield correct property"
            JsonNode.isArray (v.Value) "did not yield correct value"
            Expect.equal (v.Value.AsArray() |> JsonArray.castAs<int>) arr "yielded array was not correct"
        )
        testCase "jEntity_JsonObject_value" (fun _ ->

            let result = 
                object {
                    property "myProperty" (-. (Option.Some (object {property "key" "value"})))                    
                }
               
            let v = result |> JsonObject.tryGetProperty "myProperty"
            Expect.isSome v "did not yield correct property"
            JsonNode.isObject (v.Value) "did not yield correct value"
            Expect.isTrue (JsonObject.hasProperty "key" (v.Value.AsObject())) "yielded object was not correct"
        )
    ]

[<Tests>]
let ``yield required JEntity tests`` =
    testList "yield required properties via (+.)" [
        testCase "missing required value fails" (fun _ -> Expect.throws (fun _ -> object {property "myProperty" (+. Option.None)} |> ignore) "object creation with missing required property value did not fail")
    ]

[<Tests>]
let ``optionalObject`` = 
    testList "optionalObject" [
        testCase "optionalObject is ommitted when inner expression fails" (fun _ ->
            let r = object {
                property "willFail" (object {
                    optionalObject
                    property "" (+. Option.None)
                })
            } 
            Expect.isTrue (JsonObject.isEmpty r) "object was not empty"
            Expect.isFalse (r |> JsonObject.hasProperty "willFail") "expected property to be ommitted"
        )
        testCase "optionalObject is present when inner expression succeeds" (fun _ ->
            let r = object {
                property "willWork" (object {
                    optionalObject
                    property "hi" (-. (Option.Some 5.))
                })
            } 
            Expect.isFalse (JsonObject.isEmpty r) "object was not empty"
            Expect.isTrue (r |> JsonObject.hasProperty "willWork") "expected property to be present"
        )
    ]

[<Tests>]
let ``requiredObject`` = 
    testList "requiredObject" [
        testCase "requiredObject fails when inner expression fails" (fun _ ->
            let r() = 
                object {
                    property "willFail" (object {
                        requiredObject
                        property "hi" (+. Option.None)
                    }) 
                } |> ignore
            Expect.throws r "object creation with missing required property value did not fail"
        )
        testCase "requiredObject is present when inner expression succeeds" (fun _ ->
            let r = object {
                property "willWork" (object {
                    requiredObject
                    property "hi" (+. (Option.Some 5.))
                })
            } 
            Expect.isFalse (JsonObject.isEmpty r) "object was not empty"
            Expect.isTrue (r |> JsonObject.hasProperty "willWork") "expected property to be present"
        )
    ]

[<Tests>]
let ``optionalArray`` = 
    testList "optionalArray" [
        testCase "optionalArray is ommitted when inner expression fails" (fun _ ->
            let r = object {
                property "willFail" (array {
                    yield optionalArray
                    yield (+. Option.None)
                })
            } 
            Expect.isTrue (JsonObject.isEmpty r) "object was not empty"
            Expect.isFalse (r |> JsonObject.hasProperty "willFail") "expected property to be ommitted"
        )
        testCase "optionalArray is present when inner expression succeeds" (fun _ ->
            let r = object {
                property "willWork" (array {
                    yield optionalArray
                    yield (-. (Option.Some 5.))
                })
            } 
            Expect.isFalse (JsonObject.isEmpty r) "object was not empty"
            Expect.isTrue (r |> JsonObject.hasProperty "willWork") "expected property to be present"
        )
    ]

[<Tests>]
let ``requiredArray`` = 
    testList "requiredArray" [
        testCase "requiredArray fails when inner expression fails" (fun _ ->
            let r() = 
                object {
                    property "willFail" (array {
                        yield requiredArray
                        yield (+. Option.None)
                    }) 
                } |> ignore
            Expect.throws r "object creation with missing required property value did not fail"
        )
        testCase "requiredArray is present when inner expression succeeds" (fun _ ->
            let r = object {
                property "willWork" (array {
                    yield requiredArray
                    yield (+. (Option.Some 5.))
                })
            } 
            Expect.isFalse (JsonObject.isEmpty r) "object was not empty"
            Expect.isTrue (r |> JsonObject.hasProperty "willWork") "expected property to be present"
        )
    ]

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
