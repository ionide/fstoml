module Package

open System.IO
open FsToml.Prelude
open FsToml.ProjectSystem
open FsToml.Target
open Paket.Domain


let rec findDependencyFile folder =
    let deps = "paket.dependencies"
    try
        let folder = Path.GetFullPath folder
        let file = folder </> deps
        if File.Exists file then
            Some file
        else
            let dir = Directory.GetParent folder
            findDependencyFile dir.FullName
    with
    | _ -> None

let getAssemblies (target : Target) (name : string) =

    match findDependencyFile ^ Directory.GetCurrentDirectory () with
    | None -> [||]
    | Some p ->
        let dependenciesFile, lockFile =
            let deps = Paket.Dependencies.Locate p
            let lock =
                deps.DependenciesFile
                |> Paket.DependenciesFile.ReadFromFile
                |> fun f -> f.FindLockfile().FullName
                |> Paket.LockFile.LoadFrom
            deps, lock
        let frmwrk =
            match target.FrameworkTarget with
            | FrameworkTarget.Net ->
                let version =
                    match target.FrameworkVersion with
                    | V4_5 -> Paket.FrameworkVersion.V4_5
                    | V4_5_1 -> Paket.FrameworkVersion.V4_5_1
                    | V4_6 -> Paket.FrameworkVersion.V4_6
                    | V4_6_1 -> Paket.FrameworkVersion.V4_6_1
                    | V4_6_2 -> Paket.FrameworkVersion.V4_6_2
                Paket.FrameworkIdentifier.DotNetFramework version
            | FrameworkTarget.NetcoreApp ->
                let version =
                    match target.FrameworkVersion with
                    | V1_0 -> Paket.DotNetCoreVersion.V1_0

                Paket.FrameworkIdentifier.DotNetCore version
            | FrameworkTarget.NetStandard ->
                let version =
                    match target.FrameworkVersion with
                    | V1 -> Paket.DotNetStandardVersion.V1_0
                    | V1_1 -> Paket.DotNetStandardVersion.V1_1
                    | V1_2 -> Paket.DotNetStandardVersion.V1_2
                    | V1_3 -> Paket.DotNetStandardVersion.V1_3
                    | V1_4 -> Paket.DotNetStandardVersion.V1_4
                    | V1_5 -> Paket.DotNetStandardVersion.V1_5
                    | V1_6 -> Paket.DotNetStandardVersion.V1_6

                Paket.FrameworkIdentifier.DotNetStandard version

        let deps = lockFile.GetAllDependenciesOf (GroupName "main", PackageName name)
        deps.Add (PackageName name) |> ignore


        deps
        |> Seq.collect (fun n ->
            let a = dependenciesFile.GetInstalledPackageModel (Some "main", n.GetCompareString())
            let dlls =
                Paket.LoadingScripts.PackageAndAssemblyResolution.getDllsWithinPackage frmwrk a
                |> List.map (fun f -> f.FullName)
            let asmbls = Paket.LoadingScripts.PackageAndAssemblyResolution.getFrameworkReferencesWithinPackage a
            List.concat [dlls; asmbls]
        )
        |> Seq.toArray


