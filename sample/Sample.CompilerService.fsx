System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__

#r "../bin/FsToml.CompilerService/Chessie.dll"
#r "../bin/FsToml.CompilerService/Newtonsoft.Json.dll"
#r "../bin/FsToml.CompilerService/Paket.Core.dll"
#r "../bin/FsToml.CompilerService/System.Collections.Immutable.dll"
#r "../bin/FsToml.CompilerService/System.Reflection.Metadata.dll"
#r "../bin/FsToml.CompilerService/Nett.dll"
#r "../bin/FsToml.CompilerService/FSharp.Compiler.Service.dll"
#r "../bin/FsToml.CompilerService/FSharp.Compiler.Service.ProjectCracker.dll"
#r "../bin/FsToml.CompilerService/FsToml.dll"
#r "../bin/FsToml.CompilerService/FsToml.CompilerService.dll"
#r "System.Xml"
#r "System.Xml.Linq"

open FsToml.ProjectSystem
open FsToml.Target
open FsToml.Transform.CompilerService
open Microsoft.FSharp.Compiler.SourceCodeServices

let target = {
    FrameworkTarget = FrameworkTarget.Net
    FrameworkVersion = FrameworkVersion.V4_5
    PlatformType = PlatformType.AnyCPU
    BuildType = BuildType.Release  }

let path = System.IO.Path.GetFullPath "testproject.CompielrService.toml"
let t = FsToml.Parser.parse "testproject.CompilerService.toml"
let p = FsToml.Transform.CompilerService.getCompilerParams target (path, t)
p


// [|"--noframework"; "--fullpaths"; "--flaterrors"; "--subsystemversion:6.00";
//     "--highentropyva+"; "--target:Library"; "--tailcalls+"; "--warnaserror-";
//     "-d:TRACE"; "--debug:pdbonly"; "--optimize+"; "--platofrm:AnyCPU";
//     "--warn:3"; "--out:bin\\Release\\FsToml.CompilerService.dll";
//     "--doc:bin\\Release\\FsToml.CompilerService.dll.xml";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\mscorlib.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\FSharp\\.NETFramework\\v4.0\\4.4.0.0\\FSharp.Core.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.Core.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.Numerics.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.Xml.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.Xml.Linq.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\FSharp.Compiler.Service\\lib\\net45\\FSharp.Compiler.Service.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\FSharp.Compiler.Service.ProjectCracker\\lib\\net45\\FSharp.Compiler.Service.ProjectCracker.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\Paket.Core\\lib\\net45\\Paket.Core.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Collections.Concurrent.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Collections.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.ComponentModel.Annotations.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.ComponentModel.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.ComponentModel.EventBasedAsync.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Diagnostics.Contracts.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Diagnostics.Debug.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Diagnostics.Tools.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Diagnostics.Tracing.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Dynamic.Runtime.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Globalization.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.IO.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Linq.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Linq.Expressions.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Linq.Parallel.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Linq.Queryable.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Net.NetworkInformation.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Net.Primitives.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Net.Requests.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.ObjectModel.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Reflection.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Reflection.Emit.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Reflection.Emit.ILGeneration.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Reflection.Emit.Lightweight.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Reflection.Extensions.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Reflection.Primitives.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Resources.ResourceManager.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Runtime.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Runtime.Extensions.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Runtime.InteropServices.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Runtime.InteropServices.WindowsRuntime.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Runtime.Numerics.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Runtime.Serialization.Json.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Runtime.Serialization.Primitives.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Runtime.Serialization.Xml.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Security.Principal.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.ServiceModel.Duplex.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.ServiceModel.Http.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.ServiceModel.NetTcp.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.ServiceModel.Primitives.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.ServiceModel.Security.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Text.Encoding.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Text.Encoding.Extensions.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Text.RegularExpressions.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Threading.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Threading.Tasks.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Threading.Tasks.Parallel.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Xml.ReaderWriter.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Xml.XDocument.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\Facades\\System.Xml.XmlSerializer.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\src\\FsToml\\bin\\Release\\FsToml.dll";
//     "Transform.fs"; "AssemblyInfo.fs"|]