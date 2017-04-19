# fstoml [![Gitter](https://badges.gitter.im/fsprojects/fstoml.svg)](https://gitter.im/fsprojects/fstoml?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

FsToml is new, lighweight project file for F#.

**Warning! FsToml is experimental project, not recommended to use in production environment**

## Example

```toml
Name            = 'FantasticApp'
OutputType      = 'Exe'
FrameworkVersion = '4.5'

Files = [
    { Compile = "src/file.fs" },
    { Compile = "src/file2.fs", Link = "src/uselessLink.fs" },
    { Compile = "src/file3.fs", Sig = "src/file3.fsi" },
    { None    = "src/script.fsx", Private = true },
]

References = [
    { Reference = "System" },
    { Reference = "FSharp.Core" },
    { Project   = "Deppy.fstoml" },
    { Reference   = "lib/Fable.Core.dll", Private = true },
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

### Why?

* Sane syntax
* Removing all moving parts of normal MsBuild files that makes it imposible to parse without using MsBuild (conditional statements, variable assigments, functions calls inside of project file)
* Declarative way of defining conditional parts of project
* Deeper integration with Paket
* Better set of CLI tooling for project managment [which will make editor integration much easier]


## How does it work

`fstoml` is abstraction layer on top of MsBuild providing more declarative, high level way of defining projects. Under the hood it's translated to normal MsBuild XML `.props` file and imported by MsBuild whenever it's called. That makes `fstoml` 100% compatiable with all existing tooling, such as `dotnet` CLI and different editors.


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
