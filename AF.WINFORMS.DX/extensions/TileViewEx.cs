using AF.MVC;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;

namespace AF.WINFORMS.DX;

/// <summary>
/// Extension Methoden für TileView
/// </summary>
public static class TileViewEx
{
    /// <summary>
    /// TileView für HTML/CSS Ansicht vorbereiten
    /// </summary>
    /// <param name="tileView">View, der in die HTML-Ansicht gesetzt werden soll</param>
    /// <param name="CssTemplate">CSS Template für die Darstellung (Standard ist NULL - Template kann/muss später gesetzt werden)</param>
    /// <param name="HtmlTemplate">HTML Template (Standard ist NULL - Template kann/muss später gesetzt werden)</param>
    public static void SetHtmlMode(this TileView tileView, string? CssTemplate = null, string? HtmlTemplate = null)
    {
        tileView.BorderStyle = BorderStyles.NoBorder;
        tileView.OptionsBehavior.AutoPopulateColumns = false;
        tileView.OptionsBehavior.AllowSmoothScrolling = true;
        tileView.OptionsFind.AllowFindPanel = false;
        tileView.OptionsTiles.ShowGroupText = false;
        tileView.OptionsHtmlTemplate.ItemAutoHeight = true;
        tileView.OptionsList.DrawItemSeparators = DrawItemSeparatorsMode.None;
        tileView.OptionsTiles.IndentBetweenGroups = 0;
        tileView.OptionsTiles.IndentBetweenItems = 0;
        tileView.OptionsTiles.LayoutMode = TileViewLayoutMode.List;
        tileView.OptionsTiles.Orientation = Orientation.Vertical;
        tileView.OptionsTiles.Padding = new Padding(0);
        tileView.OptionsTiles.RowCount = 0;
        tileView.HtmlImages = Glyphs.GetImages();

        if (CssTemplate != null) tileView.TileHtmlTemplate.Styles = CssTemplate;
        if (HtmlTemplate != null) tileView.TileHtmlTemplate.Template = HtmlTemplate;
    }

    /// <summary>
    /// Add a new column to the gridview
    /// </summary>
    /// <param name="view">gridview to which the column should be added</param>
    /// <param name="columndefinition">definition of the column</param>
    /// <param name="allowedit">edit mode on/off</param>
    /// <returns>the new created column</returns>
    public static GridColumn CreateColumn(this TileView view, AFGridColumn columndefinition, bool allowedit)
    {
        GridColumn column = new()
        {
            Caption = columndefinition.Caption,
            FieldName = (columndefinition.ColumnProperty != null
                ? columndefinition.ColumnProperty.Name
                : columndefinition.ColumnFieldname ?? columndefinition.Caption),
            OptionsColumn = { ShowCaption = false }
        };
        column.Name = "col" + column.FieldName;

        view.Columns.Add(column);

        return column;
    }
    
    /// <summary>
    /// Identifier für das TileView setzen
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="identifier">ID des TileViews</param>
    public static void SetGridIdentifier(this TileView view, Guid identifier)
    {
        if (view.Tag is not AFGridViewState state)
            view.Tag = new AFGridViewState(identifier) { KeyName = "tvw" }; 
        else
            state.Identifier = identifier;
    }

    /// <summary>
    /// Identifier für das TileView holen
    /// </summary>
    /// <param name="view">TileView</param>
    /// <returns>ID (Guid) des TileViews (gesetzt via SetGridIdentifier oder Setup) oder NULL</returns>
    public static Guid? GetGridIdentifier(this TileView view)
    {
        if (view.Tag is not AFGridViewState state)
            return null;

        return state.Identifier;
    }

    /// <summary>
    /// Liefert die gespeicherten Layouts eines Benutzers als Dictionary.
    /// </summary>
    /// <param name="tileview">GridView</param>
    /// <param name="extName">Erweiterungsname</param>
    /// <returns>Dictionary mit den Layouts</returns>
    public static Dictionary<Guid, string> GetAvailableLayouts(this TileView tileview, string? extName = null)
    {
        Guid? ident = tileview.GetGridIdentifier();
        if (ident == null) return [];

        return AFCore.App.Persistance?.GetNamedValues((Guid)ident, extName: extName) ?? [];
    }

