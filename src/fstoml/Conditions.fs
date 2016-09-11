module FsToml.Conditions

open System
open FsToml.ProjectSystem

let getPlatformCondition (platform : PlatformType) : string =
    sprintf "'$(Platform)' == '%s'" ^ platform.ToString()

let getFrameworkTargetCondition (framework : FrameworkTarget) : string =
    sprintf "'$(TargetFrameworkIdentifier)' == '%s'" ^ framework.ToString()

let getFrameworkVersionCondition (version : FrameworkVersion) : string =
    sprintf "'$(TargetFrameworkVersion)' == '%s'" ^ version.ToString()

let getBuildTypeCondition (buildType : BuildType) : string =
    sprintf "'$(Configuration)' == '%s'" ^ buildType.ToString()

let getCondition (condition : Condition) : string =
    let framework = condition.FrameworkTarget
    let version = condition.FrameworkVersion
    let platform = condition.PlatformType
    let build = condition.BuildType

    let fvp =
        match framework, version, platform with
        | Some f, Some v, Some p ->
            sprintf "(%s) AND (%s) AND (%s)" (getFrameworkTargetCondition f) (getFrameworkVersionCondition v) (getPlatformCondition p)
        | Some f, Some v, None ->
            sprintf "(%s) AND (%s)" (getFrameworkTargetCondition f) (getFrameworkVersionCondition v)
        | Some f, None, Some p ->
            sprintf "(%s) AND (%s)" (getFrameworkTargetCondition f) (getPlatformCondition p)
        | None , Some v, Some p ->
            sprintf "(%s) AND (%s)" (getFrameworkVersionCondition v) (getPlatformCondition p)
        | Some f, None, None ->
            sprintf "(%s)" (getFrameworkTargetCondition f)
        | None, Some v, None ->
            sprintf "(%s)" (getFrameworkVersionCondition v)
        | None, None, Some p ->
            sprintf "(%s)" (getPlatformCondition p)
        | None, None, None -> ""

    let b =
        match build with
        | Some b -> sprintf "(%s)" (getBuildTypeCondition b)
        | None  -> ""

    if fvp <> "" && b <> "" then fvp + " AND " + b else fvp + b

type ConditionParseResult =
    | FrameworkTarget of FrameworkTarget
    | FrameworkVersion of FrameworkVersion
    | PlatformType of PlatformType
    | BuildType of BuildType


let tryParseTarget = function
    | InvariantEqual "netcore" -> Some FrameworkTarget.NetcoreApp
    | InvariantEqual "net" -> Some FrameworkTarget.Net
    | InvariantEqual "netStandard" -> Some FrameworkTarget.NetStandard
    | _ ->  None

let tryParseVersion = function
    | InvariantEqual "1_0" -> Some FrameworkVersion.V1_0
    | InvariantEqual "1_1" -> Some FrameworkVersion.V1_1
    | InvariantEqual "1_2" -> Some FrameworkVersion.V1_2
    | InvariantEqual "1_3" -> Some FrameworkVersion.V1_3
    | InvariantEqual "1_4" -> Some FrameworkVersion.V1_4
    | InvariantEqual "1_5" -> Some FrameworkVersion.V1_5
    | InvariantEqual "1_6" -> Some FrameworkVersion.V1_6
    | InvariantEqual "4_5" -> Some FrameworkVersion.V4_5
    | InvariantEqual "4_5_1" -> Some FrameworkVersion.V4_5_1
    | InvariantEqual "4_6" -> Some FrameworkVersion.V4_6
    | InvariantEqual "4_6_1" -> Some FrameworkVersion.V4_6_1
    | InvariantEqual "4_6_2" -> Some FrameworkVersion.V4_6_2
    | _ -> None

let tryParsePlatform = function
    | InvariantEqual "AnyCPU" -> Some PlatformType.AnyCPU
    | InvariantEqual "x86" -> Some PlatformType.X86
    | InvariantEqual "x64" -> Some PlatformType.X64
    | _ -> None

let tryParseBuildType = function
    | InvariantEqual "Debug" -> Some BuildType.Debug
    | InvariantEqual "Release" -> Some BuildType.Release
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

