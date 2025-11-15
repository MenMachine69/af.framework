using AF.WINFORMS.DX.Properties;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;

namespace AF.WINFORMS.DX;

/// <summary>
/// Browser für Klassen und Typen z.B. im ScriptDesigner
/// </summary>
public partial class AFClassBrowser : AFUserControl
{
    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFClassBrowser()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        cmbClass.Properties.ValueMember = "ClassType";
        cmbClass.Properties.DisplayMember = "FullName";
        cmbClass.Properties.ExportMode = ExportMode.Value;

        cmbClass.Properties.View.OptionsBehavior.AutoPopulateColumns = false;
        cmbClass.Properties.View.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;

        cmbClass.Properties.AllowNullInput = DefaultBoolean.True;
        cmbClass.Properties.NullValuePromptShowForEmptyValue = true;
        cmbClass.Properties.NullText = "<select class>";

        AFGridSetup setup = new()
        {
            GroupBy = [nameof(ClassInfo.NameSpace)],
            SortOn = nameof(ClassInfo.ClassName),
            DefaultGridStyle = eGridMode.GridView
        };
        setup.Columns.Add(new() { ColumnType = typeof(string), Caption = "Namespace", ColumnFieldname = nameof(ClassInfo.NameSpace) });
        setup.Columns.Add(new() { ColumnType = typeof(string), Caption = "Class", ColumnFieldname = nameof(ClassInfo.ClassName) });

        cmbClass.Setup(setup);

        cmbClass.AddButton(UI.GetImage(Symbol.Organization),
            imagesize: new Size(16, 16),
            enabled: false,
            showleft: true);

        

        treeClassStructure.SelectImageList = Glyphs.GetImages();
        treeClassStructure.NodeCellStyle += (_, e) =>
        {
            if (e.Node.Tag is Tuple<MethodInfo, bool> me && me.Item2)
                e.Appearance.FontStyleDelta = FontStyle.Bold;

            if (e.Node.Tag is Tuple<PropertyInfo, bool> pr && pr.Item2)
                e.Appearance.FontStyleDelta = FontStyle.Bold;
        };
    }

    /// <summary>
    /// Zugriff auf die Baumansicht
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFTreeGrid TreeView => treeClassStructure;


    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        cmbClass.QueryPopUp += beforeCmbClassPopup;
        cmbClass.EditValueChanged += classSelected;
        treeClassStructure.FocusedNodeChanged += treeNodeSelected;
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        cmbClass.QueryPopUp -= beforeCmbClassPopup;
        cmbClass.EditValueChanged -= classSelected;
        treeClassStructure.FocusedNodeChanged -= treeNodeSelected;

        base.OnHandleDestroyed(e);
    }

    private void beforeCmbClassPopup(object? sender, CancelEventArgs e)
    {
        var repotypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(t => t.GetTypes())
            .Where(t => t.IsClass && t.IsAbstract == false && t.IsPublic);

        BindingList<ClassInfo> data = [];

        foreach (Type type in repotypes)
        {
            bool ignore = true;

            // Filtern nach Namespaces, FOR Schleife ist schneller als foreach!
            for (int i = 0; i < NameSpaces.Count; i++)
            {
                if (!(type.Namespace?.StartsWith(NameSpaces[i]) ?? false)) continue;

                ignore = false;
                break;
            }

            if (ignore) continue;   

            data.Add(new(type) { ClassName = type.Name, NameSpace = type.Namespace ?? "<none>" });
        }

        cmbClass.Properties.DataSource = data;
    }

    private void classSelected(object? sender, EventArgs e)
    {
        if (cmbClass.EditValue is not Type classtype) return;

        treeClassStructure.ClearNodes();

        List<AFTreeListNode> nodes = [];

        AFTreeListNode methods = new() { Caption = Resources.LBL_METHODS, ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };
        AFTreeListNode propertys = new() { Caption = Resources.LBL_PROPERTIES, ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };

        nodes.Add(new() { Caption = classtype.FullName ?? "", Tag = classtype, ImageIndex = (int)Symbol.Organization, ImageIndexSelected = (int)Symbol.Organization });

        foreach (MethodInfo method in classtype.GetMethods().Where( p => p.IsSpecialName == false).OrderBy(p => p.Name))
        {
            if (method.IsPublic == false)
                continue;

            methods.ChildNodes.Add(new() 
            { 
                Caption = method.GetSignature(), 
                ImageIndex = (int)Symbol.Cube, 
                Tag = new Tuple<MethodInfo, bool>(method, method.DeclaringType?.Equals(classtype) ?? false), 
                ImageIndexSelected = (int)Symbol.Cube,
            });
        }

        foreach (PropertyInfo property in classtype.GetProperties().OrderBy( p => p.Name))
        {
            propertys.ChildNodes.Add(new() 
            { 
                Caption = $@"{property.Name} ({property.PropertyType.Name})", 
                ImageIndex = (int)Symbol.Wrench, 
                Tag = new Tuple<PropertyInfo, bool>(property, property.DeclaringType?.Equals(classtype) ?? false),
                ImageIndexSelected = (int)Symbol.Wrench 
            });
        }

        nodes[0].ChildNodes.Add(methods);
        nodes[0].ChildNodes.Add(propertys);

        treeClassStructure.Fill(nodes);

        treeClassStructure.ExpandToLevel(1);
    }

    private void treeNodeSelected(object sender, FocusedNodeChangedEventArgs e)
    {
        if (e.Node == null) 
        {
            mleInfo.Text = "";
            return;
        }

        if (e.Node.Tag is Tuple<MethodInfo, bool> info)
            mleInfo.Text = info.Item1.GetSummary(out _, out _);
        else if (e.Node.Tag is Tuple<PropertyInfo, bool> propertyInfo)
            mleInfo.Text = propertyInfo.Item1.GetSummary();
        else
            mleInfo.Text = "";
    }

    /// <summary>
    /// Liste der Namespaces, aus denen die Klassen angezeigt werden sollen (Filter)
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<string> NameSpaces { get; } = [];

    /// <summary>
    /// Alle Namespaces registrieren, die standardmäßig angezeigt werden sollen (AF.*)
    /// </summary>
    public void RegisterDefaultNamespaces()
    {
        NameSpaces.Add("AF.CORE");
        NameSpaces.Add("AF.DATA");
        NameSpaces.Add("AF.MVC");
        NameSpaces.Add("AF.WINFORMS.DX");
    }
}

internal class ClassInfo(Type classtype)
{
    /// <summary>
    /// NameSPace der Klasse
    /// </summary>
    public string NameSpace { get; set; } = "";

    /// <summary>
    /// Name der Klasse
    /// </summary>
    public string ClassName { get; set; } = "";

    /// <summary>
    /// vollst. Name der Klasse
    /// </summary>
    public string FullName => NameSpace + "." + ClassName;

    /// <summary>
    /// Typ, der die Klasse repräsentiert.
    /// </summary>
    public Type ClassType { get; init; } = classtype;

    /// <summary>
    /// Gibt an, ob die Methode/Eigenschaft aus einer übergeordneten Klasse stammt.
    /// </summary>
    public bool FromBaseClass { get; set; }
}