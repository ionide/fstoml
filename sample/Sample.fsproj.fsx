System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__

#r "../bin/FsToml.fsproj/Nett.dll"
#r "../bin/FsToml.fsproj/Forge.ProjectSystem.dll"
#r "../bin/FsToml.fsproj/FsToml.dll"
#r "../bin/FsToml.fsproj/FsToml.fsproj.dll"
#r "System.Xml"
#r "System.Xml.Linq"

let t = FsToml.Parser.parse "testproject.toml"
let (p, references) = FsToml.Transform.Fsproj.transform t
let f = FsToml.Transform.Fsproj.toString "../../.paket/Paket.Restore.targets" p
f

//OUTPUT:
// <Project Sdk="FSharp.NET.Sdk;Microsoft.NET.Sdk">
//   <PropertyGroup>
//     <Name>Library1</Name>
//     <OutputType>Library</OutputType>
//   </PropertyGroup>
//   <PropertyGroup Condition="">
//     <DebugSymbols>true</DebugSymbols>
//     <DebugType>Full</DebugType>
//     <Optimize>false</Optimize>
//     <OtherFlags>--warnon:1182</OtherFlags>
//   </PropertyGroup>
//   <PropertyGroup Condition="('$(TargetFrameworkIdentifier)' == '.NETFramework')">
//     <DebugSymbols>false</DebugSymbols>
//   </PropertyGroup>
//   <PropertyGroup Condition="('$(TargetFrameworkIdentifier)' == '.NETFramework') AND ('$(Configuration)' == 'Release')">
//     <DebugType>PdbOnly</DebugType>
//     <Optimize>true</Optimize>
//     <DefineConstants>RELEASE;FABLE</DefineConstants>
//   </PropertyGroup>
//   <PropertyGroup Condition="('$(TargetFrameworkIdentifier)' == '.NETFramework') AND ('$(Platform)' == 'x86') AND ('$(Configuration)' == 'Release')">
//     <OutputPath>bin/Release/x86</OutputPath>
//   </PropertyGroup>
//   <PropertyGroup Condition="('$(TargetFrameworkIdentifier)' == '.NETFramework') AND ('$(Platform)' == 'x64') AND ('$(Configuration)' == 'Release')">
//     <OutputPath>bin/Release/x64</OutputPath>
//   </PropertyGroup>
//   <ItemGroup>
//     <Reference Include="System" />
//     <Reference Include="FSharp.Core" />
//     <Reference Include="lib/Fable.Core.dll">
//         <Private>False</Private>
//     </Reference>
//   </ItemGroup>
//   <ItemGroup>
//     <ProjectReference Include="Deppy.fsproj" />
//   </ItemGroup>
//   <ItemGroup>
//     <None Include="paket.references" />
//     <Compile Include="src/file.fs" />
//     <Compile Include="src/file2.fs">
//       <Link>src/uselessLink.fs</Link>
//     </Compile>
//     <Compile Include="src/file3.fs" />
//     <None Include="src/script.fsx" />
//   </ItemGroup>
//   <Import Project="../../.paket/Paket.Restore.targets" />
// </Project>