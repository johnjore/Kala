using Android.App;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Kala.Droid")]
[assembly: AssemblyDescription("Client for OpenHAB 2.x")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Virtually Nothing Ltd")]
[assembly: AssemblyProduct("Kala.Droid")]
[assembly: AssemblyCopyright("Copyright © 2016-2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.*")]

#if DEBUG
[assembly: Application(Debuggable=true)]
#else
[assembly: Application(Debuggable = false)]
#endif
