﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tsundoku.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AppResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AppResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Tsundoku.Resources.AppResources", typeof(AppResources).Assembly);
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
        ///   Looks up a localized string similar to Load offerings.
        /// </summary>
        internal static string LoadOfferings {
            get {
                return ResourceManager.GetString("LoadOfferings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Monthly subscription for.
        /// </summary>
        internal static string MonthlySubFor {
            get {
                return ResourceManager.GetString("MonthlySubFor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not subscribing is highly recomended.
        /// </summary>
        internal static string PayWallSubTitle {
            get {
                return ResourceManager.GetString("PayWallSubTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stack more than 10 books!.
        /// </summary>
        internal static string PayWallTitle {
            get {
                return ResourceManager.GetString("PayWallTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Subscription.
        /// </summary>
        internal static string Subscription {
            get {
                return ResourceManager.GetString("Subscription", resourceCulture);
            }
        }
    }
}
