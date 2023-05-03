using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace EcfToolBarControls
{
    public class EcfToolContainer : FlowLayoutPanel
    {
        public EcfToolContainer()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Margin = new Padding(Margin.Left, 0, Margin.Right, 0);
        }

        public void Add(EcfToolBox toolGroup)
        {
            Controls.Add(toolGroup);
        }
    }
    public abstract class EcfToolBox : FlowLayoutPanel
    {
        public EcfToolBox() : base()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Margin = new Padding(Margin.Left, 0, Margin.Right, 0);
        }
        protected Control Add(Control control)
        {
            control.AutoSize = true;
            control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            control.Dock = DockStyle.Fill;
            Controls.Add(control);
            return control;
        }
    }
    public class EcfToolBarCheckComboBox : Panel
    {
        public event EventHandler SelectionChangeCommitted;

        private bool IsResultBoxUnderCursor { get; set; } = false;

        private ToolTip Tip { get; } = new ToolTip();
        private TextBox ResultBox { get; } = new TextBox();
        private DropDownButton DropButton { get; }
        private DropDownList ItemList { get; }

        public string ToolTipText { get; set; }
        public string OfText { get; set; }
        public string NameText { get; set; }
        public string ChangeAllText { 
            get 
            {
                return ItemList.ChangeAllText;
            }
            set
            {
                ItemList.ChangeAllText = value;
            }
        }
        public int MaxDropDownItems { get; set; } = 10;

        public EcfToolBarCheckComboBox()
        {
            ResultBox.ReadOnly = true;
            ResultBox.Margin = new Padding(0);
            ResultBox.Location = new Point(0, 0);

            DropButton = new DropDownButton(ResultBox.Height);

            ItemList = new DropDownList()
            {
                AnchorControl = this
            };

            ResultBox.MouseHover += ResultBox_MouseHover;
            ResultBox.Click += ResultBox_Click;
            ResultBox.MouseEnter += ResultBox_MouseEnter;
            ResultBox.MouseLeave += ResultBox_MouseLeave;
            DropButton.DropStateChanged += DropButton_DropStateChanged;
            ItemList.ItemStateChanged += ItemList_ItemStateChanged;
            ItemList.DropDownFocusLost += ItemList_DropDownFocusLost;

            Controls.Add(ResultBox);
            Controls.Add(DropButton);
        }

        // events
        private void ResultBox_MouseHover(object sender, EventArgs evt)
        {
            Tip.SetToolTip(ResultBox, ToolTipText);
        }
        private void ResultBox_Click(object sender, EventArgs evt)
        {
            DropButton.Switch();
        }
        private void ResultBox_MouseEnter(object sender, EventArgs evt)
        {
            IsResultBoxUnderCursor = true;
        }
        private void ResultBox_MouseLeave(object sender, EventArgs evt)
        {
            IsResultBoxUnderCursor = false;
        }
        private void ItemList_DropDownFocusLost(object sender, EventArgs evt)
        {
            if (!DropButton.IsUnderCursor && !IsResultBoxUnderCursor)
            {
                DropButton.Reset();
            }
        }
        private void DropButton_DropStateChanged(object sender, EventArgs evt)
        {
            if (DropButton.State == ComboBoxState.Pressed)
            {
                ItemList.ShowPopup(this);
            }
            else
            {
                ItemList.Hide();
                SelectionChangeCommitted?.Invoke(this, null);
            }
        }
        private void ItemList_ItemStateChanged(object sender, ItemCheckEventArgs evt)
        {
            UpdateResult();
        }

        // private
        private void UpdateResult()
        {
            ResultBox.Text = ToString();
            int width = TextRenderer.MeasureText(ResultBox.Text, ResultBox.Font).Width;
            if (width > ResultBox.Width)
            {
                ResultBox.Width = width;
                DropButton.Location = new Point(width, 0);
                Size = new Size(width + DropButton.Width, ResultBox.Height);
                MinimumSize = Size;
                MaximumSize = Size;
            }
        }
        protected override void Dispose(bool disposing)
        {
            Tip.Dispose();
            ItemList.Dispose();
            base.Dispose(disposing);
        }

        // public
        public void Reset()
        {
            ItemList.Reset();
        }
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", GetCheckedItems().Count, OfText, GetItems().Count, NameText);
        }
        public string GetResult()
        {
            return ResultBox.Text;
        }
        public List<CheckableItem> GetItems()
        {
            return ItemList.GetItems();
        }
        public List<CheckableItem> GetCheckedItems()
        {
            return ItemList.GetCheckedItems();
        }
        public List<CheckableItem> GetUncheckedItems()
        {
            return ItemList.GetUncheckedItems();
        }
        public void SetItems(List<CheckableItem> items)
        {
            if (items != null)
            {
                ItemList.SetItems(items);
                UpdateResult();
            }
        }
        public bool IsItemChecked(string itemId)
        {
            return ItemList.GetCheckedItems().Any(item => item.Id.Equals(itemId));
        }

        private class DropDownList : Form
        {
            public event ItemCheckEventHandler ItemStateChanged;
            public event EventHandler DropDownFocusLost;

            public Control AnchorControl { get; set; } = null;
            public string ChangeAllText { get; set; } = string.Empty;

            private CheckedListBox ItemList { get; } = new CheckedListBox();

            public DropDownList() : base()
            {
                AnchorControl = this;

                AutoSize = true;
                AutoSizeMode = AutoSizeMode.GrowAndShrink;
                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.Manual;

                ItemList.IntegralHeight = true;
                ItemList.CheckOnClick = true;
                ItemList.Margin = new Padding(0);
                ItemList.ItemCheck += ItemList_ItemCheck;
                ItemList.LostFocus += ItemList_LostFocus; ;

                Controls.Add(ItemList);
            }

            // events
            private void ItemList_LostFocus(object sender, EventArgs evt)
            {
                DropDownFocusLost?.Invoke(this, null);
            }
            private void ItemList_ItemCheck(object sender, ItemCheckEventArgs evt)
            {
                if (ItemList.Items[evt.Index] is CheckableItem item)
                {
                    item.State = (evt.NewValue == CheckState.Checked);
                    if (evt.Index == 0)
                    {
                        ChangeAllStates(item.State);
                    }
                    ItemStateChanged?.Invoke(item, evt);
                }
            }

            // publics
            public void Reset()
            {
                ItemList.SetItemChecked(0, true);
                ResetAllStates();
            }
            public void SetItems(List<CheckableItem> items)
            {
                ItemList.BeginUpdate();
                ItemList.Items.Clear();
                ItemList.Items.Add(new CheckableItem(string.Format("#{0}", ChangeAllText), true));
                ItemList.Items.AddRange(items.ToArray());
                Reset();
                ResizeDropDown();
                ItemList.EndUpdate();
            }
            public void ShowPopup(IWin32Window parent)
            {
                if (AnchorControl is EcfToolBarCheckComboBox box)
                {
                    Location = box.PointToScreen(new Point(0, box.Height));
                    Width = Math.Max(Width, box.Width);
                }
                Show(parent);
            }
            public List<CheckableItem> GetItems()
            {
                return ItemList.Items.Cast<CheckableItem>().Skip(1).ToList();
            }
            public List<CheckableItem> GetCheckedItems()
            {
                return ItemList.Items.Cast<CheckableItem>().Skip(1).Where(item => item.State == true).ToList();
            }
            public List<CheckableItem> GetUncheckedItems()
            {
                return ItemList.Items.Cast<CheckableItem>().Skip(1).Where(item => item.State == false).ToList();
            }

            // privates
            private void ResetAllStates()
            {
                for (int i = 1; i < ItemList.Items.Count; i++)
                {
                    if (ItemList.Items[i] is CheckableItem item)
                    {
                        ItemList.SetItemChecked(i, item.InitState);
                    }
                }
            }
            private void ChangeAllStates(bool state)
            {
                for (int i = 1; i < ItemList.Items.Count; i++)
                {
                    ItemList.SetItemChecked(i, state);
                }
            }
            private void ResizeDropDown()
            {
                if (AnchorControl is EcfToolBarCheckComboBox box && ItemList.Items.Count > 0)
                {
                    int itemCount = Math.Min(ItemList.Items.Count, box.MaxDropDownItems);
                    int height = (ItemList.GetItemHeight(0) + 5) * itemCount;

                    int width = 0;
                    using (Graphics gfx = ItemList.CreateGraphics())
                    {
                        gfx.PageUnit = GraphicsUnit.Pixel;
                        foreach (CheckableItem item in ItemList.Items)
                        {
                            width = Math.Max(width, (int)gfx.MeasureString(item.Display, ItemList.Font).Width);
                        }
                        if (ItemList.Items.Count > box.MaxDropDownItems)
                        {
                            width += SystemInformation.VerticalScrollBarWidth;
                        }
                        width += CheckBoxRenderer.GetGlyphSize(gfx, CheckBoxState.CheckedNormal).Width + 5;
                        width = Math.Max(width, box.Width);
                    }

                    ItemList.Size = new Size(width, height);
                }
            }
        }
        public class CheckableItem
        {
            public string Id { get; }
            public string Display { get; }
            public bool InitState { get; }
            public bool State { get; set; }

            public CheckableItem(string id, string displayName, bool initState)
            {
                Id = id;
                Display = displayName;
                State = initState;
                InitState = initState;
            }
            public CheckableItem(string displayName, bool initState) : this(displayName, displayName, initState)
            {

            }

            public override string ToString()
            {
                return Display;
            }
        }
        private class DropDownButton : ButtonBase
        {
            public event EventHandler DropStateChanged;

            public bool IsUnderCursor { get; private set; } = false;
            public ComboBoxState State { get; private set; } = ComboBoxState.Normal;

            public DropDownButton(int height) : base()
            {
                Margin = new Padding(0);
                Size = new Size(height, height);
            }

            public void Reset()
            {
                if (!State.Equals(ComboBoxState.Normal))
                {
                    State = ComboBoxState.Normal;
                    DropStateChanged?.Invoke(this, null);
                    Invalidate();
                }
            }
            public void Drop()
            {
                if (!State.Equals(ComboBoxState.Pressed))
                {
                    State = ComboBoxState.Pressed;
                    DropStateChanged?.Invoke(this, null);
                    Invalidate();
                }
            }
            public void Switch()
            {
                State = State.Equals(ComboBoxState.Normal) ? ComboBoxState.Pressed : ComboBoxState.Normal;
                DropStateChanged?.Invoke(this, null);
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs evt)
            {
                ComboBoxRenderer.DrawDropDownButton(evt.Graphics, evt.ClipRectangle, State);
            }
            protected override void OnClick(EventArgs evt)
            {
                ComboBoxState oldState = State;
                switch (State)
                {
                    case ComboBoxState.Normal: State = ComboBoxState.Pressed; break;
                    default: State = ComboBoxState.Normal; break;
                }
                if (!State.Equals(oldState))
                {
                    DropStateChanged?.Invoke(this, null);
                    Invalidate();
                }
            }
            protected override void OnMouseEnter(EventArgs evt)
            {
                IsUnderCursor = true;
            }
            protected override void OnMouseLeave(EventArgs evt)
            {
                IsUnderCursor = false;
            }
        }
    }
    public class EcfToolBarTextBox : TextBox
    {
        private string ToolTipText { get; }
        private ToolTip Tip { get; } = new ToolTip();

        public EcfToolBarTextBox(string toolTip) : base()
        {
            ToolTipText = toolTip;

            MouseHover += TextBox_MouseHover;
        }

        private void TextBox_MouseHover(object sender, EventArgs evt)
        {
            Tip.SetToolTip(this, ToolTipText);
        }
        protected override void Dispose(bool disposing)
        {
            Tip.Dispose();
            base.Dispose(disposing);
        }
    }
    public class EcfToolBarButton : Button
    {
        private ToolTip Tip { get; } = new ToolTip();

        public EcfToolBarButton(string toolTip, Image image, string text) : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            Tip.SetToolTip(this, toolTip);
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            if (image != null)
            {
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                Image = image;
            }
            Text = text;
        }

        protected override void Dispose(bool disposing)
        {
            Tip.Dispose();
            base.Dispose(disposing);
        }
    }
    public class EcfToolBarRadioButton : RadioButton
    {
        private ToolTip Tip { get; } = new ToolTip();

        public EcfToolBarRadioButton(string toolTip, Image image, string text) : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            Tip.SetToolTip(this, toolTip);

            Appearance = Appearance.Button;
            if (image != null)
            {
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                FlatAppearance.BorderColor = SystemColors.ControlDark;
                FlatAppearance.CheckedBackColor = Color.Transparent;
                FlatAppearance.MouseOverBackColor = Color.Transparent;
                FlatAppearance.MouseDownBackColor = Color.Transparent;
                Image = image;
            }
            else if (text != null)
            {
                Text = text;
            }

            CheckedChanged += EcfToolBarRadioButton_CheckedChanged;
        }

        // events
        private void EcfToolBarRadioButton_CheckedChanged(object sender, EventArgs evt)
        {
            FlatAppearance.BorderSize = Checked ? 1 : 0;
        }
        protected override void Dispose(bool disposing)
        {
            Tip.Dispose();
            base.Dispose(disposing);
        }
    }
    public abstract class EcfToolBarCheckBox : CheckBox
    {
        private ToolTip Tip { get; } = new ToolTip();

        public EcfToolBarCheckBox(string toolTip) : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            Tip.SetToolTip(this, toolTip);

            AutoCheck = true;

            Appearance = Appearance.Button;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.CheckedBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
        }

        protected override void Dispose(bool disposing)
        {
            Tip.Dispose();
            base.Dispose(disposing);
        }
    }
    public class EcfToolBarTwoStateCheckBox : EcfToolBarCheckBox
    {
        private Image CheckedImage { get; }
        private Image UncheckedImage { get; }

        public EcfToolBarTwoStateCheckBox(string toolTip, Image checkedImage, Image uncheckedImage) : base(toolTip)
        {
            CheckedImage = checkedImage;
            UncheckedImage = uncheckedImage;
            Image = UncheckedImage;

            CheckStateChanged += ToolBarTwoStateCheckBox_CheckStateChanged;

            Reset();
        }

        // events
        private void ToolBarTwoStateCheckBox_CheckStateChanged(object sender, EventArgs evt)
        {
            Image = Checked ? CheckedImage : UncheckedImage;
        }

        // public
        public void Reset()
        {
            Checked = false;
        }
    }
    public class EcfToolBarThreeStateCheckBox : EcfToolBarCheckBox
    {
        private Image IndeterminateImage { get; }
        private Image CheckedImage { get; }
        private Image UncheckedImage { get; }

        public EcfToolBarThreeStateCheckBox(string toolTip, Image indeterminateImage, Image checkedImage, Image uncheckedImage) : base(toolTip)
        {
            IndeterminateImage = indeterminateImage;
            CheckedImage = checkedImage;
            UncheckedImage = uncheckedImage;

            ThreeState = true;

            CheckStateChanged += ToolBarThreeStateCheckBox_CheckStateChanged;

            Reset();
        }

        // events
        private void ToolBarThreeStateCheckBox_CheckStateChanged(object sender, EventArgs evt)
        {
            switch (CheckState)
            {
                case CheckState.Checked: Image = CheckedImage; break;
                case CheckState.Unchecked: Image = UncheckedImage; break;
                default: Image = IndeterminateImage; break;
            }
        }

        // publics
        public void Reset()
        {
            CheckState = CheckState.Indeterminate;
        }
    }
    public class EcfToolBarNumericUpDown : NumericUpDown
    {
        private ToolTip Tip { get; } = new ToolTip();

        public EcfToolBarNumericUpDown(string toolTip) : base()
        {
            SetStyle(ControlStyles.Selectable, false);

            Tip.SetToolTip(this, toolTip);
        }

        protected override void Dispose(bool disposing)
        {
            Tip.Dispose();
            base.Dispose(disposing);
        }
    }
    public class EcfToolBarComboBox : ComboBox
    {
        private ToolTip Tip { get; } = new ToolTip();

        public EcfToolBarComboBox(string toolTip) : base()
        {
            SetStyle(ControlStyles.Selectable, false);
            Tip.SetToolTip(this, toolTip);

            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void Dispose(bool disposing)
        {
            Tip.Dispose();
            base.Dispose(disposing);
        }
    }
    public class EcfToolBarLabel : Label
    {
        public bool IsForcingBoldStyle { get; }

        public EcfToolBarLabel(string text, bool forceBold) : base()
        {
            Text = text;
            IsForcingBoldStyle = forceBold;

            SetStyle(ControlStyles.Selectable, false);
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            FontChanged += ToolBarLabel_FontChanged;

            OnFontChanged(null);
        }

        private void ToolBarLabel_FontChanged(object sender, EventArgs evt)
        {
            if (IsForcingBoldStyle)
            {
                Font = new Font(Font, FontStyle.Bold);
            }
        }
    }
}
