# fstoml [![Gitter](https://badges.gitter.im/fsprojects/fstoml.svg)](https://gitter.im/fsprojects/fstoml?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

FsToml is new, lighweight project file for F#.

**Warning! FsToml is experimental project, not recommended to use in production environment**


## Example

```toml
FsTomlVersion   = '0.0.1'
Name            = 'FantasticApp'
Guid            = 'bb0c6f01-5e57-4575-a498-5de850d9fa6c'
OutputType      = 'Exe'
FSharpCore      = '4.4.0.0'


Files = [
    { Compile = "src/file.fs" },
    { Compile = "src/file2.fs", Link = "src/uselessLink.fs" },
    { Compile = "src/file3.fs", Sig = "src/file3.fsi" },
    { None    = "src/script.fsx", Private = true },
]

References = [
    { Framework = "System" },
    { Framework = "FSharp.Core" },
    { Project   = "Deppy.fstoml" },
    { Library   = "lib/Fable.Core.dll", Private = true },
    { Package   = "Nett" }
]

DebugSymbols = true
DebugType = 'full'
Optimize = false
NoWarn = [52, 40]
OtherFlags = [ '--warnon:1182' ]

[ net ]
   DebugSymbols = false

[ net.Release ]
    Constants = [ 'RELEASE', 'FABLE' ]
    DebugType = 'pdbonly'
    Optimize = true

[ net."4_5".Release.x86 ]
    OutputPath = "bin/Release/x86"

[ net."4_5".Release.x64 ]
    OutputPath = "bin/Release/x64"
```

## Aim of the project

We want:

* Have lightweight, human readable, easy to parse project file for F#
* Use fully declarative style - no custom programming language inside of project file
* Support popular types of F# applications
* Provide easy migration path to `fsproj` in case user needs more advanced features
* Provide integrated Paket support
* Support .Net 4.5+, .Net Standard, .Net Core

We won't :

* Support all MsBuild features - no targets, no custom extensions etc.
* Provide support for application types requireing more advanced MsBuild features (like Xamarin, Azure Service Fabric etc)
* Support every possible framework target - only .Net 4.5+, .Net Standard and .Net Core

## What's working

* Project file parser based on [0.0.1 specification](spec/sprc-0.0.1.toml)
* Support for .Net 4.5+ (.Net Standard and .Net Core are still **not** supported)
* Internal project model
* Mapping internal project model to FSharpProjectOptions, what allows integretation with FCS based tooling.
* Generating equivalent `fsproj` file
* Compiling `fstoml` project without using MsBuild
* CLI for compiling and generating `fsproj`


## Contributing and copyright

The project is hosted on [GitHub](https://github.com/fsprojects/fstoml) where you can [report issues](https://github.com/fsprojects/fstoml/issues), fork
the project and submit pull requests.

The library is available under [MIT license](https://github.com/fsprojects/fstoml/blob/master/LICENSE.md), which allows modification and redistribution for both commercial and non-commercial purposes.

## Maintainer(s)

* Alfonso Garcia-Caro [@alfonsogarciacaro](https://github.com/alfonsogarciacaro)
* Jared Hester [@cloudRoutine](https://github.com/cloudRoutine)
* Krzysztof Cieślak [@Krzysztof-Cieslak](https://github.com/Krzysztof-Cieslak)
* Matthias Dittrich [@matthid](https://github.com/matthid)
* Steffen Forkmann [@forki](https://github.com/forki)


The default maintainer account for projects under "fsprojects" is [@fsprojectsgit](https://github.com/fsprojectsgit) - F# Community Project Incubation Space (repo management)
