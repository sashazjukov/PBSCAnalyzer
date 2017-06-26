using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer.Forms
{
    public partial class DebugDWContainer : DockContent
    {
        public DebugDWContainer()
        {
            InitializeComponent();
        }

        //List panelsList

        internal void ShowOrSwitchPanel(string file)
        {
            DebugDWPanel panel = null;
            // dockPanel1.
            DockPane orDefault = dockPanel1.Panes.FirstOrDefault(x => x.Appearance == DockPane.AppearanceStyle.Document);
            if (orDefault != null)
            {
                IDockContent firstOrDefault = orDefault.Contents.FirstOrDefault(s => (s is DebugDWPanel) && ((DebugDWPanel)s).file == file);
                if (firstOrDefault != null) { panel = firstOrDefault as DebugDWPanel; } else {
                    panel = new DebugDWPanel();
                    panel.file = file;
                    panel.CreateFileWatcher();
                }
            }                        
            panel.Show(dockPanel1);
        }
    }
}
