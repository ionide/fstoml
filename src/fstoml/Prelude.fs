[<AutoOpen>]
module FsToml.Prelude
open System
open System.IO

// OPERATORS
//========================


let (^) = (<|)

let inline (|?|) (pred1:'a->bool) (pred2:'a->bool)  =
    fun a -> pred1 a || pred2 a

let inline (|&|) (pred1:'a->bool) (pred2:'a->bool)  =
    fun a -> pred1 a && pred2 a

/// Combines two path strings using Path.Combine
let inline combinePaths path1 (path2 : string) = Path.Combine (path1, path2.TrimStart [| '\\'; '/' |])
let inline combinePathsNoTrim path1 path2 = Path.Combine (path1, path2)

/// Combines two path strings using Path.Combine
let inline (@@) path1 path2 = combinePaths path1 path2
let inline (</>) path1 path2 = combinePathsNoTrim path1 path2


// ACTIVE PATTERNS
//=====================================

let (|InvariantEqual|_|) (str:string) arg =
  if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0
  then Some () else None