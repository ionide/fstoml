module FsToml.MsBuild

open System
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FsToml.Target
open FsToml.ProjectSystem
open System.IO

[<EntryPoint>]
let main argv =
    let proj = Path.ChangeExtension(argv.[0], ".fstoml")
    let filename = Path.GetFileName(argv.[0])
    let t = FsToml.Parser.parse proj
    let target = {
        Target.BuildType = defaultArg (BuildType.TryParse argv.[1]) BuildType.Debug;
        PlatformType = defaultArg (PlatformType.TryParse argv.[2]) PlatformType.AnyCPU;
        FrameworkTarget = defaultArg (FrameworkTarget.TryParse argv.[3]) FrameworkTarget.Net;
        FrameworkVersion = defaultArg (FrameworkVersion.TryParse argv.[4])  FrameworkVersion.V4_6}
    let p = FsToml.Transform.Fsproj.transform target t
    let ctn = p.ToXmlString()
    let dir = Path.GetDirectoryName proj
    let pathForProps = Path.Combine(dir, "obj", filename + ".fstoml.targets")
    File.WriteAllText(pathForProps, ctn)


    0