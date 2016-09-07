module FsToml.Parser

open Nett
open System
open ProjectSystem
open Conditions

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
    member val Guid              : Guid                 = Guid.Empty with get, set
    member val AssemblyName      : string               = "" with get, set
    member val RootNamespace     : string               = "" with get, set
    member val OutputType        : string               = "" with get, set
    member val FSharpCore        : string               = "" with get, set
    member val DebugType         : string               = "" with get, set
    member val OutputPath        : string               = "" with get,set
    member val DocumentationFile : string               = "" with get,set
    member val DebugSymbols      : Nullable<bool>       = Nullable() with get, set
    member val Optimize          : Nullable<bool>       = Nullable() with get, set
    member val Tailcalls         : Nullable<bool>       = Nullable() with get,set
    member val WarningsAsErrors  : Nullable<bool>       = Nullable() with get,set
    member val Prefer32bit       : Nullable<bool>       = Nullable() with get,set
    member val WarningLevel      : Nullable<int>        = Nullable() with get,set
    member val Constants         : string []            = [||] with get,set
    member val NoWarn            : int []               = [||] with get, set
    member val OtherFlags        : string []            = [||] with get, set
    member val Files             : File []              = [||] with get,set
    member val References        : References []        = [||] with get,set
    member val ProjectReferences : ProjectReferences [] = [||] with get,set

let getConfig condition (proj : TomlProject) =
    let cond =  Conditions.parseTomlCondition condition
    {
        Condition         = cond
        Tailcalls         = proj.Tailcalls |> Option.ofNullable
        WarningsAsErrors  = proj.WarningsAsErrors |> Option.ofNullable
        Constants         = if proj.Constants |> Array.isEmpty then None else Some proj.Constants
        DebugType         = proj.DebugType |> DebugType.TryParse
        DebugSymbols      = proj.DebugSymbols |> Option.ofNullable
        Optimize          = proj.Optimize |> Option.ofNullable
        Prefer32bit       = proj.Prefer32bit |> Option.ofNullable
        WarningLevel      = proj.WarningLevel |> Option.ofNullable
        OutputPath        = if proj.OutputPath = "" then None else Some proj.OutputPath
        DocumentationFile = if proj.DocumentationFile = "" then None else Some proj.OutputPath
        NoWarn            = if proj.NoWarn |> Array.isEmpty then None else Some proj.NoWarn
        OtherFlags        = if proj.OtherFlags |> Array.isEmpty then None else Some proj.OtherFlags
    }

let toProjectSystem (proj : TomlProject) : FsTomlProject =
    let version =
        let t = proj.FsTomlVersion.Split('.')
        SemVer (int t.[0], int t.[1], int t.[2])

    let fsharpVersion =
        let t = proj.FSharpCore.Split('.')
        FSharpVer (int t.[0], int t.[1], int t.[2], int t.[3])

    let config = getConfig "" proj


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

let rec getTables (table : TomlTable) (name : string) =
    let names =
        table.Rows
        |> Seq.filter (fun kv -> kv.Value.ReadableTypeName = "table" )
        |> Seq.map (fun kv -> kv.Key, if name ="" then kv.Key else name + "." + kv.Key )
        |> Seq.toList

    let res =
        names
        |> Seq.map (fun (m,n) -> getTables (table.Get<TomlTable>(m)) n  )
        |> Seq.toList
        |> List.collect id

    let configs =
        names |> List.map (fun (m,n) -> table.Get<TomlProject>(m) |> getConfig n)

    configs @ res

let parse (path : string) =
     let main = Toml.ReadFile<_> path
     let asTable = Toml.ReadFile<TomlTable> path
     let emptyConfig = TomlProject() |> getConfig ""
     let configs = getTables asTable "" |>  List.filter (fun n -> n <> { emptyConfig with Condition = n.Condition}) |> List.toArray
     let res = main |> toProjectSystem
     {res with Configurations = Array.append res.Configurations configs}
