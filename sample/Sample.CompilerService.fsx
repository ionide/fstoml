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

// [|"--noframework"; "--fullpaths"; "--flaterrors"; "--subsystemversion:6.00";
//     "--highentropyva+"; "--target:Library"; "--tailcalls-"; "--warnaserror-";
//     "--debug:full"; "--optimize-"; "--platofrm:AnyCPU"; "--warn:3";
//     "--out:bin\\Library1.dll"; "--doc:bin\\Library1.dll.xml"; "--nowarn:52,40";
//     "--warnon:1182";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\mscorlib.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\FSharp\\.NETFramework\\v4.0\\4.4.0.0\\FSharp.Core.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Fable.Core.dll";
//     "src/file.fs"; "src/uselessLink.fs"; "src/file3.fs"|]