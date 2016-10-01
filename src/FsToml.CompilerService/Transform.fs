module FsToml.Transform.CompilerService

open FsToml
open FsToml.ProjectSystem
open Microsoft.FSharp.Compiler.SourceCodeServices

let getName project =
    project.AssemblyName +
        match project.OutputType with
        | OutputType.Library -> ".dll"
        | _ -> ".exe"


module Configuration =
    open System.IO

    let getOutputPath cfg name =
         defaultArg (cfg.OutputPath |> Option.map (fun p -> Path.Combine(p,name))) (Path.Combine("bin", name))

    let getCompilerParams (target : Target.Target) (name : string) (cfg : Configuration) =

        let debug =
            if cfg.DebugSymbols |> Option.exists id then ":full"
            elif cfg.DebugType.IsSome then
                match cfg.DebugType.Value with
                | DebugType.None -> "-"
                | DebugType.Full -> ":full"
                | DebugType.PdbOnly -> ":pdbonly"
            else "-"

        let platofrm =
            if target.PlatformType = PlatformType.AnyCPU && cfg.Prefer32bit |> Option.exists id then
                "anycpu32bitpreferred"
            else
                target.PlatformType.ToString()

        let outPath = getOutputPath cfg name
        let xmlPath = defaultArg cfg.DocumentationFile (outPath + ".xml")

        [|
            yield "--tailcalls" + if cfg.Tailcalls |> Option.exists id then "+" else "-"
            yield "--warnaserror" + if cfg.WarningsAsErrors |> Option.exists id then "+" else "-"
            if target.FrameworkTarget = FrameworkTarget.NetcoreApp then yield "--targetprofile:netcore"
            if cfg.Constants.IsSome then
                for c in cfg.Constants.Value do
                    yield "-d:" + c
            yield "--debug" + debug
            yield "--optimize" + if cfg.Optimize |> Option.exists id then "+" else "-"
            yield "--platofrm:" + platofrm
            yield "--warn:" + string (defaultArg cfg.WarningLevel 3)
            yield "--out:" + outPath
            yield "--doc:" + xmlPath
            match cfg.NoWarn with
            | None | Some [||] -> ()
            | Some nowarns -> yield "--nowarn:" + (nowarns |> Seq.map (string) |> String.concat ",")
            match cfg.OtherFlags with
            | None | Some [||] -> ()
            | Some flags -> yield! flags
        |]

module References =
    open System
    open System.IO
    open System.Reflection

    let getPath ver nm =
         Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) +
         @"\Reference Assemblies\Microsoft\Framework\.NETFramework\" + ver + "\\" + nm + ".dll"

    let sysLib ver nm =
        if Environment.OSVersion.Platform = PlatformID.Win32NT then
            // file references only valid on Windows
            getPath ver nm
        else
            let sysDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
            let (++) a b = Path.Combine(a,b)
            sysDir ++ nm + ".dll"

    let fsCore ver =
        if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then
            // file references only valid on Windows
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) +
            @"\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\" + ver + @"\FSharp.Core.dll"
        else
            sysLib ver "FSharp.Core"

    let dependsOnFacade path  =
        let a = Assembly.LoadFile path
        a.GetReferencedAssemblies()
        |> Array.exists (fun an -> an.Name.Contains "System.Runtime")

    let getFacade ver =
        if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then
             // file references only valid on Windows
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) +
            @"\Reference Assemblies\Microsoft\Framework\.NETFramework\" + ver + "\\Facades\\"
            |> Directory.GetFiles
        else
            let sysDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
            let apiDir = if Directory.Exists (sysDir + "/Facades/") then sysDir + "/Facades/" else sysDir + "-api/Facades/"
            apiDir |> Directory.GetFiles

    let getPathToReference (target : Target.Target) (reference : Reference) : string[] =
        let ver = target.FrameworkVersion.ToString()
        if Path.IsPathRooted reference.Include then
            [| yield reference.Include;  |]
        elif File.Exists reference.Include then
            let p = Path.GetFullPath reference.Include
            [| yield p; |]
        elif target.FrameworkTarget = FrameworkTarget.Net then
            [| sysLib ver reference.Include |]
        else
            [||]

    let getCompilerParams (target : Target.Target) fsharpCore (refs : Reference[]) =
        let ver = target.FrameworkVersion.ToString()
        let isFullFramework = target.FrameworkTarget = FrameworkTarget.Net
        let references =
            refs
            |> Array.where (fun n -> n.IsPackage |> not)
            |> Array.map (getPathToReference target)
            |> Array.collect id
            |> Array.filter (fun r -> r.Contains "FSharp.Core" |> not && r.Contains "mscorlib" |> not)
            |> Array.distinct
        let packages =
            refs
            |> Array.where (fun n -> n.IsPackage)
            |> Array.map(fun n -> n.Include |> Package.getAssemblies target)
            |> Array.collect id
            |> Array.map(fun n -> if Path.IsPathRooted n then n else sysLib ver n)
        let allRefs = Array.concat [references; packages]
        let allRefs = if allRefs |> Array.exists (fun n -> isFullFramework && dependsOnFacade n) then Array.concat [allRefs; getFacade ver] else allRefs
        let allRefs = allRefs |> Array.distinctBy Path.GetFileName
        let hasFSharpCore = packages |> Array.exists (fun n -> n.EndsWith "FSharp.Core.dll")


        [|
            if isFullFramework then yield "-r:" + (sysLib  ver "mscorlib")
            if hasFSharpCore |> not && isFullFramework then yield "-r:" + (fsCore fsharpCore)
            for r in allRefs do
                yield "-r:" + r
        |]

