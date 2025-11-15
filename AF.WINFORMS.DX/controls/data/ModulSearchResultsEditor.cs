using AF.MVC;
using DevExpress.Utils.Html;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.WinExplorer;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace crm.ui;

/// <summary>
/// Editor für ModulSearchResults
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class ModulSearchResultsEditor : AFEditor, IUIElement
{
    private readonly AFGridControl grid = null!;
    private readonly WinExplorerView view = null!;
    private readonly GridView viewAll = null!;
    private readonly BindingList<SearchHitHit> data = [];
    private AFSidePanel side = null!;
    private AFLabelCaptionH2 lblSearchProgress = null!;
    private readonly AFTablePanel table = null!;
    private int secCount;
    private SearchEngine? engine;

    /// <summary>
    /// Constructor
    /// </summary>
    public ModulSearchResultsEditor()
    {
        if (UI.DesignMode) return;

        UI.StyleChanged += (_, _) =>
        {
            view.Appearance.EmptySpace.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Control);
        };

        lblSearchProgress = new() { Dock = DockStyle.Top, Text = "Suche wird ausgeführt...", Padding = new(5) };
        lblSearchProgress.AutoSizeMode = LabelAutoSizeMode.Vertical;
        lblSearchProgress.Visible = false;
        Controls.Add(lblSearchProgress);

        table = new() { Dock = DockStyle.Fill, UseSkinIndents = false };
        table.BeginLayout();

        var btn = table.Add<AFButton>(1, 1).Text("ZURÜCK");
        btn.SetButton(eButtonType.Flat);
        btn.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowCircleLeft);
        btn.Click += (_, _) =>
        {
            grid.DataSource = null;
            grid.MainView = view;
            grid.FocusedView = view;
            Application.DoEvents();
            grid.DataSource = data;
            grid.RefreshDataSource();

            table.Rows[0].Visible = false;
        };

      

        var panel = table.Add<AFPanel>(2, 1, colspan: 2);

        side = new AFSidePanel() { Dock = DockStyle.Right, Size = new(250, 250) };
        panel.Controls.Add(side);
        side.BringToFront();

        grid = new() { Dock = DockStyle.Fill };
        panel.Controls.Add(grid);
        grid.BringToFront();

        viewAll = new();
        viewAll.GridControl = grid;

        viewAll.FocusedRowObjectChanged += (_, e) =>
        {
            if (e.Row is not IModel model) return;

            showPreview(model);
        };

        grid.ViewCollection.Add(viewAll);

        view = new();
        view.GridControl = grid;
        view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { Caption = "Element", FieldName = "Caption" });
        view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { Caption = "Gruppe", FieldName = "Group" });

        view.ColumnSet.TextColumn = view.Columns[0];
        view.ColumnSet.DescriptionColumn = view.Columns[1];
        view.ColumnSet.GroupColumn = view.Columns[1];

        view.GroupCount = 1;

        view.Appearance.EmptySpace.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Control);
        view.Appearance.EmptySpace.Options.UseBackColor = true;
        view.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        view.HtmlImages = Glyphs.GetObjectImages();
        view.OptionsHtmlTemplate.AllowContentSelection = DevExpress.Utils.DefaultBoolean.False;
        view.OptionsSelection.ItemSelectionMode = IconItemSelectionMode.Press;
        view.OptionsView.ContentHorizontalAlignment = DevExpress.Utils.HorzAlignment.Near;
        view.OptionsView.Style = WinExplorerViewStyle.ExtraLarge;
        view.OptionsViewStyles.ExtraLarge.HorizontalIndent = 5;
        view.OptionsViewStyles.ExtraLarge.VerticalIndent = 5;
        view.OptionsViewStyles.ExtraLarge.HtmlTemplate.Name = "empty";
        view.OptionsViewStyles.ExtraLarge.HtmlTemplate.Template = "<div class=\"container\">${Caption}</div>";
        view.OptionsViewStyles.ExtraLarge.HtmlTemplate.Styles = ".container { width: 340px; height: 60px;  }";

        //view.CustomDrawItem += (_, e) =>
        //{
        //    e.DrawHtml(, );
        //};

        view.QueryItemTemplate += (_, e) =>
        {
            if (e.Row is SearchHitHit hit)
            {
                e.Template.Assign(!hit.ShowAll
                    ? view.HtmlTemplates.FirstOrDefault(t => t.Name == hit.SectionName)
                    : view.HtmlTemplates.FirstOrDefault(t => t.Name == hit.SectionName + "_MORE"));
            }
        };

        view.HtmlElementMouseClick += (_, e) =>
        {
            if (e.ElementId == "pshopen" && view.GetRow(e.RowHandle) is SearchHitHit hit)
                UI.ViewManager.OpenPage(hit.Model.ModelLink);
            else if (e.ElementId == "psshowall" && view.GetRow(e.RowHandle) is SearchHitHit hitAll)
                searchAll(hitAll);
        };

        view.FocusedRowObjectChanged += (_, e) =>
        {
            if (e.Row is not SearchHitHit hit) return;

            showPreview(hit.Model);
        };

        grid.ViewCollection.Add(view);

        grid.MainView = view;
        grid.FocusedView = view;

        grid.DataSource = data;

        table.SetRow(2, TablePanelEntityStyle.Relative, 1f);
        table.SetColumn(2, TablePanelEntityStyle.Relative, 1f);

        table.EndLayout();
        Controls.Add(table);
        table.BringToFront();

        table.Rows[0].Visible = false;
    }


    private void showPreview(IModel model)
    {
        if (model.GetType().GetController() is not IControllerUI controller) return;

        var detailtype = controller.GetUIElementType(eUIElement.PluginDetail);

        if (detailtype == null && side.Controls.Count == 0) return;

        if (detailtype == null && side.Controls.Count > 0)
        {
            side.Controls.Clear(true);
            return;
        }

        if (detailtype != null && side.Controls.Count > 0 && side.Controls[0].GetType() == detailtype)
        {
            (side.Controls[0] as AFDetail)!.Model = model;
            return;
        }

        if (side.Controls.Count > 0) side.Controls.Clear(true);

        var detail = controller.GetUIElement(eUIElement.PluginDetail)!;
        
        ((Control)detail).Dock = DockStyle.Fill;

        side.ClientSize = side.ClientSize with { Width = ((Control)detail).Width };
        side.Controls.Add((Control)detail);


        (side.Controls[0] as AFDetail)!.Model = model;
    }


    private void searchAll(SearchHitHit hitAll)
    {
        if (hitAll.Section == null) return;

        grid.DataSource = null;
        grid.MainView = viewAll;
        grid.FocusedView = viewAll;
        Application.DoEvents();

        var controller = hitAll.Model.GetType().GetController();

        viewAll.Setup(controller.GetGridSetup(eGridStyle.SearchHits));

        var treffer = controller.Search(true, hitAll.Section.Query, hitAll.Section.QueryParameter);

        grid.DataSource = treffer;

        grid.RefreshDataSource();

        table.Rows[0].Visible = true;
    }

    /// <summary>
    /// Zugriff auf die Datenquelle/Liste der Treffer
    /// </summary>
    public BindingList<SearchHitHit> Datasource => data;

    /// <summary>
    /// Treffer für einen durchsuchten Typen anzeigen.
    /// </summary>
    /// <param name="section">Abschnitt/durchsuchter Typ</param>
    /// <param name="hits">Trefferliste</param>
    public void ShowHits(SearchHitSection section, IBindingList hits)
    {
        ++secCount;

        if (section.IsAll)
        {
            grid.FocusedView = viewAll;
            grid.MainView = viewAll;
            grid.DataSource = hits;
        }
        else
        {
            if (grid.FocusedView != view)
            {
                grid.DataSource = null;
                grid.FocusedView = view;
                grid.MainView = view;
                Application.DoEvents();

                grid.DataSource = Datasource;
            }

            Datasource.RaiseListChangedEvents = false;

            // HTML-Templates aus Controller besorgen...
            if (section.TypeDesc.GetController() is IControllerUI controller)
            {
                HtmlTemplate template = new() { Name = section.TypeDesc.Name, Styles = controller.SearchHitHtmlTemplate.CssTemplate, Template = controller.SearchHitHtmlTemplate.HtmlTemplate.Contains("#SYMBOL#") ? controller.SearchHitHtmlTemplate.HtmlTemplate.Replace("#SYMBOL#", "target") : controller.SearchHitHtmlTemplate.HtmlTemplate };
                view.HtmlTemplates.Add(template);

                template = new() { Name = section.TypeDesc.Name + "_MORE", Styles = controller.SearchMoreHtmlTemplate.CssTemplate, Template = controller.SearchMoreHtmlTemplate.HtmlTemplate };
                view.HtmlTemplates.Add(template);
            }

            int hitcnt = 0;
            string grpname = $"{secCount:00} {section.Caption} ({(hits.Count > 15 ? "15+" : hits.Count)} Treffer)";

            // Datasource.Add(section);
            foreach (IModel? hit in hits)
            {
                if (hit == null) continue;

                ++hitcnt;

                string caption = hit?.ToString() ?? "";

                Datasource.Add(item: new SearchHitHit { Model = hit!, Caption = caption, Group = grpname, SectionName = section.TypeDesc.Name, ShowAll = hitcnt > 15, Section = (hitcnt > 15 ? section : null) });
            }

            Datasource.RaiseListChangedEvents = true;
        }

        grid.RefreshDataSource();
    }


    /// <summary>
    /// Suche wurde gestartet...
    /// </summary>
    public void BeginSearch()
    {
        if (grid.DataSource is IBindingList list)
            list.Clear();

        Datasource.Clear();

        secCount = 0;

        lblSearchProgress.Visible = true;
        table.Rows[0].Visible = false;

        Application.DoEvents();
    }

    /// <summary>
    /// Suche wurde beendet...
    /// </summary>
    public void EndSearch()
    {
        lblSearchProgress.Visible = false;

        Application.DoEvents();

    }

    /// <summary>
    /// Beginn der Suche im übergebenen Typen.
    /// </summary>
    /// <param name="tdesc">Beschreibung des Typs, der durchsucht wird.</param>
    public void SearchProgress(TypeDescription tdesc)
    {
        lblSearchProgress.Text = $"Suche wird ausgeführt... Suche nach {tdesc.Context?.NamePlural ?? tdesc.Name}.";
        Application.DoEvents();
    }

    /// <summary>
    /// Zu verwendende Suchmaschine setzen.
    /// </summary>
    /// <param name="searchengine"></param>
    public void SetEngine(SearchEngine searchengine)
    {
        engine = searchengine;
    }
}

