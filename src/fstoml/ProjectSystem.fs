module FsToml.ProjectSystem

open System

type PlatformType =
    | X86 |  X64 | AnyCPU

    override self.ToString () = self |> function
        | X86     -> Constants.X86
        | X64     -> Constants.X64
        | AnyCPU  -> Constants.AnyCPU

    static member Parse text = text |> function
        | InvariantEqual Constants.X86     -> X86
        | InvariantEqual Constants.X64     -> X64
        | InvariantEqual "Any CPU"
        | InvariantEqual Constants.AnyCPU  -> AnyCPU
        | _ ->
            failwithf "Could not parse '%s' into a `PlatformType`" text

    static member TryParse text = text |> function
        | InvariantEqual Constants.X86     -> Some X86
        | InvariantEqual Constants.X64     -> Some X64
        | InvariantEqual "Any CPU"
        | InvariantEqual Constants.AnyCPU  -> Some AnyCPU
        | _ -> None

[<RequireQualifiedAccess>]
type BuildAction =
    /// Represents the source files for the compiler.
    | Compile
    /// Represents files that are not compiled into the project, but may be embedded or published together with it.
    | Content
    /// Represents an assembly (managed) reference in the project.
//    | Reference
    /// Represents files that should have no role in the build process
    | None
    | Resource
    /// Represents resources to be embedded in the generated assembly.
    | EmbeddedResource


    override self.ToString () = self |> function
        | Compile          -> Constants.Compile
        | Content          -> Constants.Content
  //      | Reference        -> Constants.Reference
        | None             -> Constants.None
        | Resource         -> Constants.Resource
        | EmbeddedResource -> Constants.EmbeddedResource

    static member Parse text = text |> function
        | InvariantEqual Constants.Compile          -> Compile
        | InvariantEqual Constants.Content          -> Content
    //    | InvariantEqual Constants.Reference        -> Reference
        | InvariantEqual Constants.None             -> None
        | InvariantEqual Constants.Resource         -> Resource
        | InvariantEqual Constants.EmbeddedResource -> EmbeddedResource
        | _ ->
            failwithf "Could not parse '%s' into a `BuildAction`" text

    static member TryParse text = text |> function
        | InvariantEqual Constants.Compile          -> Some Compile
        | InvariantEqual Constants.Content          -> Some Content
      //  | InvariantEqual Constants.Reference        -> Some Reference
        | InvariantEqual Constants.None             -> Some None
        | InvariantEqual Constants.Resource         -> Some Resource
        | InvariantEqual Constants.EmbeddedResource -> Some EmbeddedResource
        | _                          -> Option.None


// Under "Compile" in https://msdn.microsoft.com/en-us/library/bb629388.aspx
type CopyToOutputDirectory =
    | Never | Always | PreserveNewest

    override self.ToString () = self |> function
        | Never          -> Constants.Never
        | Always         -> Constants.Always
        | PreserveNewest -> Constants.PreserveNewest

    static member Parse text = text |> function
        | InvariantEqual Constants.Never          -> Never
        | InvariantEqual Constants.Always         -> Always
        | InvariantEqual Constants.PreserveNewest -> PreserveNewest
        | _ ->
            failwithf "Could not parse '%s' into a `CopyToOutputDirectory`" text

    static member TryParse text = text |> function
        | InvariantEqual Constants.Never          -> Some Never
        | InvariantEqual Constants.Always         -> Some Always
        | InvariantEqual Constants.PreserveNewest -> Some PreserveNewest
        | _                        -> None


[<RequireQualifiedAccess>]
type DebugType =
    | None | PdbOnly | Full

    override self.ToString () = self |> function
        | None    -> Constants.None
        | PdbOnly -> Constants.PdbOnly
        | Full    -> Constants.Full

    static member Parse text = text |> function
        | InvariantEqual Constants.None    -> DebugType.None
        | InvariantEqual Constants.PdbOnly -> DebugType.PdbOnly
        | InvariantEqual Constants.Full    -> DebugType.Full
        | _ ->
            failwithf "Could not parse '%s' into a `DebugType`" text

    static member TryParse text = text |> function
        | InvariantEqual Constants.None    -> Some DebugType.None
        | InvariantEqual Constants.PdbOnly -> Some DebugType.PdbOnly
        | InvariantEqual Constants.Full    -> Some DebugType.Full
        | _                 -> Option.None


/// Determines the output of compiling the F# Project
type OutputType =
    /// Build a console executable
    | Exe
    ///  Build a Windows executable
    | Winexe
    /// Build a library
    | Library
    /// Build a module that can be added to another assembly (.netmodule)
    | Module

    override self.ToString () = self |> function
        | Exe     -> Constants.Exe
        | Winexe  -> Constants.Winexe
        | Library -> Constants.Library
        | Module  -> Constants.Module

    static member Parse text = text |> function
        | InvariantEqual Constants.Exe     -> Exe
        | InvariantEqual Constants.Winexe  -> Winexe
        | InvariantEqual Constants.Library -> Library
        | InvariantEqual Constants.Module  -> Module
        | _ ->
            failwithf "Could not parse '%s' into a `OutputType`" text

    static member TryParse text = text |> function
        | InvariantEqual Constants.Exe     -> Some Exe
        | InvariantEqual Constants.Winexe  -> Some Winexe
        | InvariantEqual Constants.Library -> Some Library
        | InvariantEqual Constants.Module  -> Some Module
        | _                                -> None

[<RequireQualifiedAccess>]
type BuildType =
    | Debug
    | Release


    override self.ToString () = self |> function
        | Debug     -> Constants.Debug
        | Release  -> Constants.Release

    static member Parse text = text |> function
        | InvariantEqual Constants.Debug   -> Debug
        | InvariantEqual Constants.Release -> Release
        | _ ->
            failwithf "Could not parse '%s' into a `OutputType`" text

    static member TryParse text = text |> function
        | InvariantEqual Constants.Debug   -> Some Debug
        | InvariantEqual Constants.Release -> Some Release
        | _                                -> None

type SemVer =
    | SemVer of Major:int * Minor:int * Patch:int

    override self.ToString () = self |> function
        | SemVer (maj, min, p) -> sprintf "%d.%d.%d"  maj min p


type FSharpVer =
    | FSharpVer of Framework:int * Major:int * Minor:int * Patch:int

     override self.ToString () = self |> function
        | FSharpVer (f,maj, min, p) -> sprintf "%d.%d.%d.%d" f maj min p

//   Framework Versions :
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


// Framework Id
type FrameworkTarget =
    | Net
    | NetStandard
    | NetcoreApp

    override self.ToString () = self |> function
        | Net         -> ".NETFramework"
        | NetStandard -> ".NETStandard"
        | NetcoreApp  -> ".NETCoreApp"

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
}

type ProjectReference = {
    Include   : string
    Name      : string option
    Guid      : Guid option
    CopyLocal : bool option
}

type FsTomlProject = {
    FsTomlVersion     : SemVer
    Name              : string
    AssemblyName      : string
    RootNamespace     : string
    Guid              : Guid
    OutputType        : OutputType
    FSharpCore        : FSharpVer
    Configurations    : Configuration []
    Files             : SourceFile []
    References        : Reference []
    ProjectReferences : ProjectReference []
}
