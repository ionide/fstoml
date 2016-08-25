System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__
#r "../../packages/nett/lib/net40/nett.dll"
#r "bin/release/fstoml.dll"

open Nett
open System


type FsTomlProject () =
    member val FsTomlVersion : string = "" with get, set
    member val Name : string          = "" with get, set
    member val AssemblyName : string  = "" with get, set
    member val RootNamespace : string = "" with get, set
    member val Guid : Guid            = Guid.Empty with get, set
    member val OutputType : string    = "" with get, set
    member val FSharpCore : string    = "" with get, set
    member val DebugSymbols : bool    = false with get, set
    member val DebugType : string     = "" with get, set
    member val Optimize : bool        = false with get, set
    member val NoWarn : int []        = [||] with get, set
    member val OtherFlags : string [] = [||] with get, set


let table = Toml.ReadFile<FsTomlProject> "testproject.toml";;

table.AssemblyName;;
table.DebugSymbols;;
table.DebugType;;
table.FSharpCore;;
table.FsTomlVersion;;
table.Guid;;
table.Name;;
table.RootNamespace;;
table.OtherFlags;;
//|> Seq.iter (fun kvp -> printfn "%A - %A" kvp.Key (kvp.Value.Get<_>))

