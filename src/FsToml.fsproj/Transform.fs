module FsToml.Transform.Fsproj

open System
open Forge.ProjectSystem
open FsToml.ProjectSystem
open Forge.XLinq
open Forge

module Conditions =
    let getPlatformCondition (platform : PlatformType) : string =
        sprintf "'$(Platform)' == '%s'" ^ platform.ToString()

    let getFrameworkTargetCondition (framework : FrameworkTarget) : string =
        sprintf "'$(TargetFrameworkIdentifier)' == '%s'" ^ framework.ToString()

    let getFrameworkVersionCondition (version : FrameworkVersion) : string =
        sprintf "'$(TargetFrameworkVersion)' == '%s'" ^ version.ToString()

    let getBuildTypeCondition (buildType : BuildType) : string =
        sprintf "'$(Configuration)' == '%s'" ^ buildType.ToString()

    let getCondition (condition : Condition) : string =
        let framework = condition.FrameworkTarget
        let version = condition.FrameworkVersion
        let platform = condition.PlatformType
        let build = condition.BuildType

        let fvp =
            match framework, version, platform with
            | Some f, Some v, Some p ->
                sprintf "(%s) AND (%s) AND (%s)" (getFrameworkTargetCondition f) (getFrameworkVersionCondition v) (getPlatformCondition p)
            | Some f, Some v, None ->
                sprintf "(%s) AND (%s)" (getFrameworkTargetCondition f) (getFrameworkVersionCondition v)
            | Some f, None, Some p ->
                sprintf "(%s) AND (%s)" (getFrameworkTargetCondition f) (getPlatformCondition p)
            | None , Some v, Some p ->
                sprintf "(%s) AND (%s)" (getFrameworkVersionCondition v) (getPlatformCondition p)
            | Some f, None, None ->
                sprintf "(%s)" (getFrameworkTargetCondition f)
            | None, Some v, None ->
                sprintf "(%s)" (getFrameworkVersionCondition v)
            | None, None, Some p ->
                sprintf "(%s)" (getPlatformCondition p)
            | None, None, None -> ""

        let b =
            match build with
            | Some b -> sprintf "(%s)" (getBuildTypeCondition b)
            | None  -> ""

        if fvp <> "" && b <> "" then fvp + " AND " + b else fvp + b



let emptyProperty name =
    {
        Name      = name
        Condition = None
        Data      = None
    }

let optProperty name data =
    {
        Name      = name
        Condition = None
        Data      = data
    }

let transformOutputType (input : FsToml.ProjectSystem.OutputType) =
    match input with
    | OutputType.Exe     -> Forge.ProjectSystem.Exe
    | OutputType.Library -> Forge.ProjectSystem.Library
    | OutputType.Module  -> Forge.ProjectSystem.Module
    | OutputType.Winexe  -> Forge.ProjectSystem.Winexe

let transformDebugType (input : DebugType) =
    match input with
    | DebugType.None    -> Forge.ProjectSystem.DebugType.None
    | DebugType.PdbOnly -> Forge.ProjectSystem.DebugType.PdbOnly
    | DebugType.Full    -> Forge.ProjectSystem.DebugType.Full

let transformBuildAction(input : BuildAction) =
    match input with
    | BuildAction.Compile          -> Forge.ProjectSystem.BuildAction.Compile
    | BuildAction.Content          -> Forge.ProjectSystem.BuildAction.Content
    | BuildAction.None             -> Forge.ProjectSystem.BuildAction.None
    | BuildAction.Resource         -> Forge.ProjectSystem.BuildAction.Resource
    | BuildAction.EmbeddedResource -> Forge.ProjectSystem.BuildAction.EmbeddedResource

let transformCopyToOutputDirectory (input : CopyToOutputDirectory) =
    match input with
    | CopyToOutputDirectory.Always         -> Forge.ProjectSystem.CopyToOutputDirectory.Always
    | CopyToOutputDirectory.Never          -> Forge.ProjectSystem.CopyToOutputDirectory.Never
    | CopyToOutputDirectory.PreserveNewest -> Forge.ProjectSystem.CopyToOutputDirectory.PreserveNewest

