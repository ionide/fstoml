module FsToml.Transform.CompilerService

open FsToml
open FsToml.ProjectSystem
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SimpleSourceCodeServices

let getCompilerParams (target : Target.Target) ((path,project) : string * FsTomlProject) =
    [| |]

let getFSharpProjectOptions  (target : Target.Target) ((path,project) : string * FsTomlProject) =
    let parms = getCompilerParams target (path,project)
    let checker = FSharpChecker.Instance
    checker.GetProjectOptionsFromCommandLineArgs (project.Name, parms)

let getFSharpProjectOptionsFromFile (target : Target.Target option) path =
    let proj = Parser.parse path

    let t =
        match target with
        | Some t -> t
        | None ->
            let ver = defaultArg proj.FrameworkVersion FrameworkVersion.V4_6
            {
                Target.FrameworkTarget = FrameworkTarget.Net
                Target.FrameworkVersion = ver
                Target.PlatformType = PlatformType.AnyCPU
                Target.BuildType = BuildType.Debug
            }

    getFSharpProjectOptions t (path,proj)

let compile (target : Target.Target) ((path,project) : string * FsTomlProject) =
    let scs = SimpleSourceCodeServices()
    let prms = getCompilerParams target (path, project)
    printfn "Target:"
    printfn "%A" target
    printfn "Compieler parameters:"
    printfn "%A" prms
    prms |> scs.Compile



