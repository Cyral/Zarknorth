#region Usings
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ZarknorthClient; 
#endregion

#region Assembly Attributes
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Zarknorth")]
[assembly: AssemblyProduct("Zarknorth")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Zarknorth Team")]
[assembly: AssemblyCopyright("Copyright © 2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
#if DEBUG
[assembly: VersionText("Dev")]
[assembly: WarningText("")]
#else
[assembly: VersionText("Alpha (Source Code Release)")]
[assembly: WarningText("")]
#endif
// Stops common IL dissasembler
[assembly: SuppressIldasm]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type. Only Windows
// assemblies support COM.
[assembly: ComVisible(false)]

// On Windows, the following GUID is for the ID of the typelib if this
// project is exposed to COM. On other platforms, it unique identifies the
// title storage container when deploying this assembly to the device.
[assembly: Guid("b42c3a36-38f7-448b-a50d-b2c59f680e49")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("0.3.*")] 
#endregion

#region Custom Assembly Attributes
[AttributeUsage(AttributeTargets.Assembly)]
public class WarningText : Attribute
{
    public string Text;
    public WarningText() : this(String.Empty) { }
    public WarningText(string text) { Text = text; }
}
[AttributeUsage(AttributeTargets.Assembly)]
public class VersionText : Attribute
{
    public string Text;
    public VersionText() : this(String.Empty) { }
    public VersionText(string text) { Text = text; }
}
#endregion
