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

type Dependency () =
    member val Reference       : string         = null with get,set
    member val Project         : string         = null with get,set
    member val Package         : string         = null with get,set
    member val Name            : string         = "" with get,set
    member val HintPath        : string         = "" with get,set
    member val Private         : Nullable<bool> = Nullable() with get,set
    member val SpecificVersion : Nullable<bool> = Nullable() with get,set

type TomlProject () =
    member val Name              : string               = "" with get, set
    member val Guid              : Nullable<Guid>       = Nullable() with get, set
    member val AssemblyName      : string               = null with get, set
    member val RootNamespace     : string               = null with get, set
    member val OutputType        : string               = "" with get, set
    member val DebugType         : string               = "" with get, set
    member val OutputPath        : string               = "" with get, set
    member val DocumentationFile : string               = "" with get, set
    member val FrameworkVersion  : string               = "" with get, set
    member val DebugSymbols      : Nullable<bool>       = Nullable() with get, set
    member val Optimize          : Nullable<bool>       = Nullable() with get, set
    member val Tailcalls         : Nullable<bool>       = Nullable() with get, set
    member val WarningsAsErrors  : Nullable<bool>       = Nullable() with get, set
    member val Prefer32bit       : Nullable<bool>       = Nullable() with get, set
    member val WarningLevel      : Nullable<int>        = Nullable() with get, set
    member val Constants         : string []            = [||] with get, set
    member val NoWarn            : int []               = [||] with get, set
    member val OtherFlags        : string []            = [||] with get, set
    member val Files             : File []              = [||] with get, set
    member val Dependencies      : Dependency []        = [||] with get, set

let parseDebugType = function
    | InvariantEqual Constants.None    -> Some DebugType.None
    | InvariantEqual Constants.PdbOnly -> Some DebugType.PdbOnly
    | InvariantEqual Constants.Full    -> Some DebugType.Full
    | _                                -> None

let parseCopyToOutput = function
    | InvariantEqual Constants.Never          -> Some CopyToOutputDirectory.Never
    | InvariantEqual Constants.Always         -> Some CopyToOutputDirectory.Always
    | InvariantEqual Constants.PreserveNewest -> Some CopyToOutputDirectory.PreserveNewest
    | _                                       -> None

let parseOutputType t = t |> function
    | InvariantEqual Constants.Exe     -> OutputType.Exe
    | InvariantEqual Constants.Winexe  -> OutputType.Winexe
    | InvariantEqual Constants.Library -> OutputType.Library
    | InvariantEqual Constants.Module  -> OutputType.Module
    | _ ->
        failwithf "Could not parse '%s' into a `OutputType`" t

let getConfig condition (proj : TomlProject) =
    let cond =  Conditions.parseTomlCondition condition
    {
        Condition         = cond
        Tailcalls         = proj.Tailcalls |> Option.ofNullable
        WarningsAsErrors  = proj.WarningsAsErrors |> Option.ofNullable
        Constants         = if proj.Constants |> Array.isEmpty then None else Some proj.Constants
        DebugType         = proj.DebugType |> parseDebugType
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
    let isNotNull = isNull >> not

    let frameworkVersion =
        match proj.FrameworkVersion with
        | "4.5" -> Some FrameworkVersion.V4_5
        | "4.5.1" -> Some FrameworkVersion.V4_5_1
        | "4.6" -> Some FrameworkVersion.V4_6
        | "4.6.1" -> Some FrameworkVersion.V4_6_1
        | "4.6.2" -> Some FrameworkVersion.V4_6_2
        | _  -> None

    let config = getConfig "" proj

    let refs =
        proj.Dependencies
        |> Array.where (fun r -> isNull r.Project)
        |> Array.map (fun r ->
            let incl =
                if isNotNull r.Reference then r.Reference
                else r.Package

            {
                Reference.Include = incl
                HintPath          = if r.HintPath = "" then None else Some r.HintPath
                Name              = if r.Name = "" then None else Some r.Name
                SpecificVersion   = r.SpecificVersion |> Option.ofNullable
                CopyLocal         = r.Private |> Option.ofNullable |> Option.map (not)
                IsPackage         = isNotNull r.Package
            }
        )

    let projRefs =
        proj.Dependencies
        |> Array.where (fun r -> isNull r.Project |> not)
        |> Array.map (fun p ->
            {
                ProjectReference.Include = p.Project
                Name                     = if p.Name = "" then None else Some p.Name
                Guid                     = None
                CopyLocal                = p.Private |> Option.ofNullable |> Option.map (not)
            }
        )

    let files =

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
                Copy = f.Copy |> parseCopyToOutput
                OnBuild = ba
            }
        )

    {
        FrameworkVersion = frameworkVersion
        Name = proj.Name
        AssemblyName = proj.AssemblyName |> Option.ofObj
        RootNamespace = proj.RootNamespace |> Option.ofObj
        Guid = proj.Guid |> Option.ofNullable
        OutputType = proj.OutputType |> parseOutputType
        Configurations = [|config|]
        ProjectReferences = projRefs
        References = refs
        Files = files
        CustomConfigs = [||]
    }

let getCustomConfigs (name : string) (table : TomlTable) =
    name, table.Rows
    |> Seq.map (fun kv ->
        let k = kv.Key
        let v = kv.Value.Get<string>()
        Config(k,v))
    |> Seq.toList

let rec getTables (table : TomlTable) (name : string) : (Configuration list * (string * CustomConfig list) list) =
    let names =
        table.Rows
        |> Seq.filter (fun kv -> kv.Value.ReadableTypeName = "table" )
        |> Seq.map (fun kv -> kv.Key, if name ="" then kv.Key else name + "." + kv.Key )
        |> Seq.toList

    let res =
        names
        |> Seq.map (fun (m,n) -> getTables (table.Get<TomlTable>(m)) n  )
        |> Seq.toList

    let resC = res |> List.collect fst

    let resT = res |> List.collect snd

    let configNames, otherNames =
        names |> List.partition (fun (_,n) -> Conditions.canParseTomlCondition n)

    let configs =
        configNames
        |> List.map (fun (m,n) -> table.Get<TomlProject>(m) |> getConfig n)

    let customConfigs =
        otherNames
        |> List.map (fun (m,n) -> table.Get<TomlTable>(m) |> getCustomConfigs n )

    configs @ resC, customConfigs @ resT




let parse (path : string) =
    let main = Toml.ReadFile<TomlProject> path
    let fullFileTable = Toml.ReadFile<TomlTable> path
    let emptyConfig = TomlProject() |> getConfig ""
    let cfg, tables = getTables fullFileTable ""
    let configs = cfg |>  List.filter (fun n -> n <> { emptyConfig with Condition = n.Condition}) |> List.toArray
    let customCfgs = tables |> List.map (fun (name, t) -> name, t |> List.toArray ) |> List.toArray
    let res = main |> toProjectSystem
    {res with Configurations = Array.append res.Configurations configs; CustomConfigs = customCfgs}
