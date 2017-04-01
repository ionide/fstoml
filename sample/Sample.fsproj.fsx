System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__
#r "../temp/bin/Nett.dll"
#r "../temp/bin/Forge.ProjectSystem.dll"
#r "../temp/bin/FsToml.dll"
#r "../temp/bin/FsToml.fsproj.dll"
#r "System.Xml"
#r "System.Xml.Linq"

open FsToml.Target
open FsToml.ProjectSystem

let t = FsToml.Parser.parse "sample.toml"
let target = { Target.BuildType = BuildType.Debug; PlatformType = PlatformType.AnyCPU; FrameworkTarget = FrameworkTarget.Net; FrameworkVersion = FrameworkVersion.V4_6}
let p = FsToml.Transform.Fsproj.transform target t
p.ToXmlString()
