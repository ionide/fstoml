module FsToml.Conditions

open System
open FsToml.ProjectSystem


type ConditionParseResult =
    | FrameworkTarget of FrameworkTarget
    | FrameworkVersion of FrameworkVersion
    | PlatformType of PlatformType
    | BuildType of BuildType


let tryParseTarget = function
    | InvariantEqual Constants.NetCore     -> Some FrameworkTarget.NetcoreApp
    | InvariantEqual Constants.Net         -> Some FrameworkTarget.Net
    | InvariantEqual Constants.NetStandard -> Some FrameworkTarget.NetStandard
    | _ ->  None

let tryParseVersion = function
    | InvariantEqual Constants.``1_0``   -> Some FrameworkVersion.V1_0
    | InvariantEqual Constants.``1_1``   -> Some FrameworkVersion.V1_1
    | InvariantEqual Constants.``1_2``   -> Some FrameworkVersion.V1_2
    | InvariantEqual Constants.``1_3``   -> Some FrameworkVersion.V1_3
    | InvariantEqual Constants.``1_4``   -> Some FrameworkVersion.V1_4
    | InvariantEqual Constants.``1_5``   -> Some FrameworkVersion.V1_5
    | InvariantEqual Constants.``1_6``   -> Some FrameworkVersion.V1_6
    | InvariantEqual Constants.``4_5``   -> Some FrameworkVersion.V4_5
    | InvariantEqual Constants.``4_5_1`` -> Some FrameworkVersion.V4_5_1
    | InvariantEqual Constants.``4_5_2`` -> Some FrameworkVersion.V4_6
    | InvariantEqual Constants.``4_6``   -> Some FrameworkVersion.V4_6_1
    | InvariantEqual Constants.``4_6_1`` -> Some FrameworkVersion.V4_6_2
    | _ -> None

let tryParsePlatform = function
    | InvariantEqual Constants.AnyCPU -> Some PlatformType.AnyCPU
    | InvariantEqual Constants.X86    -> Some PlatformType.X86
    | InvariantEqual Constants.X64    -> Some PlatformType.X64
    | _ -> None

let tryParseBuildType = function
    | InvariantEqual Constants.Debug  -> Some BuildType.Debug
    | InvariantEqual Constants.Release -> Some BuildType.Release
    | _ -> None


let private parsePart (cond : string) : ConditionParseResult option =
    match tryParseTarget cond, tryParseVersion cond, tryParsePlatform cond, tryParseBuildType cond with
    | Some c, _,_,_ ->  FrameworkTarget c |> Some
    | _, Some c, _, _ -> FrameworkVersion c |> Some
    | _, _, Some c, _ -> PlatformType c |> Some
    | _,_,_, Some c -> BuildType c |> Some
    | _ -> None

let apply condition parseResult  =
    match parseResult with
    | FrameworkTarget c -> {condition with FrameworkTarget = Some c}
    | FrameworkVersion c -> {condition with FrameworkVersion = Some c}
    | PlatformType c -> {condition with PlatformType = Some c}
    | BuildType c -> {condition with BuildType = Some c}

let parseTomlCondition (condition : string) : Condition =
    let results =
        if condition.Contains "." |> not then
            parsePart condition |> Array.singleton
        else
            condition.Split '.' |> Array.map parsePart

    let empty =
        {
            FrameworkTarget = None
            FrameworkVersion = None
            PlatformType = None
            BuildType = None
        }
    results
    |> Array.choose id
    |> Array.fold apply empty

