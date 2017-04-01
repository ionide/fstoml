[<RequireQualifiedAccess>]
module FsToml.Constants

// Common Constants
let [<Literal>] Name        = "Name"
let [<Literal>] None        = "None"
let [<Literal>] Reference   = "Reference"

// Platform Constants

let [<Literal>] X86       = "x86"
let [<Literal>] X64       = "x64"
let [<Literal>] AnyCPU    = "AnyCPU"

// BuildAction Constants

let [<Literal>] Compile          = "Compile"
let [<Literal>] Content          = "Content"
let [<Literal>] Resource         = "Resource"
let [<Literal>] EmbeddedResource = "EmbeddedResource"

// CopyToOutputDirectory Constants
let [<Literal>] Never             = "Never"
let [<Literal>] Always            = "Always"
let [<Literal>] PreserveNewest    = "PreserveNewest"

// DebugType Constants

let [<Literal>] PdbOnly   = "PdbOnly"
let [<Literal>] Full      = "Full"

// BuildType Constants
let [<Literal>] Debug   = "Debug"
let [<Literal>] Release = "Release"


// OutputType Constants
let [<Literal>] Exe       = "Exe"
let [<Literal>] Winexe    = "Winexe"
let [<Literal>] Library   = "Library"
let [<Literal>] Module    = "Module"

//FrameworkTarget Constants
let [<Literal>] NetCore       = "netCore"
let [<Literal>] Net           = "net"
let [<Literal>] NetStandard   = "netStandard"

//FrameworkVersion Constants
let [<Literal>] ``1_0``   = "1_0"
let [<Literal>] ``1_1``   = "1_1"
let [<Literal>] ``1_2``   = "1_2"
let [<Literal>] ``1_3``   = "1_3"
let [<Literal>] ``1_4``   = "1_4"
let [<Literal>] ``1_5``   = "1_5"
let [<Literal>] ``1_6``   = "1_6"
let [<Literal>] ``4_5``   = "4_5"
let [<Literal>] ``4_5_1`` = "4_5_1"
let [<Literal>] ``4_5_2`` = "4_5_2"
let [<Literal>] ``4_6``   = "4_6"
let [<Literal>] ``4_6_1`` = "4_6_1"