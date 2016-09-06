module FsToml.Parser

open Nett
open System
open ProjectSystem

type File () =
    member val Compile          : string         = null with get,set
    member val Content          : string         = null with get,set
    member val Resource         : string         = null with get,set
    member val EmbeddedResource : string         = null with get,set
    member val None             : string         = null with get,set
    member val Link             : string         = "" with get,set
    member val Copy             : string         = "" with get,set
    member val Sig              : string         = "" with get,set
    member val Private          : Nullable<bool> = Nullable() with get,set

type References () =
    member val Include         : string         = "" with get,set
    member val Name            : string         = "" with get,set
    member val HintPath        : string         = "" with get,set
    member val Private         : Nullable<bool> = Nullable() with get,set
    member val SpecificVersion : Nullable<bool> = Nullable() with get,set

type ProjectReferences () =
    member val Include : string         = "" with get,set
    member val Name    : string         = "" with get,set
    member val Project : Guid           = Guid.Empty with get,set
    member val Private : Nullable<bool> = Nullable() with get,set

type TomlProject () =
    member val FsTomlVersion     : string               = "" with get, set
    member val Name              : string               = "" with get, set
    member val AssemblyName      : string               = "" with get, set
    member val RootNamespace     : string               = "" with get, set
    member val Guid              : Guid                 = Guid.Empty with get, set
    member val OutputType        : string               = "" with get, set
    member val FSharpCore        : string               = "" with get, set
    member val DebugSymbols      : bool                 = false with get, set
    member val DebugType         : string               = "" with get, set
    member val Optimize          : bool                 = false with get, set
    member val Tailcalls         : bool                 = false with get,set
    member val WarningsAsErrors  : bool                 = false with get,set
    member val Prefer32bit       : bool                 = false with get,set
    member val WarningLevel      : int                  = 3 with get,set
    member val OutputPath        : string               = "" with get,set
    member val DocumentationFile : string               = "" with get,set
    member val Constants         : string []            = [||] with get,set
    member val NoWarn            : int []               = [||] with get, set
    member val OtherFlags        : string []            = [||] with get, set
    member val Files             : File []              = [||] with get,set
    member val References        : References []        = [||] with get,set
    member val ProjectReferences : ProjectReferences [] = [||] with get,set

let toProjectSystem (proj : TomlProject) : FsTomlProject =
    let version =
        let t = proj.FsTomlVersion.Split('.')
        SemVer (int t.[0], int t.[1], int t.[2])

    let fsharpVersion =
        let t = proj.FSharpCore.Split('.')
        FSharpVer (int t.[0], int t.[1], int t.[2], int t.[3])

    let config =
        {
            FrameworkTarget   = None
            FrameworkVersion  = None
            PlatformType      = None
            Tailcalls         = proj.Tailcalls
            WarningsAsErrors  = proj.WarningsAsErrors
            Constants         = proj.Constants
            DebugType         = proj.DebugType |> DebugType.Parse
            DebugSymbols      = proj.DebugSymbols
            Optimize          = proj.Optimize
            Prefer32bit       = proj.Prefer32bit
            WarningLevel      = proj.WarningLevel
            OutputPath        = proj.OutputPath
            DocumentationFile = proj.DocumentationFile
            NoWarn            = proj.NoWarn
            OtherFlags        = proj.OtherFlags
        }

    let projRefs =
        proj.ProjectReferences
        |> Array.map (fun p ->
            {
                ProjectReference.Include = p.Include
                Name                     = if p.Name = "" then None else Some p.Name
                Guid                     = Some p.Project
                CopyLocal                = p.Private |> Option.ofNullable |> Option.map (not)
            }
        )

    let refs =
        proj.References
        |> Array.map (fun r ->
            {
                Reference.Include = r.Include
                HintPath          = if r.HintPath = "" then None else Some r.HintPath
                Name              = if r.Name = "" then None else Some r.Name
                SpecificVersion   = r.SpecificVersion |> Option.ofNullable
                CopyLocal         = r.Private |> Option.ofNullable |> Option.map (not)
            }
        )

    let files =
        let isNotNull = isNull >> not

        proj.Files
        |> Array.map (fun f ->
            let incld, ba =
                if isNotNull f.Compile then f.Compile, BuildAction.Compile
                elif isNotNull f.Content then f.Content, BuildAction.Content
                elif isNotNull f.Resource then f.Resource, BuildAction.Resource
                elif isNotNull f.EmbeddedResource then f.EmbeddedResource, BuildAction.EmbeddedResource
                elif isNotNull f.None then f.None, BuildAction.None
                else "", BuildAction.None
            {
                SourceFile.Include = incld
                Link = if f.Link = "" then None else Some f.Link
                Copy = f.Copy |> CopyToOutputDirectory.TryParse
                OnBuild = ba
            }
        )

    {
        FsTomlVersion = version
        Name = proj.Name
        AssemblyName = proj.AssemblyName
        RootNamespace = proj.RootNamespace
        Guid = proj.Guid
        OutputType = proj.OutputType |> OutputType.Parse
        FSharpCore = fsharpVersion
        Configurations = [|config|]
        ProjectReferences = projRefs
        References = refs
        Files = files
    }

let parse (path : string) =
     Toml.ReadFile<_> path
     |> toProjectSystem
