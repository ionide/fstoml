System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__

#r "../bin/FsToml.CompilerService/Nett.dll"
#r "../bin/FsToml.CompilerService/FSharp.Compiler.Service.dll"
#r "../bin/FsToml.CompilerService/FsToml.dll"
#r "../bin/FsToml.CompilerService/FsToml.CompilerService.dll"
#r "System.Xml"
#r "System.Xml.Linq"

open FsToml.ProjectSystem
open FsToml.Transform.CompilerService

let target = {
    FrameworkTarget = FrameworkTarget.Net
    FrameworkVersion = FrameworkVersion.V4_5
    PlatformType = PlatformType.AnyCPU
    BuildType = BuildType.Release  }

let t = FsToml.Parser.parse "testproject.toml"
let p = FsToml.Transform.CompilerService.getCompilerParams target t
p

// [|"-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\mscorlib.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\FSharp\\.NETFramework\\v4.0\\4.4.0.0\\FSharp.Core.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Fable.Core.dll"|]