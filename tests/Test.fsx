#r @"..\src\JsonDSL\bin\Debug\netstandard2.0\JsonDSL.dll"
open JsonDSL
open System.Text.Json

let options = JsonSerializerOptions(WriteIndented = true)

let string_value =
    object {
        property "string_value" "yes"
    }

let number_value =
    object {
        property "number_value" 42
    }

let boolean_value =
    object {
        property "boolean_value" true
    }

let array_value = 
    object {
        property "array_value" [1..8]
    }

let object_value =
    object {
        property "object_value" (object {
            property "string_value" "yes"
            property "number_value" 42
            property "boolean_value" true
            property "array_value" [1..8]
        })
    }

let all_values =
    object {
        property "string_value" "yes"
        property "number_value" 42
        property "boolean_value" true
        property "array_value" [1..8]

        property "object_value" (object {
            property "string_value" "yes"
            property "number_value" 42
            property "boolean_value" true
            property "array_value" [1..8]
        })

    }

object_value.ToJsonString(options)