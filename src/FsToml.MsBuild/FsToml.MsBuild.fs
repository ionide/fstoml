module FsToml.MsBuild

open System
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
    let objDir = Path.Combine(dir, "obj")
    let pathForProps = Path.Combine(objDir, filename + ".fstoml.props")
    let ctn' = ctn.Replace("""ToolsVersion="" DefaultTargets="" """, """ToolsVersion="14.0" """)
    if not (Directory.Exists objDir) then Directory.CreateDirectory objDir |> ignore
    File.WriteAllText(pathForProps, ctn')


    0