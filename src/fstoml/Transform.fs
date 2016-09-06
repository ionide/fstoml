module FsToml.Transform

open System
open Forge.ProjectSystem
open FsToml.ProjectSystem
open Forge.XLinq
open Forge

let emptyProperty name =
    {
        Name      = name
        Condition = None
        Data      = None
    }

let transformOutputType (input : FsToml.ProjectSystem.OutputType) =
    match input with
    | Exe     -> Forge.ProjectSystem.Exe
    | Library -> Forge.ProjectSystem.Library
    | Module  -> Forge.ProjectSystem.Module
    | Winexe  -> Forge.ProjectSystem.Winexe

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
        Condition            = Conditions.getCondition c.FrameworkTarget c.FrameworkVersion c.PlatformType
        DebugSymbols         = property Constants.DebugSymbols c.DebugSymbols
        DebugType            = property Constants.DebugType (transformDebugType c.DebugType)
        Optimize             = property Constants.Optimize c.Optimize
        Tailcalls            = property Constants.Tailcalls c.Tailcalls
        OutputPath           = property Constants.OutputPath c.OutputPath
        CompilationConstants = property Constants.CompilationConstants (c.Constants |> String.concat ";")
        WarningLevel         = property Constants.WarningLevel (c.WarningLevel |> WarningLevel)
        PlatformTarget       = property Constants.PlatformTarget Forge.ProjectSystem.PlatformType.AnyCPU //TODO: Always AnyCPU ??
        Prefer32Bit          = property Constants.Prefer32Bit c.Prefer32bit
        OtherFlags           = property Constants.OtherFlags (c.OtherFlags |> List.ofArray)
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