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
    open System.IO

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

    let getCompilerParams (target : Target) (name : string) (cfg : Configuration) =

        let debug =
            if cfg.DebugSymbols |> Option.exists id then ":full"
            elif cfg.DebugType.IsSome then
                match cfg.DebugType.Value with
                | DebugType.None -> "-"
                | DebugType.Full -> ":full"
                | DebugType.PdbOnly -> ":pdbonly"
            else "-"

        let platofrm =
            if target.PlatformType = PlatformType.AnyCPU && cfg.Prefer32bit |> Option.exists id then
                "anycpu32bitpreferred"
            else
                target.PlatformType.ToString()

        let outPath = defaultArg (cfg.OutputPath |> Option.map (fun p -> Path.Combine(p,name))) (Path.Combine("bin", name))
        let xmlPath = defaultArg cfg.DocumentationFile (outPath + ".xml")

        [|
            yield "--tailcalls" + if cfg.Tailcalls |> Option.exists id then "+" else "-"
            yield "--warnaserror" + if cfg.WarningsAsErrors |> Option.exists id then "+" else "-"

            if cfg.Constants.IsSome then
                for c in cfg.Constants.Value do
                    yield "-d:" + c
            yield "--debug" + debug
            yield "--optimize" + if cfg.Optimize |> Option.exists id then "+" else "-"
            yield "--platofrm:" + platofrm
            yield "--warn:" + string (defaultArg cfg.WarningLevel 3)
            yield "--out:" + outPath
            yield "--doc:" + xmlPath
            match cfg.NoWarn with
            | None | Some [||] -> ()
            | Some nowarns -> yield "--nowarn:" + (nowarns |> Seq.map (string) |> String.concat ",")
            match cfg.OtherFlags with
            | None | Some [||] -> ()
            | Some flags -> yield! flags
        |]

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

    let fsCore ver =
        if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then
            // file references only valid on Windows
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) +
            @"\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\" + ver + @"\FSharp.Core.dll"
        else
            sysLib ver "FSharp.Core"

    let getPathToReference (target : Target) (reference : Reference) : string =
        if Path.IsPathRooted reference.Include then reference.Include
        elif File.Exists reference.Include then Path.GetFullPath reference.Include
        else
            sysLib (target.FrameworkVersion.ToString()) reference.Include

    let getCompilerParams target fsharpCore (refs : Reference[]) =
        let references =
            refs
            |> Array.filter (fun r -> r.Include <> "FSharp.Core" && r.Include <> "mscorlib")
            |> Array.map (getPathToReference target)
        [|
            yield "-r:" + (sysLib  (target.FrameworkVersion.ToString()) "mscorlib")
            yield "-r:" + (fsCore fsharpCore)
            for r in references do
                yield "-r:" + r
        |]

module Files =
    let getCompilerParams (files : SourceFile[]) =
        files
        |> Array.filter(fun r -> r.OnBuild = BuildAction.Compile)
        |> Array.map(fun r -> r.Link |> Option.fold (fun s e -> e) r.Include)



let getCompilerParams (target : Target) (project : FsTomlProject) =
    let name =
        project.AssemblyName +
            match project.OutputType with
            | OutputType.Library -> ".dll"
            | _ -> ".exe"

    let cfg = project.Configurations |> Configuration.getConfig target |> Configuration.getCompilerParams target name
    let refs = project.References |> References.getCompilerParams target (project.FSharpCore.ToString())
    let files = project.Files |> Files.getCompilerParams

    [|
        yield "--noframework"
        yield "--fullpaths"
        yield "--flaterrors"
        yield "--subsystemversion:6.00"
        yield "--highentropyva+"
        yield "--target:" + project.OutputType.ToString()
        yield! cfg
        yield! refs
        yield! files
    |]


let getFSharpProjectOptions  (target : Target) (project : FsTomlProject) =
    let parms = getCompilerParams target project
    let checker = FSharpChecker.Instance
    checker.GetProjectOptionsFromCommandLineArgs (project.Name, parms)




