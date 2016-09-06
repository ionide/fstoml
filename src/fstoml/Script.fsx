System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__

#r "bin/release/Nett.dll"
#r "bin/release/Forge.ProjectSystem.dll"
#r "bin/release/fstoml.dll"
#r "System.Xml"
#r "System.Xml.Linq"


let t = FsToml.Parser.parse "testproject.toml"
let p = FsToml.Transform.transform t
let f = p.ToXmlString()
f

//OUTPUT:

// "<?xml version="1.0" encoding="utf-8" standalone="yes"?>
// <Project ToolsVersion="14.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
//   <PropertyGroup>
//     <Name>Library1</Name>
//     <AssemblyName>Library1</AssemblyName>
//     <RootNamespace>Library1</RootNamespace>
//     <SchemaVersion>2.0</SchemaVersion>
//     <ProjectGuid>bb0c6f01-5e57-4575-a498-5de850d9fa6c</ProjectGuid>
//     <OutputType>Library</OutputType>
//     <TargetFSharpCoreVersion>FsToml.ProjectSystem+FSharpVer</TargetFSharpCoreVersion>
//   </PropertyGroup>
//   <PropertyGroup Condition="">
//     <DebugSymbols>true</DebugSymbols>
//     <DebugType>Full</DebugType>
//     <Optimize>false</Optimize>
//     <Tailcalls>false</Tailcalls>
//     <OutputPath></OutputPath>
//     <DefineConstants></DefineConstants>
//     <WarningLevel>3</WarningLevel>
//     <PlatformTarget>AnyCPU</PlatformTarget>
//     <Prefer32Bit>false</Prefer32Bit>
//     <OtherFlags>--warnon:1182</OtherFlags>
//   </PropertyGroup>
//   <ItemGroup>
//     <Reference Include="System" />
//     <Reference Include="FSharp.Core" />
//     <Reference Include="Fable.Core">
//       <Private>False</Private>
//     </Reference>
//   </ItemGroup>
//   <ItemGroup>
//     <ProjectReference Include="">
//       <Name>Deppy</Name>
//       <Project>{f3d0b372-3af7-49d9-98ed-5a78e9416098}</Project>
//       <Private>False</Private>
//     </ProjectReference>
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
// </Project>"