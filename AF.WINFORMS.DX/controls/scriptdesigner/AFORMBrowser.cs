using System.Text;
using AF.MVC;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraTreeList;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars;

namespace AF.WINFORMS.DX;

/// <summary>
/// Browser für ORM einer Anwendung.
///
/// Angezeigt werden die Models und die zugehörigen Controller.
/// </summary>
[DesignerCategory("Code")]
public class AFORMBrowser : AFUserControl
{
    private readonly List<Tuple<IDatabase, string>> databases = [];
    private readonly AFLabel mleInfo = null!;
    private readonly AFTreeGrid treeClassStructure = null!;
    private bool showVererbung = false;
    private readonly AFBarManager manager = null!;
    private readonly AFBarController barController = null!;
    private readonly AFEditFind sleFind = null!;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFORMBrowser()
    {
        if (UI.DesignMode) return;

        barController = new();
        // barController.AutoBackColorInBars = true;

        manager = new();
        manager.Form = this;
        manager.Controller = barController;
        manager.BeginInit();

        AFTablePanel table = new AFTablePanel() { Dock = DockStyle.Top, AutoSize = true, UseSkinIndents = false };
        Controls.Add(table);

        table.BeginLayout();
        var tbar = table.AddBar(manager, 1, 1);
        var btn = tbar.AddButton("btnToggleVererbung", img: UI.GetImage(Symbol.AppsAddIn));
        btn.SuperTip = UI.GetSuperTip("Anzeige geerbte", "Umschalten der Anzeige geerbter Methoden und Eigenschaften.");
        btn.ButtonStyle = BarButtonStyle.Check;
        btn.ItemClick += (_, e) =>
        {
            if (e.Item.Name == "btnToggleVererbung")
            {
                showVererbung = !showVererbung;
                LoadTree();
            }
        };

        sleFind = table.Add<AFEditFind>(2, 1);
        sleFind.Tree = treeClassStructure;
        sleFind.Margin = new(0);

        table.SetRow(1, TablePanelEntityStyle.Absolute, 30.0f);
        table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        table.EndLayout();

        AFSplitContainer splitter = new() { Dock = DockStyle.Fill, FixedPanel = SplitFixedPanel.Panel2, Horizontal = false };
        Controls.Add(splitter);
        splitter.BringToFront();

        splitter.Panel2.Height = 150;

        treeClassStructure = new() { Dock = DockStyle.Fill, BorderStyle = BorderStyles.NoBorder };
        splitter.Panel1.Controls.Add(treeClassStructure);

        treeClassStructure.SelectImageList = Glyphs.GetImages();
        treeClassStructure.NodeCellStyle += (_, e) =>
        {
            if (e.Node.Tag is Tuple<MethodInfo, bool> { Item2: true })
                e.Appearance.FontStyleDelta = FontStyle.Bold;

            if (e.Node.Tag is Tuple<PropertyInfo, bool> { Item2: true })
                e.Appearance.FontStyleDelta = FontStyle.Bold;
        };

        splitter.Panel2.Controls.Add(new SeparatorControl() { Size = new(100, 1), Dock = DockStyle.Top, Margin = new(0), Padding = new(0) });

        mleInfo = new() { Padding = new(3), BorderStyle = BorderStyles.NoBorder, Dock = DockStyle.Fill, AllowHtmlString = true, Appearance = { TextOptions = { WordWrap = WordWrap.Wrap, VAlignment = VertAlignment.Top } } };
        splitter.Panel2.Controls.Add(mleInfo);
        mleInfo.BringToFront();

        manager.EndInit();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        LoadTree();

        treeClassStructure.HideFindPanel();
        treeClassStructure.FocusedNodeChanged += treeNodeSelected;
    }


    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (UI.DesignMode)
        {
            base.OnHandleDestroyed(e);
            return;
        }

        treeClassStructure.FocusedNodeChanged -= treeNodeSelected;

        base.OnHandleDestroyed(e);

