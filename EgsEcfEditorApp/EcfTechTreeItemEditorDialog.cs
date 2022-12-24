using EcfFileViews;
using EgsEcfEditorApp.Properties;
using EgsEcfParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static EgsEcfEditorApp.EcfTechTreeDialog;

namespace EgsEcfEditorApp
{
    public partial class EcfTechTreeItemEditorDialog : Form
    {
        private EcfItemSelectorDialog ElementPicker { get; } = new EcfItemSelectorDialog();

        public EcfTechTreeItemEditorDialog()
        {
            InitializeComponent();
            InitForm();
        }

        // event
        private void InitForm()
        {
            Icon = IconRecources.Icon_AppBranding;
            Text = TitleRecources.EcfTechTreeDialog_ElementEditorHeader;
        }

        // public
        public DialogResult ShowDialog(IWin32Window parent, ElementNode sourceNode, List<EcfBlock> availableElements)
        {

            

            return ShowDialog(parent);
        }
    }
}