    /// <summary>
    /// Setup des TileViews.
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="setup">Einstellungen für das TileView</param>
    /// <param name="kanbanMode">TileView als Kanban darstellen</param>
    public static void Setup(this TileView view, AFGridSetup setup, bool kanbanMode = false)
    {
        if (setup.GridIdentifier.IsNotEmpty() && view.GetGridIdentifier().Equals(setup.GridIdentifier))
            return;

        if (setup.GridIdentifier.IsNotEmpty())
            view.SetGridIdentifier(setup.GridIdentifier);

        view.GridControl.DataSource = null;
        view.GridControl.RefreshDataSource();

        view.BeginInit();

        view.SetHtmlMode(
            HtmlTemplate: setup.DefaultHtmlTemplate,
            CssTemplate: setup.DefaultCssTemplate);

        foreach (AFGridColumn column in setup.Columns)
            CreateColumn(view, column, false);

        if (kanbanMode)
        {
            view.Appearance.Group.FontStyleDelta = FontStyle.Bold;
            view.Appearance.Group.FontSizeDelta = 2;
            view.Appearance.Group.Options.UseFont = true;
            
            view.OptionsTiles.LayoutMode = TileViewLayoutMode.Kanban;
            view.OptionsTiles.HighlightFocusedTileStyle = HighlightFocusedTileStyle.Content;
            view.OptionsTiles.HorizontalContentAlignment = HorzAlignment.Near;
            view.OptionsTiles.VerticalContentAlignment = VertAlignment.Top;
            view.OptionsTiles.Orientation = Orientation.Horizontal;
            view.OptionsTiles.ShowGroupText = true;
            view.OptionsTiles.GroupTextPadding = new Padding(20, 12, 0, 12);
            view.OptionsTiles.IndentBetweenGroups = 30;
            view.OptionsTiles.IndentBetweenItems = 5;
            view.OptionsTiles.ItemPadding = new Padding(8);
            view.OptionsTiles.ItemSize = setup.KanbanOptions.TileSize;
            view.OptionsHtmlTemplate.ItemAutoHeight = setup.KanbanOptions.AutoHeight;

            view.OptionsBehavior.AllowSmoothScrolling = true;

            view.OptionsEditForm.ActionOnModifiedRowChange = EditFormModifiedAction.Nothing;
            view.OptionsEditForm.ShowUpdateCancelPanel = DefaultBoolean.False;
            
            view.OptionsKanban.GroupFooterButton.Visible = DefaultBoolean.False;
            view.OptionsKanban.ShowGroupBackground = DefaultBoolean.False;

            view.OptionsDragDrop.AllowDrag = setup.KanbanOptions.AllowDrag;

            foreach (AFKanbanGroup column in setup.KanbanGroups)
                view.OptionsKanban.Groups.Add(new() { Caption = column.Caption, Name = column.Name, GroupValue = column.GroupValue });

            if (setup.KanbanOptions.GroupColumn.IsEmpty() || view.Columns.FirstOrDefault(col => col.FieldName == setup.KanbanOptions.GroupColumn) == null)
                throw new ArgumentException($"Name der Gruppenspalte nicht angegeben oder Spalte mit angegebenem Namen existiert nicht ({setup.KanbanOptions.GroupColumn}).");

            view.ColumnSet.GroupColumn = view.Columns[setup.KanbanOptions.GroupColumn];
        }
        
        view.OptionsSelection.MultiSelect = setup.AllowMultiSelect;

        if (setup.SortOn.IsEmpty() == false)
        {
            GridColumn? column = view.Columns.FirstOrDefault(col => col.FieldName == setup.SortOn);

            if (column != null)
            {
                ColumnSortOrder order = setup.SortOrder == eOrderMode.Ascending ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;
                column.SortOrder = order;
                view.SortInfo.Add(new GridColumnSortInfo(column, order));
            }
        }

        view.EndInit();

        if (setup.HtmlImages is { Length: > 0 })
        {
            SvgImageCollection collection = new();

            setup.HtmlImages.ForEach(i => collection.Add(i.Item1, i.Item2));
            view.HtmlImages = collection;
        }
        else
            view.HtmlImages = Glyphs.GetImages();
        
        // view.FocusedRowObjectChanged += View_FocusedRowObjectChanged;

        if (view.Tag is not AFGridViewState state) return;

        state.DefaultTemplate = view.GetTemplate();
    }

    
    /// <summary>
    /// Aktuelles Template as AFHtmlTemplate sichern
    /// </summary>
    /// <param name="view">TileView</param>
    /// <returns>AFHtmlTemplate mit den aktuellen Einstellungen</returns>
    public static AFHtmlTemplate GetTemplate(this TileView view)
    {
        return new(view.TileHtmlTemplate.Template, view.TileHtmlTemplate.Styles);
    }

