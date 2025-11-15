using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraTreeList;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterter Explorer für Systemfehler
/// </summary>
public sealed class FormExceptionExplorer : FormBase
{
    private AFEditMultiline errMsg = null!;
    private AFEditMultiline stack = null!;
    private AFTreeGrid tree = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public FormExceptionExplorer(ExceptionInfo exception)
    {
        if (UI.DesignMode) return;

        Padding = new(8, 3, 8, 8);
        Text = "Informationen zum Fehler";
        
        AFTablePanel table = new() { Dock = DockStyle.Fill, UseSkinIndents = true };
        table.BeginLayout();
        table.Add<AFLabelGrayText>(1, 1).Text = "Fehlermeldung";
        errMsg = table.Add<AFEditMultiline>(2, 1);
        errMsg.ReadOnly = true;
        table.Add<AFLabelGrayText>(3, 1).Text = "Callstack";
        stack = table.Add<AFEditMultiline>(4, 1);
        stack.ReadOnly = true;

        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(2, TablePanelEntityStyle.AutoSize);
        table.SetRow(4, TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();

        tree = new() { Dock = DockStyle.Fill }; 
        AFSplitContainer splitter = new() { Dock = DockStyle.Fill };

        splitter.FixedPanel = SplitFixedPanel.Panel1;
        splitter.SplitterPosition = 250;
        splitter.Panel1.Controls.Add(tree);
        splitter.Panel2.Controls.Add(table);
        


        AFNavTabControl tabs = new() { Dock = DockStyle.Fill };
        tabs.PaintStyleName = "AFFlat";
        var btn = new CustomHeaderButton(ButtonPredefines.Glyph);
        btn.Width = 24;
        btn.ToolTip = "Fehlermeldung exportieren";
        btn.ImageOptions.SvgImage = UI.GetImage(Symbol.Save);
        btn.ImageOptions.SvgImageSize = new(16, 16);
        tabs.CustomHeaderButtons.Add(btn);

        tabs.CustomHeaderButtonClick += (_, _) =>
        {
            exception.ToJsonFile(new("c:\\temp\\error.json"));
        };

        var page = new XtraTabPage() { Text = "Fehlermeldung" };
        page.Controls.Add(splitter);
        tabs.TabPages.Add(page);
        
        page = new XtraTabPage() { Text = "Systeminformationen" };
        page.Controls.Add(new AFGridControl() { Dock = DockStyle.Fill });
        tabs.TabPages.Add(page);

        AFGridSetup setup = new()
        {
            AllowEdit = false,
            AllowAddNew = false,
            AllowMultiSelect = false
        };
        setup.Columns.Add(new() { ColumnFieldname = nameof(KeyValue.Key), Caption = "Name"});
        setup.Columns.Add(new() { ColumnFieldname = nameof(KeyValue.Value), Caption = "Wert", Bold = true });

        ((AFGridControl)page.Controls[0]).Setup(setup);
        ((AFGridControl)page.Controls[0]).DataSource = exception.SystemInformations;

        page = new XtraTabPage() { Text = "Module" };
        page.Controls.Add(new AFGridControl() { Dock = DockStyle.Fill });
        tabs.TabPages.Add(page);

        setup = new()
        {
            AllowEdit = false,
            AllowAddNew = false,
            AllowMultiSelect = false
        };
        setup.Columns.Add(new() { ColumnFieldname = nameof(AFSystemInformation.AssemblyInformation.Name), Caption = "Name" });
        setup.Columns.Add(new() { ColumnFieldname = nameof(AFSystemInformation.AssemblyInformation.FullPath), Caption = "Name" });
        setup.Columns.Add(new() { ColumnFieldname = nameof(AFSystemInformation.AssemblyInformation.IsDynamic), Caption = "Dynamic" });

        ((AFGridControl)page.Controls[0]).Setup(setup);
        ((AFGridControl)page.Controls[0]).DataSource = exception.Assemblies;


        page = new XtraTabPage() { Text = "Screenshots" };
        page.Controls.Add(new AFPictureBox() { Dock = DockStyle.Fill, Properties = { SizeMode = PictureSizeMode.Squeeze }});
        tabs.TabPages.Add(page);

        if (exception.Screenshots.Count > 0)
            ((AFPictureBox)page.Controls[0]).Image = exception.Screenshots[0];

        Controls.Add(tabs);

        List<AFTreeListNode> nodes = [];
        AFTreeListNode parent = new() { Caption = exception.Exception!.GetType().Name, Tag = exception.Exception! };
        nodes.Add(parent);
        Exception current = exception.Exception!;

       

        tree.Fill(nodes);
        tree.ExpandToLevel(0);

        tree.FocusedNodeChanged += treeNodeSelected;

        
        StartPosition = FormStartPosition.CenterScreen;
        Size = new(800, 600);
    }


    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        treeNodeSelected(this, new FocusedNodeChangedEventArgs(null, tree.FocusedNode ));
    }

    private void treeNodeSelected(object sender, FocusedNodeChangedEventArgs e)
    {
        if (e.Node == null)
        {
            errMsg.Text = "";
            stack.Text = "";
            return;
        }

        if (e.Node is not { Tag: Exception exception }) return;


        errMsg.Text = exception.Message;
        stack.Text = exception.StackTrace;

        errMsg.Size = new(errMsg.Width, Math.Min(errMsg.CalcAutoHeight(), 150));
    }
}

