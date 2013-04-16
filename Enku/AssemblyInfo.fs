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

open System.Reflection
open System.Runtime.CompilerServices;

[<assembly:AssemblyDescription("Enku.dll")>]
[<assembly:AssemblyCompany("https://github.com/nakamura-to/Enku")>]
[<assembly:AssemblyTitle("Enku.dll")>]
[<assembly:AssemblyCopyright("Copyright 2013, Toshihiro Nakamura")>]
[<assembly:AssemblyProduct("Enku")>]
[<assembly:AssemblyVersion("0.0.0.3")>]

[<assembly:InternalsVisibleTo("Enku.Test")>]
do()
