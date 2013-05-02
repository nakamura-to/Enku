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

namespace Enku

open System
open System.IO
open System.Net
open System.Net.Http
open System.Net.Http.Formatting
open System.Net.Http.Headers
open System.Text

type TextMediaTypeFormatter() =
  inherit MediaTypeFormatter()
  do
    base.SupportedMediaTypes.Add(MediaTypeHeaderValue("text/plain"))
    base.SupportedMediaTypes.Add(MediaTypeHeaderValue("text/html"))
    base.SupportedEncodings.Add(UTF8Encoding(encoderShouldEmitUTF8Identifier = false, throwOnInvalidBytes = true))
    base.SupportedEncodings.Add(UnicodeEncoding(bigEndian = false, byteOrderMark = true, throwOnInvalidBytes = true))
  override this.CanReadType(typ) = false
  override this.CanWriteType(typ) = true
  override this.WriteToStreamAsync(typ: Type, value: obj, writeStream: Stream, content: HttpContent, transportContext: TransportContext) = 
    match value with
    | :? Stream as stream ->
      using stream <| fun readStream -> readStream.CopyToAsync(writeStream)
    | _ ->
      let encoding = base.SelectCharacterEncoding(content.Headers)
      use writer = new StreamWriter(writeStream, encoding, 1024, leaveOpen = true)
      writer.WriteAsync(string value)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module MediaType =

  let private textMediaTypeFormatter = TextMediaTypeFormatter()

  type T =
    /// content negotiation
    | Neg
    /// media type
    | Name of string
    /// media type formatter
    | Formatter of MediaTypeFormatter

  /// application/json media type
  let json = Name "application/json"

  /// application/xml media type
  let xml = Name "application/xml"

  /// text/html
  let html = Formatter textMediaTypeFormatter

  /// text/plain
  let plain = Formatter textMediaTypeFormatter