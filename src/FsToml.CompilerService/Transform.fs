module FsToml.Transform.CompilerService

open FsToml.ProjectSystem
open Microsoft.FSharp.Compiler.SourceCodeServices

type Target = {
    FrameworkTarget   : FrameworkTarget
    FrameworkVersion  : FrameworkVersion
    PlatformType      : PlatformType
    BuildType         : BuildType
}

module Configuration =

    let satisfy (target : Target) (condition : Condition) =
        let result =
            condition.FrameworkTarget |> Option.forall ((=) target.FrameworkTarget) &&
            condition.FrameworkVersion |> Option.forall ((=) target.FrameworkVersion) &&
            condition.PlatformType |> Option.forall ((=) target.PlatformType) &&
            condition.BuildType |> Option.forall ((=) target.BuildType)
        let score =
            (condition.FrameworkTarget |> Option.count) +
            (condition.FrameworkVersion |> Option.count) +
            (condition.PlatformType |> Option.count) +
            (condition.BuildType |> Option.count)
        result, score

    let emptyCondition = {
        Condition.FrameworkTarget = None
        FrameworkVersion = None
        PlatformType = None
        BuildType = None
    }


    let emptyConfig ={
        Condition = emptyCondition
        Tailcalls = None
        WarningsAsErrors = None
        Constants = None
        DebugType = None
        DebugSymbols = None
        Optimize = None
        Prefer32bit = None
        WarningLevel = None
        OutputPath = None
        DocumentationFile = None
        NoWarn = None
        OtherFlags = None
    }

    let sum state e =
        {
            Condition = state.Condition
            Tailcalls = if state.Tailcalls.IsSome then state.Tailcalls else e.Tailcalls
            WarningsAsErrors = if state.WarningsAsErrors.IsSome then state.WarningsAsErrors else e.WarningsAsErrors
            Constants = if state.Constants.IsSome then state.Constants else e.Constants
            DebugType = if state.DebugType.IsSome then state.DebugType else e.DebugType
            DebugSymbols = if state.DebugSymbols.IsSome then state.DebugSymbols else e.DebugSymbols
            Optimize = if state.Optimize.IsSome then state.Optimize else e.Optimize
            Prefer32bit = if state.Prefer32bit.IsSome then state.Prefer32bit else e.Prefer32bit
            WarningLevel = if state.WarningLevel.IsSome then state.WarningLevel else e.WarningLevel
            OutputPath = if state.OutputPath.IsSome then state.OutputPath else e.OutputPath
            DocumentationFile = if state.DocumentationFile.IsSome then state.DocumentationFile else e.DocumentationFile
            NoWarn = if state.NoWarn.IsSome then state.NoWarn else e.NoWarn
            OtherFlags = if state.OtherFlags.IsSome then state.OtherFlags else e.OtherFlags

        }

    let getConfig (target : Target) (cfgs : Configuration[]) : Configuration =
        cfgs
        |> Seq.map (fun c -> c, (satisfy target c.Condition))
        |> Seq.choose (fun (c, (res, score)) -> if res then Some (c,score) else None)
        |> Seq.sortByDescending snd
        |> Seq.map fst
        |> Seq.fold sum emptyConfig

module References =
    open System
    open System.IO

    let sysLib ver nm =
        if Environment.OSVersion.Platform = PlatformID.Win32NT then
            // file references only valid on Windows
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) +
            @"\Reference Assemblies\Microsoft\Framework\.NETFramework\" + ver + "\\" + nm + ".dll"
        else
            let sysDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
            let (++) a b = Path.Combine(a,b)
            sysDir ++ nm + ".dll"

    let getPathToReference (target : Target) (reference : Reference) : string =
        if Path.IsPathRooted reference.Include then reference.Include
        elif File.Exists reference.Include then Path.GetFullPath reference.Include
        else
            sysLib (target.FrameworkVersion.ToString()) reference.Include


    let getReferences target = Array.map (getPathToReference target)


let toCompilerParams (target : Target) (project : FsTomlProject) =
    let cfg = project.Configurations |> Configuration.getConfig target
    let references = project.References |> References.getReferences target
    [|
        yield "-r:" + (References.sysLib  (target.FrameworkVersion.ToString()) "mscorlib")
        for r in references do
            yield "-r:" + r
    |]


let getFSharpProjectOptions  (target : Target) (project : FsTomlProject) =
    let parms = toCompilerParams target project
    let checker = FSharpChecker.Instance
    checker.GetProjectOptionsFromCommandLineArgs (project.Name, parms)




