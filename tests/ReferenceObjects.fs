module ReferenceObjects

open TestUtils

open System.Text.Json.Nodes

module JsonStrings = 

    let string_value = getEmbeddedJsonString "test_files.string_value.json"

    let number_value = getEmbeddedJsonString "test_files.number_value.json"

    let boolean_value = getEmbeddedJsonString "test_files.boolean_value.json"

    let array_value = getEmbeddedJsonString "test_files.array_value.json"

    let object_value = getEmbeddedJsonString "test_files.object_value.json"

    let all_values = getEmbeddedJsonString "test_files.all_values.json"

module JsonObjects =
    
    let string_value = JsonObject.Parse(JsonStrings.string_value).AsObject()

    let number_value = JsonObject.Parse(JsonStrings.number_value).AsObject()

    let boolean_value = JsonObject.Parse(JsonStrings.boolean_value).AsObject()

    let array_value = JsonObject.Parse(JsonStrings.array_value).AsObject()

    let object_value = JsonObject.Parse(JsonStrings.object_value).AsObject()

    let all_values = JsonObject.Parse(JsonStrings.all_values).AsObject()
