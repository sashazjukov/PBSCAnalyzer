using System;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer
{
    public static class MyDockHelper
    {
        public static void MakePanelVisible(DockContent dockContent)
        {
            DockState a = dockContent.DockState;
            DockState toState = a;
            switch (a)
            {
                case DockState.Unknown:
                    toState = DockState.DockBottom;
                    break;
                case DockState.Float:
                    break;
                case DockState.DockTopAutoHide:
                    toState = DockState.DockTop;
                    break;
                case DockState.DockLeftAutoHide:
                    toState = DockState.DockLeft;
                    break;
                case DockState.DockBottomAutoHide:
                    toState = DockState.DockBottom;
                    break;
                case DockState.DockRightAutoHide:
                    toState = DockState.DockRight;
                    break;
                case DockState.Document:
                    break;
                case DockState.DockTop:
                    break;
                case DockState.DockLeft:
                    break;
                case DockState.DockBottom:
                    break;
                case DockState.DockRight:
                    break;
                case DockState.Hidden:
                    toState = DockState.DockBottom;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            dockContent.DockState = toState;
        }        
    }
}