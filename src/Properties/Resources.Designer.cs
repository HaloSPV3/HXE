﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HXE.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HXE.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  _    ___   ________
        ///| |  | \ \ / /  ____|
        ///| |__| |\ V /| |__
        ///|  __  | &gt; &lt; |  __|
        ///| |  | |/ . \| |____
        ///|_|  |_/_/ \_\______| :: Halo XE
        ///=================================
        ///A HCE wrapper and kernel for SPV3
        ///---------------------------------
        ///:: https://github.com/HaloSPV3/hxe
        ///---------------------------------
        ///
        ///HXE can be invoked with the following arguments:
        ///
        ///      --config               Opens configuration GUI
        ///      --positions            Opens positions GUI
        ///      --load                 Initiat [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Banner {
            get {
                return ResourceManager.GetString("Banner", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This binary has been compiled using build-{0}.
        /// </summary>
        internal static string BannerBuildNumber {
            get {
                return ResourceManager.GetString("BannerBuildNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com/HaloSPV3/HXE/tag/build-{0}.
        /// </summary>
        internal static string BannerBuildSource {
            get {
                return ResourceManager.GetString("BannerBuildSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Patches for Halo CE v1.0.10
        ///;orig SHA256: FEEA46FCE285EC071016CF5534ABE47ECF36F6CFAC8F1973EE6919851EA5A037
        ///
        ///Make large address aware
        ///haloce.exe
        ///;-----------------------
        ///;Normally 32-bit applications can only use up to 2GB of RAM. This patch increases that limit to 4GB.
        ///;It is required for some maps and mods.
        ///;
        ///;Set the IMAGE_FILE_LARGE_ADDRESS_AWARE characteristic flag in the PE header
        ///00000136: 0F 2F
        ///
        ///Remove DRM and key checks
        ///haloce.exe
        ///;------------------------
        ///;Allows playing and hosting  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string patches {
            get {
                return ResourceManager.GetString("patches", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ..
        /// </summary>
        internal static string Progress {
            get {
                return ResourceManager.GetString("Progress", resourceCulture);
            }
        }
    }
}
