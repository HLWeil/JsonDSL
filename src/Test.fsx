#r @"..\src\JsonDSL\bin\Debug\netstandard2.0\JsonDSL.dll"

open JsonDSL
open System.Text.Json

let options = JsonSerializerOptions(WriteIndented = true)
type Person = {
    Name: string
    Email: string option
}

let convert (p:Person) = 
    object {
        property "name" p.Name
        property "email" (+. p.Email)
    }
    
let convertOptional (p:Person) = 
    object {
        property "name" p.Name
        property "email" (-. p.Email)
    }    
convertOptional {Name = "Duderich"; Email = Option.None} //
convertOptional {Name = "Duderich"; Email = Option.Some "yes@yes.yup"} //
object {property "hi" (-. (Option.None))}

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

//let x = 
//    array {
//        for i in [1.. 10] do
//            object {
//                property "number" i
           
//            }
//    }


x.ToJsonString(options)

jNode.ToJsonString(options)