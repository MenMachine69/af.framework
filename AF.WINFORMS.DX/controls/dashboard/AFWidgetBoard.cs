using DevExpress.Utils;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.ButtonPanel;

namespace AF.WINFORMS.DX;

/// <summary>
/// Control zur Darstellung eines Boards mit Widgets (z.B. Dashboard etc.)
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFWidgetBoard : AFUserControl
{
    private readonly DocumentManager manager = null!;
    private readonly WidgetView widgets = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rows">Zeilen</param>
    /// <param name="columns">Spalten</param>
    /// <param name="space">Elementabstand</param>
    public AFWidgetBoard(int rows, int columns, int space)
    {
        if (UI.DesignMode) return;

        widgets = new();
        widgets.LayoutMode = LayoutMode.TableLayout;
        widgets.RootContainer.Orientation = Orientation.Vertical;
        manager = new() { ContainerControl = this, View = widgets };
        manager.ViewCollection.Add( widgets );

        Setup(rows, columns, space);
    }

    /// <summary>
    /// Setup des WidgetBoards.
    /// 
    /// Alle Elemente entfernen wenn notwendig und Konfiguration der Zeien, 
    /// Spalten und Abstände anhand der übergebenen Werte.
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    /// <param name="space"></param>
    /// <exception cref="Exception"></exception>
    public void Setup(int rows, int columns, int space)
    {

        while (widgets.Documents.Count > 0)
        {
            var toremove = widgets.Documents.Last()?.Control;

            if (toremove == null)
                throw new Exception("Widget-Dokument ohne Control gefunden!");

            widgets.RemoveDocument(toremove);

            // TODO: Prüfen ob Dispose notwendig ist!
            // toremove.Dispose();
        }

        widgets.Documents.Clear();

        if (widgets.DocumentSpacing == space &&
            widgets.Columns.Count == columns &&
            widgets.Rows.Count == rows) return;

        widgets.Columns.Clear();
        widgets.Rows.Clear();

        widgets.DocumentSpacing = space;

        for (int i = 0; i < columns; ++i)
            widgets.Columns.Add(new());

        for (int i = 0; i < rows; ++i)
            widgets.Rows.Add(new());
    }


    /// <summary>
    /// Zugriff auf das WidgetView
    /// </summary>
    public WidgetView View => widgets;

    /// <summary>
    /// Zugriff auf den Documentmanager
    /// </summary>
    public DocumentManager Manager => manager;

    /// <summary>
    /// Ein Element/Widget hinzufügen
    /// </summary>
    /// <param name="ctrl">Control das als Widget angezeigt werden soll widget</param>
    /// <param name="caption">caption for this panel</param>
    /// <param name="row">Zeile (1-basiert)</param>
    /// <param name="col">Spalte (1-basiert)</param>
    /// <param name="rowspan">Zeilen</param>
    /// <param name="colspan">Spalten</param>
    public T AddWidget<T>(T ctrl, string caption, int row, int col, int rowspan, int colspan) where T : Control
    {
        Document document = (Document)widgets.AddDocument(ctrl);
        document.RowIndex = row - 1;
        document.RowSpan = rowspan;
        document.ColumnIndex = col - 1;
        document.ColumnSpan = colspan;
        document.Caption = caption;
        document.Properties.AllowClose = DefaultBoolean.False;
        document.Properties.AllowGlyphSkinning = DefaultBoolean.True;
        document.Properties.AllowFloat = DefaultBoolean.False;

        if (ctrl is not AFPresenterBase presenter) return ctrl;

        if (presenter.SetupElement != null)
        {
            var btn = new CustomHeaderButton("", ButtonStyle.PushButton);
            btn.Tag = "Setup";
            ((IBaseButton)btn).Properties.ImageOptions.SvgImage = UI.GetImage(Symbol.Wrench);
            ((IBaseButton)btn).Properties.ImageOptions.SvgImageSize = new(12, 12);
            ((IBaseButton)btn).Properties.UseCaption = false;
            ((IBaseButton)btn).Properties.UseImage = true;
            ((IBaseButton)btn).Properties.ToolTip = "Einstellungen bearbeiten";

            document.CustomHeaderButtons.Add(btn);
        }

        if (presenter.RefreshContent != null)
        {
            var btn = new CustomHeaderButton("", ButtonStyle.PushButton);
            btn.Tag = "Refresh";
            ((IBaseButton)btn).Properties.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowSync);
            ((IBaseButton)btn).Properties.ImageOptions.SvgImageSize = new(12, 12);
            ((IBaseButton)btn).Properties.UseCaption = false;
            ((IBaseButton)btn).Properties.UseImage = true;
            ((IBaseButton)btn).Properties.ToolTip = "Anzeige aktualisieren";

            document.CustomHeaderButtons.Add(btn);
        }


        document.CustomButtonClick += (_, e) =>
        {
            if (document.Control is not AFPresenterBase element) return;

            if (e.Button is not CustomHeaderButton button) return;

            if (button.Tag is not string name) return;

            if (name == "Refresh")
                element.RefreshContent?.Invoke(element);

            if (name == "Setup")
                element.SetupElement?.Invoke(element);

        };

        return ctrl;
    }
}