module ProjectReferences =
    open System.IO

    let getTomlReference (target : Target.Target) (reference : ProjectReference) =
        let path = reference.Include |> Path.GetFullPath
        let proj = FsToml.Parser.parse path
        let config = proj.Configurations |> Target.getConfig target
        let name = getName proj
        Path.Combine(path |> Path.GetDirectoryName, Configuration.getOutputPath config name)

    let getFsprojReference (target : Target.Target) (reference : ProjectReference) =
        let path = reference.Include |> Path.GetFullPath
        let cfg = target.BuildType.ToString()
        let platform = target.PlatformType.ToString()
        let proj = ProjectCracker.GetProjectOptionsFromProjectFile(path, [("Configuration", cfg); ("Platform", platform) ])
        let out = proj.OtherOptions |> Array.pick (fun n -> if n.StartsWith "-o" || n.StartsWith "--o" then Some n else None)
        out.Replace("--out:", "").Replace("-o:", "")

    let getCompilerParams target (references : ProjectReference[]) =
        references |> Array.map (fun r -> "-r:" + if r.Include.EndsWith ".fstoml" then getTomlReference target r else getFsprojReference target r)




module Files =
    let getCompilerParams (files : SourceFile[]) =
        files
        |> Array.filter(fun r -> r.OnBuild = BuildAction.Compile)
        |> Array.map(fun r -> r.Link |> Option.fold (fun s e -> e) r.Include)

let getCompilerParams (target : Target.Target) ((path,project) : string * FsTomlProject) =
    let p = System.IO.Directory.GetCurrentDirectory ()
    System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName path)
    let name = getName project
    let cfg = project.Configurations |> Target.getConfig target |> Configuration.getCompilerParams target name
    let refs = project.References |> References.getCompilerParams target (project.FSharpCore.ToString())
    let files = project.Files |> Files.getCompilerParams
    let projRefs = project.ProjectReferences |> ProjectReferences.getCompilerParams target
    System.IO.Directory.SetCurrentDirectory p

    [|
        yield "--noframework"
        yield "--fullpaths"
        yield "--flaterrors"
        yield "--subsystemversion:6.00"
        yield "--highentropyva+"
        yield "--target:" + project.OutputType.ToString()
        yield! cfg
        yield! refs
        yield! projRefs
        yield! files

    |]


let getFSharpProjectOptions  (target : Target.Target) ((path,project) : string * FsTomlProject) =
    let parms = getCompilerParams target (path,project)
    let checker = FSharpChecker.Instance
    checker.GetProjectOptionsFromCommandLineArgs (project.Name, parms)




