﻿#pragma checksum "..\..\..\Diplôme\Bordereau.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "34AFC1710446AC970B692678484030647D8555795D8C08B5495ED098E3EE26C3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using G3D;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using Microsoft.Reporting.WinForms;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace G3D.Diplôme {
    
    
    /// <summary>
    /// Bordereau
    /// </summary>
    public partial class Bordereau : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\..\Diplôme\Bordereau.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.AutoCompleteBox FilièreGroupeText;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\Diplôme\Bordereau.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker DateEnvoyé;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Diplôme\Bordereau.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Reporting.WinForms.ReportViewer _ReportViewer;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/G3D;component/dipl%c3%b4me/bordereau.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Diplôme\Bordereau.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.FilièreGroupeText = ((System.Windows.Controls.AutoCompleteBox)(target));
            return;
            case 2:
            this.DateEnvoyé = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 3:
            
            #line 35 "..\..\..\Diplôme\Bordereau.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BordereauClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this._ReportViewer = ((Microsoft.Reporting.WinForms.ReportViewer)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

