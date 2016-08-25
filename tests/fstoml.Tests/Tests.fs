module fstoml.Tests

open FsToml
open NUnit.Framework

[<Test>]
let ``hello returns 42`` () =
  Assert.AreEqual(42,42)
