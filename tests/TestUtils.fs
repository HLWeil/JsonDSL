module TestUtils

open System.Reflection
open System.IO

let getEmbeddedJsonString (filename:string) =
    let assembly = Assembly.GetExecutingAssembly()
    use str = assembly.GetManifestResourceStream($"JsonDSLTests.{filename}")
    use r = new StreamReader(str)
    r.ReadToEnd()