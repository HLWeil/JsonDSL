#r @"..\src\JsonDSL\bin\Release\net7.0\JsonDSL.dll"


open JsonDSL
open System.Text.Json

let options = JsonSerializerOptions(WriteIndented = true)

let jNode =
    object {
        property "hello" 1
        property "my" 1u
        property "friend" "lolzor"
        property "xddd" (object {
            property "sabbrobertie" "works as Indented xddd"
            property "sabArrei" (array {
                1
                2
                3            
            })
            property "sabArre2i" (array {
                object {property "MyProperty" "get off it"}
                object {property "MyProperty" "get off dawdaw3it"}
                object {property "MyProperty" "get off 333it"}            
            })
        }
            
        )
    }

jNode.ToJsonString(options)