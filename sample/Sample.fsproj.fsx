System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__
#r "../bin/FsToml.fsproj/Nett.dll"
#r "../bin/FsToml.fsproj/Forge.ProjectSystem.dll"
#r "../bin/FsToml.fsproj/FsToml.dll"
#r "../bin/FsToml.fsproj/FsToml.fsproj.dll"

#r "System.Xml"
#r "System.Xml.Linq"

open FsToml.Target
open FsToml.ProjectSystem

let t = FsToml.Parser.parse "testproject.toml"
let target = { Target.BuildType = BuildType.Debug; PlatformType = PlatformType.AnyCPU; FrameworkTarget = FrameworkTarget.Net; FrameworkVersion = FrameworkVersion.V4_6}
let p = FsToml.Transform.Fsproj.transform target t
p
