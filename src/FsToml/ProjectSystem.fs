module FsToml.ProjectSystem

open System

[<RequireQualifiedAccess>]
type PlatformType =
    | X86 |  X64 | AnyCPU

    override self.ToString () = self |> function
        | X86     -> Constants.X86
        | X64     -> Constants.X64
        | AnyCPU  -> Constants.AnyCPU

    static member TryParse = function
        | Constants.X86 -> Some X86
        | Constants.X64 -> Some X64
        | Constants.AnyCPU -> Some AnyCPU
        | _ -> None

[<RequireQualifiedAccess>]
type BuildAction =
    | Compile
    | Content
    | None
    | Resource
    | EmbeddedResource

    override self.ToString () = self |> function
        | Compile          -> Constants.Compile
        | Content          -> Constants.Content
        | None             -> Constants.None
        | Resource         -> Constants.Resource
        | EmbeddedResource -> Constants.EmbeddedResource


[<RequireQualifiedAccess>]
type CopyToOutputDirectory =
    | Never | Always | PreserveNewest

    override self.ToString () = self |> function
        | Never          -> Constants.Never
        | Always         -> Constants.Always
        | PreserveNewest -> Constants.PreserveNewest


[<RequireQualifiedAccess>]
type DebugType =
    | None | PdbOnly | Full

    override self.ToString () = self |> function
        | None    -> Constants.None
        | PdbOnly -> Constants.PdbOnly
        | Full    -> Constants.Full

[<RequireQualifiedAccess>]
type OutputType =
    | Exe
    | Winexe
    | Library
    | Module

    override self.ToString () = self |> function
        | Exe     -> Constants.Exe
        | Winexe  -> Constants.Winexe
        | Library -> Constants.Library
        | Module  -> Constants.Module

[<RequireQualifiedAccess>]
type BuildType =
    | Debug
    | Release


    override self.ToString () = self |> function
        | Debug     -> Constants.Debug
        | Release  -> Constants.Release

    static member TryParse = function
        | Constants.Debug -> Some Debug
        | Constants.Release -> Some Release
        | _ -> None

[<RequireQualifiedAccess>]
type FrameworkVersion =
    | V1_0
    | V1_1
    | V1_2
    | V1_3
    | V1_4
    | V1_5
    | V1_6
    | V4_5
    | V4_5_1
    | V4_6
    | V4_6_1
    | V4_6_2

    override self.ToString () = self |> function
        | V1_0   -> "v1.0"
        | V1_1   -> "v1.1"
        | V1_2   -> "v1.2"
        | V1_3   -> "v1.3"
        | V1_4   -> "v1.4"
        | V1_5   -> "v1.5"
        | V1_6   -> "v1.6"
        | V4_5  -> "v4.5"
        | V4_5_1 -> "v4.5.1"
        | V4_6   -> "v4.6"
        | V4_6_1 -> "v4.6.1"
        | V4_6_2 -> "v4.6.2"

    static member TryParse = function
        | "v1.0" -> Some V1_0
        | "v1.1" -> Some V1_1
        | "v1.2" -> Some V1_2
        | "v1.3" -> Some V1_3
        | "v1.4" -> Some V1_4
        | "v1.5" -> Some V1_5
        | "v1.6" -> Some V1_6
        | "v4.5" -> Some V4_5
        | "v4.5.1" -> Some V4_5_1
        | "v4.6" -> Some V4_6
        | "v4.6.1" -> Some V4_6_1
        | "v4.6.2" -> Some V4_6_2
        | _ -> None


[<RequireQualifiedAccess>]
type FrameworkTarget =
    | Net
    | NetStandard
    | NetcoreApp

    override self.ToString () = self |> function
        | Net         -> ".NETFramework"
        | NetStandard -> ".NETStandard"
        | NetcoreApp  -> ".NETCoreApp"

    static member TryParse = function
        | ".NETFramework" -> Some Net
        | ".NETStandard" -> Some NetStandard
        | ".NETCoreApp" -> Some NetcoreApp
        | _ -> None

type Condition = {
    FrameworkTarget   : FrameworkTarget option
    FrameworkVersion  : FrameworkVersion option
    PlatformType      : PlatformType option
    BuildType         : BuildType option
}

type Configuration = {
    Condition         : Condition
    Tailcalls         : bool option
    WarningsAsErrors  : bool option
    Constants         : string [] option
    DebugType         : DebugType option
    DebugSymbols      : bool option
    Optimize          : bool option
    Prefer32bit       : bool option
    WarningLevel      : int option
    OutputPath        : string option
    DocumentationFile : string option
    NoWarn            : int [] option
    OtherFlags        : string [] option
}

type SourceFile = {
    Include     : string
    OnBuild     : BuildAction
    Link        : string option
    Copy        : CopyToOutputDirectory option
}

type Reference = {
    Include         : string
    HintPath        : string option
    Name            : string option
    SpecificVersion : bool option
    CopyLocal       : bool option
    IsPackage       : bool
}

type ProjectReference = {
    Include   : string
    Name      : string option
    Guid      : Guid option
    CopyLocal : bool option
}

type FsTomlProject = {
    Name              : string
    AssemblyName      : string option
    RootNamespace     : string option
    Guid              : Guid option
    OutputType        : OutputType
    FrameworkVersion  : FrameworkVersion option
    Configurations    : Configuration []
    Files             : SourceFile []
    References        : Reference []
    ProjectReferences : ProjectReference []
}
