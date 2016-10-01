module FsToml.CLI

open System.IO

open Argu
open FsToml
open FsToml.Prelude
open FsToml.Transform

module Commands =
    let compile target path =
        try
            let path = Path.GetFullPath path
            let proj = Parser.parse path
            let (errors, status) = CompilerService.compile target (path, proj)

            if status = 0 then
                printfn "OK"
            else
                printfn "Error status code: %d" status
                errors |> Array.iter(fun e -> printfn "(%s) [%d:%d - %d:%d] %s" (e.Severity.ToString()) e.StartLineAlternate e.StartColumn e.EndLineAlternate e.EndColumn e.Message )
        with
        | ex ->
            printfn "Fatal error: %s" ex.Message
            printfn "Trace: %s" ex.StackTrace
            printfn "Data: %A" ex.Data

    let fsproj path =
        try
            let fn = Path.GetFileNameWithoutExtension path
            let path = Path.GetFullPath path
            let dir = Path.GetDirectoryName path
            let proj = Parser.parse path
            let (fproj, refs) = Fsproj.transform proj

            File.WriteAllText (dir </> fn + ".fsproj", fproj.ToXmlString())
            File.WriteAllLines (dir </> "paket.references", refs)
        with
        | ex ->
            printfn "Fatal error: %s" ex.Message

module Arguments =
    open ProjectSystem
    open Target

    type Framework =
        | Full
        | Netcore

    type Version =
        | ``4.5`` = 1
        | ``4.5.1`` = 2
        | ``4.6`` = 3
        | ``4.6.1`` = 4
        | ``4.6.2`` = 5

    type Platform =
        | X64
        | X86
        | AnyCPU

    type Build =
        | Debug
        | Release

    type FsprojArgs =
        | [<MainCommand; ExactlyOnce; Last>] Proj of string
    with
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Proj _ -> "Relative or absolute path to `project.fstoml` file. Required"


    type CompileArgs =
        // | Framework of Framework option
        | Version of Version option
        | Platform of Platform option
        | Build of Build option
        | [<MainCommand; ExactlyOnce; Last>] Proj of string
    with
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                // | Framework _ -> "Specify target framework profile of this assembly. Valid values are `full` or `netcore`. Default value: `full`"
                | Version _ -> "Specify target framework version of this assembly. Valid values are `4.5`, `4.5.1`, `4.6`, `4.6.1`, `4.6.2`. Default value: `4.5`"
                | Platform _ -> "Specify target platform of this assembly. Valid values are `x64`, `x86`, `AnyCPU`. Default value: `AnyCPU`"
                | Build _ -> "Specify target build type of this assembly. Valid values are `Debug` and `Release`. Default value: `Debug`"
                | Proj _ -> "Relative or absolute path to `project.fstoml` file. Required"


    type Args =
        | [<CliPrefix(CliPrefix.None)>] Compile of ParseResults<CompileArgs>
        | [<CliPrefix(CliPrefix.None)>] Fsproj of ParseResults<FsprojArgs>
    with
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Compile _ -> "Compiles project"
                | Fsproj _ -> "Transforms project to `*.fsproj` file"

    let toTarget (pr : ParseResults<CompileArgs>) =
        let proj = pr.GetResult <@ CompileArgs.Proj @>

        let target = FrameworkTarget.Net
            // match pr.TryGetResult <@ CompileArgs.Framework @> |> Option.bind id with
            // | None | Some Full -> FrameworkTarget.Net
            // | Some Netcore -> FrameworkTarget.NetcoreApp

        let ver =
            match target with
            | FrameworkTarget.NetcoreApp -> FrameworkVersion.V1_0
            | _ ->
                match pr.TryGetResult <@ CompileArgs.Version @> |> Option.bind id with
                | None -> FrameworkVersion.V4_5
                | Some ``4.5`` -> FrameworkVersion.V4_5
                | Some ``4.5.1`` -> FrameworkVersion.V4_5_1
                | Some ``4.6`` -> FrameworkVersion.V4_6
                | Some ``4.6.1`` -> FrameworkVersion.V4_6_1
                | Some ``4.6.2`` -> FrameworkVersion.V4_6_2

        let bld =
            match pr.TryGetResult <@ CompileArgs.Build @> |> Option.bind id with
            | None | Some Debug -> BuildType.Debug
            | Some Release -> BuildType.Release

        let plt =
            match pr.TryGetResult <@ CompileArgs.Platform @> |> Option.bind id with
            | None | Some AnyCPU -> PlatformType.AnyCPU
            | Some X64 -> PlatformType.X64
            | Some X86 -> PlatformType.X86

        let target = {
            Target.FrameworkTarget = target
            FrameworkVersion = ver
            BuildType = bld
            PlatformType = plt
        }
        proj, target

    let toPath (pr : ParseResults<FsprojArgs>) =
        pr.GetResult <@ FsprojArgs.Proj @>




[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<Arguments.Args>()
    let res = parser.Parse argv
    let all = res.GetAllResults()
    match all with
    | [(Arguments.Compile c)] ->
        let path, target = c |> Arguments.toTarget
        Commands.compile target path
        0
    | [(Arguments.Fsproj f)] ->
        f |> Arguments.toPath |> Commands.fsproj
        0
    | _ ->
        printfn "%s" ^ parser.PrintUsage()
        1
