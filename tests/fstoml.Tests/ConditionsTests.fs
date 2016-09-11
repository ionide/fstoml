module FsToml.Tests.Conditions

open NUnit.Framework
open FsUnit

open FsToml
open Conditions
open ProjectSystem


[<Test>]
let ``Parse `netcore.1_6.Release.x86` `` () =
    """netcore.1_6.Release.x86"""
    |> parseTomlCondition
    |> should equal {
        FrameworkTarget = Some FrameworkTarget.NetcoreApp
        FrameworkVersion = Some FrameworkVersion.V1_6
        BuildType = Some BuildType.Release
        PlatformType = Some PlatformType.X86
      }

[<Test>]
let ``Parse `netcore.1_6.Release.x64` `` () =
    """netcore.1_6.Release.x64"""
    |> parseTomlCondition
    |> should equal {
        FrameworkTarget = Some FrameworkTarget.NetcoreApp
        FrameworkVersion = Some FrameworkVersion.V1_6
        BuildType = Some BuildType.Release
        PlatformType = Some PlatformType.X64
      }

[<Test>]
let ``Parse `netcore.1_6.Release.AnyCPU` `` () =
    """netcore.1_6.Release.AnyCPU"""
    |> parseTomlCondition
    |> should equal {
        FrameworkTarget = Some FrameworkTarget.NetcoreApp
        FrameworkVersion = Some FrameworkVersion.V1_6
        BuildType = Some BuildType.Release
        PlatformType = Some PlatformType.AnyCPU
      }

[<Test>]
let ``Parse `netcore.1_6.Release.Blah` `` () =
    """netcore.1_6.Release.Blah"""
    |> parseTomlCondition
    |> should equal {
        FrameworkTarget = Some FrameworkTarget.NetcoreApp
        FrameworkVersion = Some FrameworkVersion.V1_6
        BuildType = Some BuildType.Release
        PlatformType = None
      }

[<Test>]
let ``Parse `netcore.1_6.Release` `` () =
    """netcore.1_6.Release"""
    |> parseTomlCondition
    |> should equal {
        FrameworkTarget = Some FrameworkTarget.NetcoreApp
        FrameworkVersion = Some FrameworkVersion.V1_6
        BuildType = Some BuildType.Release
        PlatformType = None
      }


[<Test>]
let ``Parse `netcore.1_6.Debug.x86` `` () =
    """netcore.1_6.Debug.x86"""
    |> parseTomlCondition
    |> should equal {
        FrameworkTarget = Some FrameworkTarget.NetcoreApp
        FrameworkVersion = Some FrameworkVersion.V1_6
        BuildType = Some BuildType.Debug
        PlatformType = Some PlatformType.X86
      }

[<Test>]
let ``Parse `netcore.1_6.Blah.x86` `` () =
    """netcore.1_6.Blah.x86"""
    |> parseTomlCondition
    |> should equal {
        FrameworkTarget = Some FrameworkTarget.NetcoreApp
        FrameworkVersion = Some FrameworkVersion.V1_6
        BuildType = None
        PlatformType = Some PlatformType.X86
      }

[<Test>]
let ``Parse `netcore.1_6.x86` `` () =
    """netcore.1_6.x86"""
    |> parseTomlCondition
    |> should equal {
        FrameworkTarget = Some FrameworkTarget.NetcoreApp
        FrameworkVersion = Some FrameworkVersion.V1_6
        BuildType = None
        PlatformType = Some PlatformType.X86
      }

[<Test>]
let ``Parse `net.4_6.Release.x86` `` () =
    """net.4_6.Release.x86"""
    |> parseTomlCondition
    |> should equal {
        FrameworkTarget = Some FrameworkTarget.Net
        FrameworkVersion = Some FrameworkVersion.V4_6
        BuildType = Some BuildType.Release
        PlatformType = Some PlatformType.X86
      }

[<Test>]
let ``Parse `netstandard.1_0.Release.x86` `` () =
    """netstandard.1_0.Release.x86"""
    |> parseTomlCondition
    |> should equal {
        FrameworkTarget = Some FrameworkTarget.NetStandard
        FrameworkVersion = Some FrameworkVersion.V1_0
        BuildType = Some BuildType.Release
        PlatformType = Some PlatformType.X86
      }