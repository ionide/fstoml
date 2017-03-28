System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__

#r "../bin/FsToml.fsproj/Nett.dll"
#r "../bin/FsToml.fsproj/Forge.ProjectSystem.dll"
#r "../bin/FsToml.fsproj/FsToml.dll"
#r "../bin/FsToml.fsproj/FsToml.fsproj.dll"
#r "System.Xml"
#r "System.Xml.Linq"

let t = FsToml.Parser.parse "testproject.toml"
let (p, references) = FsToml.Transform.Fsproj.transform t
