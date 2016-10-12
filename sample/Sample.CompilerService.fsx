// System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__

#I "../bin/FsToml.CompilerService"
#r "Chessie.dll"
#r "Mono.Cecil.dll"
#r "Mono.Cecil.Rocks.dll"
#r "Mono.Cecil.Pdb.dll"
#r "Mono.Cecil.Mdb.dll"
#r "Newtonsoft.Json.dll"
#r "Paket.Core.dll"
#r "System.Collections.Immutable.dll"
#r "System.Reflection.Metadata.dll"
#r "Nett.dll"
#r "FSharp.Compiler.Service.dll"
#r "FSharp.Compiler.Service.ProjectCracker.dll"
#r "FsToml.dll"
#r "FsToml.CompilerService.dll"
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

let path = System.IO.Path.Combine (__SOURCE_DIRECTORY__, "testproject.CompilerService.toml")
let t = FsToml.Parser.parse path
let p = FsToml.Transform.CompilerService.getCompilerParams target (path, t)
let p2 = FsToml.Transform.CompilerService.getFSharpProjectOptionsFromFile None path

printf "%A" p
printf "%A" p2


// [|"--noframework"; "--fullpaths"; "--flaterrors"; "--subsystemversion:6.00";
//     "--highentropyva+"; "--target:Library"; "--tailcalls+"; "--warnaserror-";
//     "-d:TRACE"; "--debug:pdbonly"; "--optimize+"; "--platofrm:AnyCPU";
//     "--warn:3"; "--out:bin\\Release\\FsToml.CompilerService.dll";
//     "--doc:bin\\Release\\FsToml.CompilerService.dll.xml";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\mscorlib.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.Core.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.Numerics.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.Xml.dll";
//     "-r:C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5\\System.Xml.Linq.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\fsharp.compiler.service\\lib\\net45\\FSharp.Compiler.Service.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\system.collections.immutable\\lib\\netstandard1.0\\System.Collections.Immutable.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\system.reflection.metadata\\lib\\netstandard1.1\\System.Reflection.Metadata.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\fsharp.compiler.service.projectcracker\\lib\\net45\\FSharp.Compiler.Service.ProjectCracker.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\paket.core\\lib\\net45\\Paket.Core.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\chessie\\lib\\net40\\Chessie.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\fsharp.core\\lib\\net40\\FSharp.Core.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\mono.cecil\\lib\\net45\\Mono.Cecil.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\mono.cecil\\lib\\net45\\Mono.Cecil.Rocks.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\mono.cecil\\lib\\net45\\Mono.Cecil.Pdb.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\mono.cecil\\lib\\net45\\Mono.Cecil.Mdb.dll";
//     "-r:d:\\Programowanie\\Projekty\\fstoml\\packages\\newtonsoft.json\\lib\\net45\\Newtonsoft.Json.dll";
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