    /// <summary>
    /// Aktuelles Layout des Benutzers unter einem bestimmten Namen speichern.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// In Single-User-Apps (keine User-ID) entspricht diese Methode der SaveLayout-Methode.
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="name">Name des zu speichernden Layouts</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult SaveLayoutAs(this TileView view, string name, string? extName = null)
    {
        Guid? ident = view.GetGridIdentifier();

        if (string.IsNullOrWhiteSpace(name))
            return CommandResult.Error("Es wurde kein Name angegeben.");

        if (!supportLayout(ident))
            return CommandResult.None;

        AFCore.App.Persistance?.Set((Guid)ident!, view.GetTemplate().ToJsonBytes(), name: name, extName: extName);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTSAVED);
    }

    /// <summary>
    /// Aktuelles Layout des GridViews für den aktuellen Benutzer speichern.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notwendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult SaveLayout(this TileView view, string? extName = null)
    {
        Guid? ident = view.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        AFCore.App.Persistance?.Set((Guid)ident!, view.GetTemplate().ToJsonBytes(), extName: extName);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTSAVED);
    }

    /// <summary>
    /// Aktuelles Layout des GridViews als Standardlayout für alle Benutzer speichern.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notwendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult SaveDefaultLayout(this TileView view, string? extName = null)
    {
        Guid? ident = view.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        AFCore.App.Persistance?.Set((Guid)ident!, view.GetTemplate().ToJsonBytes(), userid: Guid.Empty, extName: extName);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTSAVED);
    }

    /// <summary>
    /// Gespeichertes Standardlayout des Grids löschen (wenn vorhanden)
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notwendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult DeleteDefaultLayout(this TileView view, string? extName = null)
    {
        Guid? ident = view.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        AFCore.App.Persistance?.Delete((Guid)ident!, userid: Guid.Empty, extName: extName);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTRESETED);
    }

    /// <summary>
    /// Gespeichertes Standardlayout des Benutzers löschen (wenn vorhanden)
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notwendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult DeleteUserLayout(this TileView view, string? extName = null)
    {
        Guid? ident = view.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        AFCore.App.Persistance?.Delete((Guid)ident!, extName: extName);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTRESETED);
    }

    /// <summary>
    /// Standardlayout des Grids laden (ignoriert alle persistierten Layouts)
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notwendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="view">TileView</param>
    public static CommandResult ResetLayout(this TileView view)
    {
        if (view.Tag is not AFGridViewState state || state.DefaultLayout == null) return CommandResult.None;

        RestoreLayout(view, state.DefaultLayout);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTRESETED);
    }

    /// <summary>
    /// Standardlayout für Benutzer laden.
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    /// <returns>Ergebnis</returns>
    public static CommandResult LoadDefaultLayout(this TileView view, string? extName = null)
    {
        if (view.GetGridIdentifier() is not { } id || id == Guid.Empty) return CommandResult.None;

        var data = AFCore.App.Persistance?.Get(id, extName: extName);

        if (data != null)
            view.RestoreLayout(data);
        else
        {
            if (view.Tag is not AFGridViewState state || state.DefaultLayout == null) return CommandResult.None;

            RestoreLayout(view, state.DefaultLayout);
        }

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTLOADED);
    }

    /// <summary>
    /// Standardlayout für Benutzer laden.
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    /// <param name="name">Name des zu ladenden Layouts</param>
    /// <returns>Ergebnis</returns>
    public static CommandResult LoadLayout(this TileView view, string name, string? extName = null)
    {
        if (view.GetGridIdentifier() is not { } id || id == Guid.Empty) return CommandResult.None;

        if (string.IsNullOrWhiteSpace(name)) return CommandResult.Warning("Es wurde kein Name für das zu landende Layout übergeben.");

        var data = AFCore.App.Persistance?.Get(id, name: name, extName: extName);

        if (data != null)
            view.RestoreLayout(data);
        else
            return CommandResult.Warning($"Das Layout mit dem Namen '{name}' wurde nicht gefunden.");

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTLOADED);
    }

    /// <summary>
    /// Layout des TileViews mit einer bestimmten ID (SYS_ID) laden.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notwendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="id">ID des Layouts (SYS_ID)</param>
    public static CommandResult LoadLayout(this TileView view, Guid id)
    {
        var ident = view.GetGridIdentifier();

        if (!supportLayout(ident))
            return CommandResult.None;

        var data = AFCore.App.Persistance?.GetByID(id);

        if (data == null)
            return CommandResult.None;

        view.RestoreLayout(data);

        return CommandResult.Info(Properties.Resources.LBL_LAYOUTLOADED);
    }

    /// <summary>
    /// Layouts des TileViews für den aktuellen Benutzer verwalten.
    /// 
    /// Voraussetzung ist die Verwendung der AF-Persistance (AFCore.App.Persistance). 
    /// Es ist nicht zwingend notendig, dass eine User-ID gesetzt ist (z.B. in Anwendungen ohne Userverwaltung/Single-User-Apps).
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="extName">Name des Layouts (KeyName)</param>
    public static CommandResult ManageLayouts(this TileView view, string? extName = null)
    {
        var ident = view.GetGridIdentifier();

        if (ident == null || !supportLayout(ident))
            return CommandResult.None;

        using var dlg = new FormLayoutManager("Layouts der Ansicht verwalten", (Guid)ident, extName);

        if (dlg.ShowDialog(view.GridControl.FindForm()) == DialogResult.OK && dlg.SelectedLayout != null)
            return view.LoadLayout((Guid)dlg.SelectedLayout);

        return CommandResult.None;
    }

    /// <summary>
    /// Template aus AFHtmlTemplate setzen
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="template">Template</param>
    public static void SetTemplate(this TileView view, AFHtmlTemplate template)
    {
        view.TileHtmlTemplate.Template = template.HtmlTemplate; 
        view.TileHtmlTemplate.Styles = template.CssTemplate;
    }

    /// <summary>
    /// Layout aus serialisiertem AFHtmlTemplate setzen
    /// </summary>
    /// <param name="view">TileView</param>
    /// <param name="data">Serialisiertes Layout/Template</param>
    public static void RestoreLayout(this TileView view, byte[] data)
    {
        if (Functions.DeserializeJsonBytes<AFHtmlTemplate>(data) is not { } template) return;

        SetTemplate(view, template);
    }

    private static bool supportLayout(Guid? ident)
    {
        if (ident == null || ident.Equals(Guid.Empty))
            return false;

        // prüfen ob Speicherung überhaupt möglich ist...
        if (AFCore.App.Persistance == null || AFCore.App.Persistance.IsAvailable == false)
            return false;

        return true;
    }
}