        manager?.Dispose();
        barController?.Dispose();
    }
    
    private void treeNodeSelected(object sender, FocusedNodeChangedEventArgs e)
    {
        if (e.Node == null) 
        {
            mleInfo.Text = "";
            return;
        }

        if (e.Node.Tag is Tuple<MethodInfo, bool> info)
        {
            StringBuilder sbtext = new();
            
            sbtext.AppendLine(info.Item1.GetSummary(out var paramliste, out var returndesc));

            var parameters = info.Item1.GetParameters();
            
            if (parameters.Length > 0)
            {
                sbtext.AppendLine("\r\n<b><u>Parameter</u></b>");
                foreach (var parameter in parameters)
                {
                    sbtext.AppendLine("<b>" +parameter.Name + "</b> ("+parameter.ParameterType.Name+ (parameter.IsOptional ? ", optional: " +(parameter.DefaultValue?.ToString() ?? "null"): "") +")");
                    sbtext.Append(paramliste.FirstOrDefault(p => p.Item1 == parameter.Name)?.Item2 + "\r\n" ?? "");
                }
            }
            sbtext.AppendLine("\r\n<b><u>Rückgabe</u></b>");
            sbtext.AppendLine(info.Item1.ReturnParameter?.ParameterType.Name ?? "void");
             sbtext.AppendLine(returndesc);
            mleInfo.Text = sbtext.ToString();
        }
        else if (e.Node.Tag is Tuple<PropertyInfo, bool> propertyInfo)
            mleInfo.Text = propertyInfo.Item1.GetSummary();
        else
            mleInfo.Text = "";
    }

    /// <summary>
    /// Eine Datenbank, deren Models angezeigt werden sollen registrieren.
    /// </summary>
    /// <param name="database"></param>
    /// <param name="name"></param>
    public void RegisterDatabase(IDatabase database, string name)
    {
        databases.Add(new(database, name));
    }

    /// <summary>
    /// Registrierte Datenbanken entfernen
    /// </summary>
    public void ClearDatabases()
    {
        databases.Clear();
    }

    /// <summary>
    /// Baumstruktur laden...
    /// </summary>
    public void LoadTree()
    {
        treeClassStructure.ClearNodes();

        List<AFTreeListNode> nodes = [];

        // Filtern nach Namespaces, FOR Schleife ist schneller als foreach!
        foreach (var database in databases.OrderBy(db => db.Item2))
        {
            AFTreeListNode nodeDB = new() { Caption = database.Item2, Tag = database, ImageIndex = (int)Symbol.Organization, ImageIndexSelected = (int)Symbol.Organization };

            foreach (var baseTableType in database.Item1.Configuration.BaseTableTypes)
            {
                foreach (var tableType in baseTableType.GetChildTypesOf().OrderBy(typ => typ.FullName))
                {
                    var tdesc = tableType.GetTypeDescription();

                    AFTreeListNode nodeTable = new() { Caption = tdesc.Name + " (" + tableType.FullName + ")", ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };

                    AFTreeListNode nodePropertys = new() { Caption = "Eigenschaften", ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };

                    foreach (PropertyInfo property in tdesc.Properties.Values.OrderBy(p => p.Name))
                    {
                        if (!showVererbung && !(property.DeclaringType?.Equals(tableType) ?? false))
                            continue;

                        nodePropertys.ChildNodes.Add(new()
                        {
                            Caption = $@"{property.Name} ({property.PropertyType.Name})",
                            ImageIndex = (int)Symbol.Wrench,
                            Tag = new Tuple<PropertyInfo, bool>(property, property.DeclaringType?.Equals(tableType) ?? false),
                            ImageIndexSelected = (int)Symbol.Wrench
                        });
                    }

                    nodeTable.ChildNodes.Add(nodePropertys);

                    AFTreeListNode nodeViews = new() { Caption = "Views", ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };

                    var views = TypeEx.GetViewTypes(tdesc);
                    foreach (var view in views)
                    {
                        AFTreeListNode nodeView = new() { Caption = view.Name + " (" + view.Type.FullName + ")", ImageIndex = (int)Symbol.Filter, Tag = null, ImageIndexSelected = (int)Symbol.Filter };

                        nodePropertys = new() { Caption = "Eigenschaften", ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };

                        foreach (PropertyInfo property in view.Properties.Values.OrderBy(p => p.Name))
                        {
                            if (!showVererbung && !(property.DeclaringType?.Equals(view.Type) ?? false))
                                continue;

                            nodePropertys.ChildNodes.Add(new()
                            {
                                Caption = $@"{property.Name} ({property.PropertyType.Name})",
                                ImageIndex = (int)Symbol.Wrench,
                                Tag = new Tuple<PropertyInfo, bool>(property, property.DeclaringType?.Equals(view.Type) ?? false),
                                ImageIndexSelected = (int)Symbol.Wrench
                            });
                        }

                        nodeView.ChildNodes.Add(nodePropertys);

                        AFTreeListNode nodeViewMethods = new() { Caption = "Methoden", ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };

                        foreach (MethodInfo method in view.Type.GetMethods().Where(p => p.IsSpecialName == false).OrderBy(p => p.Name))
                        {
                            if (method.IsPublic == false)
                                continue;

                            if (!showVererbung && !(method.DeclaringType?.Equals(view.Type) ?? false))
                                continue;

                            nodeViewMethods.ChildNodes.Add(new()
                            {
                                Caption = method.GetSignature(),
                                ImageIndex = (int)Symbol.Cube,
                                Tag = new Tuple<MethodInfo, bool>(method, method.DeclaringType?.Equals(view.Type) ?? false),
                                ImageIndexSelected = (int)Symbol.Cube,
                            });
                        }

                        nodeView.ChildNodes.Add(nodeViewMethods);
                        nodeViews.ChildNodes.Add(nodeView);
                    }


                    nodeTable.ChildNodes.Add(nodeViews);

                    AFTreeListNode nodeMethods = new() { Caption = "Methoden", ImageIndex = (int)Symbol.Folder, Tag = null, ImageIndexSelected = (int)Symbol.Folder };

                    try
                    {
                        var controller = tableType.GetController()?.GetType();
                        if (controller != null)
                        {
                            if (controller.HasInterface(typeof(IControllerUI)))
                                controller = controller.BaseType;
                        }

                        if (controller != null)
                        {
                            AFTreeListNode nodeController = new() { Caption = controller.Name, ImageIndex = (int)Symbol.Settings, Tag = null, ImageIndexSelected = (int)Symbol.Settings };

                            foreach (MethodInfo method in controller.GetMethods().Where(p => p.IsSpecialName == false).OrderBy(p => p.Name))
                            {
                                if (method.IsPublic == false)
                                    continue;

                                if (!showVererbung && !(method.DeclaringType?.Equals(controller) ?? false))
                                    continue;


                                nodeController.ChildNodes.Add(new()
                                {
                                    Caption = method.GetSignature(),
                                    ImageIndex = (int)Symbol.Cube,
                                    Tag = new Tuple<MethodInfo, bool>(method, method.DeclaringType?.Equals(controller) ?? false),
                                    ImageIndexSelected = (int)Symbol.Cube,
                                });
                            }

                            nodeMethods.ChildNodes.Add(nodeController);
                        }
                    }
                    catch //(Exception ex)
                    {
                        // do nothing...
                    }

                    foreach (MethodInfo method in tableType.GetMethods().Where(p => p.IsSpecialName == false).OrderBy(p => p.Name))
                    {
                        if (method.IsPublic == false)
                            continue;

                        if (!showVererbung && !(method.DeclaringType?.Equals(tableType) ?? false))
                            continue;

                        nodeMethods.ChildNodes.Add(new()
                        {
                            Caption = method.GetSignature(),
                            ImageIndex = (int)Symbol.Cube,
                            Tag = new Tuple<MethodInfo, bool>(method, method.DeclaringType?.Equals(tableType) ?? false),
                            ImageIndexSelected = (int)Symbol.Cube,
                        });
                    }

                    

                    nodeTable.ChildNodes.Add(nodeMethods);
                
                    nodeDB.ChildNodes.Add(nodeTable);
                }


            }

            nodes.Add(nodeDB);
        }

        treeClassStructure.Fill(nodes);
        treeClassStructure.ExpandToLevel(0);
    }
}

internal class ORMInfo(Type modelType)
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
    /// Typ, der die Klasse repräsentiert...
    /// </summary>
    public Type ClassType { get; init; } = modelType;

    /// <summary>
    /// Gibt an, ob die Methode/Eigenschaft aus einer übergeordneten Klasse stammt.
    /// </summary>
    public bool FromBaseClass { get; set; }
}