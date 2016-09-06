module FsToml.Conditions

open System
open FsToml.ProjectSystem

let getPlatformCondition (platform : PlatformType) : string =
    sprintf "'$(Platform)' == '%s'" ^ platform.ToString()

let getFrameworkTargetCondition (framework : FrameworkTarget) : string =
    sprintf "'$(TargetFrameworkIdentifier)' == '%s'" ^ framework.ToString()

let getFrameworkVersionCondition (version : FrameworkVersion) : string =
    sprintf "'$(TargetFrameworkVersion)' == '%s'" ^ version.ToString()

let getCondition (framework : FrameworkTarget option) (version : FrameworkVersion option) (platform : PlatformType option) : string =
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
        sprintf "%s" (getFrameworkTargetCondition f)
    | None, Some v, None ->
        sprintf "%s" (getFrameworkVersionCondition v)
    | None, None, Some p ->
        sprintf "%s" (getPlatformCondition p)
    | None, None, None -> ""