let transformBuildConfig(c : Configuration) : ConfigSettings =
    {
        Condition            = Conditions.getCondition c.Condition
        DebugSymbols         = optProperty Constants.DebugSymbols c.DebugSymbols
        DebugType            = optProperty Constants.DebugType (c.DebugType |> Option.map transformDebugType)
        Optimize             = optProperty Constants.Optimize c.Optimize
        Tailcalls            = optProperty Constants.Tailcalls c.Tailcalls
        OutputPath           = optProperty Constants.OutputPath c.OutputPath
        CompilationConstants = optProperty Constants.CompilationConstants (c.Constants |>Option.map (String.concat ";"))
        WarningLevel         = optProperty Constants.WarningLevel (c.WarningLevel |> Option.map WarningLevel)
        PlatformTarget       = optProperty Constants.PlatformTarget None
        Prefer32Bit          = optProperty Constants.Prefer32Bit c.Prefer32bit
        OtherFlags           = optProperty Constants.OtherFlags (c.OtherFlags |> Option.map List.ofArray)
    }

let transformSettings (tomlProj : FsTomlProject ) : ProjectSettings =
    {
        Name                         = property Constants.Name tomlProj.Name
        AssemblyName                 = property Constants.AssemblyName tomlProj.AssemblyName
        RootNamespace                = property Constants.RootNamespace tomlProj.RootNamespace
        Configuration                = emptyProperty Constants.Configuration
        Platform                     = emptyProperty Constants.Platform
        SchemaVersion                = property Constants.SchemaVersion "2.0"
        ProjectGuid                  = property Constants.ProjectGuid tomlProj.Guid
        ProjectType                  = emptyProperty Constants.ProjectType
        OutputType                   = property Constants.OutputType (transformOutputType tomlProj.OutputType)
        TargetFrameworkVersion       = emptyProperty Constants.TargetFrameworkVersion
        TargetFrameworkProfile       = emptyProperty Constants.TargetFrameworkProfile
        AutoGenerateBindingRedirects = emptyProperty Constants.AutoGenerateBindingRedirects
        TargetFSharpCoreVersion      = property Constants.TargetFSharpCoreVersion (tomlProj.FSharpCore.ToString())
        DocumentationFile            = emptyProperty Constants.DocumentationFile
    }

let transformSourceFile (tomlSourceFile : SourceFile) =
    {
        Forge.ProjectSystem.SourceFile.Include   = tomlSourceFile.Include
        Forge.ProjectSystem.SourceFile.Condition = None
        Forge.ProjectSystem.SourceFile.OnBuild   = transformBuildAction tomlSourceFile.OnBuild
        Forge.ProjectSystem.SourceFile.Link      = tomlSourceFile.Link
        Forge.ProjectSystem.SourceFile.Paket     = None
        Forge.ProjectSystem.SourceFile.Copy      = tomlSourceFile.Copy |> Option.map transformCopyToOutputDirectory
    }

let transformReference (tomlReference : Reference) =
    {
        Forge.ProjectSystem.Reference.Include         = tomlReference.Include
        Forge.ProjectSystem.Reference.Condition       = None
        Forge.ProjectSystem.Reference.HintPath        = tomlReference.HintPath
        Forge.ProjectSystem.Reference.Name            = tomlReference.Name
        Forge.ProjectSystem.Reference.SpecificVersion = tomlReference.SpecificVersion
        Forge.ProjectSystem.Reference.CopyLocal       = tomlReference.CopyLocal
        Forge.ProjectSystem.Reference.Paket           = None
    }

let transformProjectReference (tomlProjectReference : ProjectReference) =
    {
        Forge.ProjectSystem.ProjectReference.Include   = tomlProjectReference.Include
        Forge.ProjectSystem.ProjectReference.Condition = None
        Forge.ProjectSystem.ProjectReference.Name      = tomlProjectReference.Name
        Forge.ProjectSystem.ProjectReference.Guid      = tomlProjectReference.Guid
        Forge.ProjectSystem.ProjectReference.CopyLocal = tomlProjectReference.CopyLocal
    }


let transform (tomlProj : FsTomlProject ) : FsProject =
    {
        ToolsVersion      = "14.0"
        DefaultTargets    = ["Build"]
        BuildConfigs      = tomlProj.Configurations |> Array.map transformBuildConfig |> Array.toList
        ProjectReferences = tomlProj.ProjectReferences |> Array.map transformProjectReference |> ResizeArray
        References        = tomlProj.References |> Array.map transformReference |> ResizeArray
        SourceFiles       = tomlProj.Files |> Array.map transformSourceFile |> Array.toList |> SourceTree
        Settings          = transformSettings tomlProj
    }