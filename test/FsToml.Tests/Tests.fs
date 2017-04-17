module Tests

open Expecto
open FsToml.Conditions
open FsToml.ProjectSystem

[<Tests>]
let tests =
  testList "conditions" [
    testList "parse Toml Conditions" [
      testCase "simple Framework Target - .Net" <| fun _ ->
        let t = "net" |> parseTomlCondition
        Expect.equal t {
            FrameworkTarget = Some FrameworkTarget.Net
            FrameworkVersion = None
            PlatformType = None
            BuildType = None } "simple framework target  - .Net"

      testCase "simple Framework Target - NetcoreApp" <| fun _ ->
        let t = "netCore" |> parseTomlCondition
        Expect.equal t {
            FrameworkTarget = Some FrameworkTarget.NetcoreApp
            FrameworkVersion = None
            PlatformType = None
            BuildType = None } "simple framework target  - NetcoreApp"

      testCase "simple Framework Target - NetStandard" <| fun _ ->
        let t = "netStandard" |> parseTomlCondition
        Expect.equal t {
            FrameworkTarget = Some FrameworkTarget.NetStandard
            FrameworkVersion = None
            PlatformType = None
            BuildType = None } "simple framework target  - NetStandard"
    ]
  ]