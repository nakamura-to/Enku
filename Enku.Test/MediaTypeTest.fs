//----------------------------------------------------------------------------
//
// Copyright (c) 2013, Toshihiro Nakamura.
//
// This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
// copy of the license can be found in the LICENSE.txt file at the root of this distribution. 
// By using this source code in any fashion, you are agreeing to be bound 
// by the terms of the Apache License, Version 2.0.
//
// You must not remove this notice, or any other, from this software.
//----------------------------------------------------------------------------

namespace Enku.Test

open NUnit.Framework
open System.IO
open System.Net.Http
open Enku

module MediaTypeTest =

  [<Test>]
  let ``TextMediaTypeFormatter should write a string value as string``() =
    let formatter = new TextMediaTypeFormatter()
    let value = "あいう"
    use writeStream = new MemoryStream()
    use content = new ObjectContent(value.GetType(), value, formatter)
    let task = formatter.WriteToStreamAsync(content.ObjectType, content.Value, writeStream, content, null)
    task.Wait()
    writeStream.Position <- 0L
    use reader = new StreamReader(writeStream)
    let x = reader.ReadToEnd()
    x |> isEqualTo "あいう"

  [<Test>]
  let ``TextMediaTypeFormatter should write a stream value as string``() =
    let formatter = new TextMediaTypeFormatter()
    use value = new MemoryStream()
    use stringWriter = new StreamWriter(value)
    stringWriter.Write("あいう")
    stringWriter.Flush()
    value.Position <- 0L
    use writeStream = new MemoryStream()
    use content = new ObjectContent(value.GetType(), value, formatter)
    let task = formatter.WriteToStreamAsync(content.ObjectType, content.Value, writeStream, content, null)
    task.Wait()
    writeStream.Position <- 0L
    use reader = new StreamReader(writeStream)
    let x = reader.ReadToEnd()
    x |> isEqualTo "あいう"