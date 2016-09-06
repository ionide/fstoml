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
    sprintf "'$(Configuration)' =='%s'" ^ buildType.ToString()

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


