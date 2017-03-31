namespace FsToml.MsBuild

open System
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FsToml.Target
open FsToml.ProjectSystem
open System.IO


type FsTomlTask() =
    inherit Task()

    [<Required>] member val FrameworkTarget = "" with get,set
    [<Required>] member val FrameworkVersion = "" with get,set
    [<Required>] member val BuildType = "" with get,set
    [<Required>] member val PlatformType = "" with get,set
    [<Required>] member val ProjectFile = "" with get,set


    override this.Execute() =
        let proj = Path.ChangeExtension(this.ProjectFile, ".fstoml")
        let t = FsToml.Parser.parse proj
        let target = {
            Target.BuildType = defaultArg (BuildType.TryParse this.BuildType) BuildType.Debug;
            PlatformType = defaultArg (PlatformType.TryParse this.PlatformType) PlatformType.AnyCPU;
            FrameworkTarget = defaultArg (FrameworkTarget.TryParse this.FrameworkTarget) FrameworkTarget.Net;
            FrameworkVersion =defaultArg (FrameworkVersion.TryParse this.FrameworkVersion)  FrameworkVersion.V4_6}
        let p = FsToml.Transform.Fsproj.transform target t
        let ctn = p.ToXmlString()
        let dir = Path.GetDirectoryName proj
        let pathForProps = Path.Combine(dir, "obj", "fstoml.props")
        File.WriteAllText(pathForProps, ctn)
        true
