System.IO.Directory.SetCurrentDirectory __SOURCE_DIRECTORY__

#r "../bin/FsToml.fsproj/Nett.dll"
#r "../bin/FsToml.fsproj/Forge.ProjectSystem.dll"
#r "../bin/FsToml.fsproj/FsToml.dll"
#r "../bin/FsToml.fsproj/FsToml.fsproj.dll"
#r "System.Xml"
#r "System.Xml.Linq"


let t = FsToml.Parser.parse "testproject.toml"
let p = FsToml.Transform.Fsproj.transform t
let f = p.ToXmlString()
f

//OUTPUT:

// <?xml version="1.0" encoding="utf-8" standalone="yes"?>
// <Project ToolsVersion="14.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
//   <PropertyGroup>
//     <Name>Library1</Name>
//     <AssemblyName>Library1</AssemblyName>
//     <RootNamespace>Library1</RootNamespace>
//     <SchemaVersion>2.0</SchemaVersion>
//     <ProjectGuid>bb0c6f01-5e57-4575-a498-5de850d9fa6c</ProjectGuid>
//     <OutputType>Library</OutputType>
//     <TargetFSharpCoreVersion>4.4.0.0</TargetFSharpCoreVersion>
//   </PropertyGroup>
//   <PropertyGroup Condition="">
//     <DebugSymbols>true</DebugSymbols>
//     <DebugType>Full</DebugType>
//     <Optimize>false</Optimize>
//     <OtherFlags>--warnon:1182</OtherFlags>
//   </PropertyGroup>
//   <PropertyGroup Condition="('$(TargetFrameworkIdentifier)' == '.NETCoreApp')">
//     <DebugSymbols>false</DebugSymbols>
//   </PropertyGroup>
//   <PropertyGroup Condition="('$(TargetFrameworkIdentifier)' == '.NETCoreApp') AND ('$(Configuration)' == 'Release')">
//     <DebugType>PdbOnly</DebugType>
//     <Optimize>true</Optimize>
//     <DefineConstants>RELEASE;FABLE</DefineConstants>
//   </PropertyGroup>
//   <PropertyGroup Condition="('$(TargetFrameworkIdentifier)' == '.NETCoreApp') AND ('$(TargetFrameworkVersion)' == 'v1.6') AND ('$(Platform)' == 'x86') AND ('$(Configuration)' == 'Release')">
//     <OutputPath>bin/Release/x86</OutputPath>
//   </PropertyGroup>
//   <PropertyGroup Condition="('$(TargetFrameworkIdentifier)' == '.NETCoreApp') AND ('$(TargetFrameworkVersion)' == 'v1.6') AND ('$(Platform)' == 'x64') AND ('$(Configuration)' == 'Release')">
//     <OutputPath>bin/Release/x64</OutputPath>
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
// </Project>