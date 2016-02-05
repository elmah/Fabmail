#region Copyright (c) 2015 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Elmah.Fabmail")]
[assembly: AssemblyDescription("Fabulous Mail for ELMAH")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ELMAH")]
[assembly: AssemblyCopyright("Copyright \u00a9 2015, Atif Aziz.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("76c202f3-a222-4a8f-a3ef-44c79e8bb7c6")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif
