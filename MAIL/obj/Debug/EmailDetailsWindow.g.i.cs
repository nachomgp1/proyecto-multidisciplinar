﻿#pragma checksum "..\..\EmailDetailsWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9476E74DD5C26AFBEAB6A3629F278E5E7AC85D41A2156ACEACE6A04817116D69"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace MAIL {
    
    
    /// <summary>
    /// EmailDetailsWindow
    /// </summary>
    public partial class EmailDetailsWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\EmailDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image loadingIndicator;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\EmailDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel emailDetailsPanel;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\EmailDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock subjectTextBlock;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\EmailDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock fromTextBlock;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\EmailDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock toTextBlock;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\EmailDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock dateTextBlock;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\EmailDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox bodyTextBox;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\EmailDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border attachmentsSection;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\EmailDetailsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel attachmentsStackPanel;
        
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
            System.Uri resourceLocater = new System.Uri("/MAIL;component/emaildetailswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\EmailDetailsWindow.xaml"
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
            this.loadingIndicator = ((System.Windows.Controls.Image)(target));
            return;
            case 2:
            this.emailDetailsPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 3:
            this.subjectTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.fromTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.toTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.dateTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.bodyTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.attachmentsSection = ((System.Windows.Controls.Border)(target));
            return;
            case 9:
            this.attachmentsStackPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 10:
            
            #line 76 "..\..\EmailDetailsWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CloseButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

