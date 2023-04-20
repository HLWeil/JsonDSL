module TestObjects

open JsonDSL

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
        property "array_value" (array {
            yield true
            for i in 2..8 -> i
            yield "9"
            yield [10]
            yield (object {
                property "11" 11
            })
        })
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
        property "array_value" [
            box "string"
            box 42
            box false
            box [ "inner_array" ]
        ]

        property "object_value" (object {
            property "string_value" "yes"
            property "number_value" 42
            property "boolean_value" true
            property "array_value" [
                box "string"
                box 42
                box false
                box [ "inner_array" ]
            ]
        })

    }