module Test

open System.Reflection
open System.Runtime.Versioning

[<EntryPoint>]
let main argv =
    let asm = Assembly.GetEntryAssembly()
    let att = asm.GetCustomAttributes(typedefof<TargetFrameworkAttribute>, false).[0] :?> TargetFrameworkAttribute
    let name = att.FrameworkName

    if name <> ".NETFramework,Version=v4.5" then
        failwithf "Wrong Framework profile or version. Assembly version: %s" name
    0
