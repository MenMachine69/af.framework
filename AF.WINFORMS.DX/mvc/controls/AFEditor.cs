using AF.MVC;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Basisklasse der IEditor Klassen in WinForms.
/// </summary>
public class AFEditorBase : AFUserControl, IEditor
{
    private int _defaultEditorHeight = 400;
    private int _defaultEditorWidth = 800;

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool SupportPages { get; set; }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual bool HideCaption { get; set; }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual IModel? Model { get; set; }

    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IViewPage? ParentPage { get; set; }

    /// <summary>
    /// Vom Editor benötigte Standardbreite (in Pixel).
    ///
    /// Der Wert wird bei High-DPI Bildschirmen automatisch skaliert.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DefaultEditorWidth
    {
        get => ScaleUtils.ScaleValue(_defaultEditorWidth);
        set => _defaultEditorWidth = value;
    }

    /// <summary>
    /// Aktueller ViewContext des Editors.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ViewContext { get; set; } = string.Empty;

    /// <summary>
    /// Vom Editor benötigte Standardhöhe (in Pixel).
    ///
    /// Der Wert wird bei High-DPI Bildschirmen automatisch skaliert.
    /// In der Regel wird die Höhe automatisch anhand des Inhalts ermittelt.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DefaultEditorHeight
    {
        get => ScaleUtils.ScaleValue(_defaultEditorHeight);
        set => _defaultEditorHeight = value;
    }
    
    /// <inheritdoc />
    public virtual void AfterLayout(object sender) {  }

    /// <inheritdoc />
    public virtual bool IsValid(ValidationErrorCollection errors) { return true; }

    /// <inheritdoc />
    public virtual void RegisterControl(object control) { }

    /// <inheritdoc />
    public virtual void UnRegisterControl(object control) { }

    /// <inheritdoc />
    public virtual EditorPageInfo[] GetPages() { return []; }

    /// <inheritdoc />
    public virtual void SelectPage(int pageindex) { }

    /// <inheritdoc />
    public virtual void RefreshDatasource() { }

    /// <inheritdoc />
    public virtual eMessageBoxResult ConfirmClosing() => eMessageBoxResult.No;
}

/// <summary>
/// Editor, der als Anzeige und/oder Editor für ein IModel verwendet werden kann.
/// 
/// Dies ist eine Basisklasse, und der konkrete Editor muss von dieser Klasse geerbt werden.
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public class AFEditor : AFEditorBase
{
    private AFBindingConnector? connector;
    private IContainer? components;
    private AFErrorProvider? errProvider;
    private BarManager? barmanager = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            barmanager?.Dispose();
        }
        base.Dispose(disposing);
    }


    /// <summary>
    /// Wird ausgeführt, wenn der Editor zerstört wird.
    /// 
    /// Hier werden die registrieren Controls vn den zugeordneten Events abgemeldet.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        foreach (Control subctrl in Controls)
            unregisterControl(subctrl);
        
        base.OnHandleDestroyed(e);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditor()
    {
        if (UI.DesignMode) return;

        components = new Container();
        connector = new(components) { ContainerControl = this };
    }

    /// <summary>
    /// Zugriff auf den Connector...
    /// </summary>
    public AFBindingConnector? Connector => connector;

    /// <summary>
    /// current model
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override IModel? Model
    {
        get => connector?.DataSource as IModel;
        set
        {
            connector!.DataSource = value;
            (connector!.DataSource as IModel)?.CommitChanges();
        }
    }
        
    /// <summary>
    /// Methode, die nach Abschluss des Layouts aufgerufen wird (EndLayout).
    /// </summary>
    /// <param name="sender"></param>
    public override void AfterLayout(object sender)
    {
        if (sender is not Control ctrl) return;

        if (ctrl is AFTablePanel table)
        {
            if (table.Dock == DockStyle.Fill)
                Dock = DockStyle.Fill;
            else
                Size = new Size(Width, table.Height + table.Padding.Vertical);
        }

        DefaultEditorHeight = Size.Height;

        foreach (Control subctrl in Controls)
            registerControl(subctrl);

    }

    private void registerControl(Control ctrl)
    {
        if (ctrl.Controls.Count > 0)
        {
            foreach (Control subctrl in ctrl.Controls)
                registerControl(subctrl);

            RegisterControl(ctrl);
        }
        else
            RegisterControl(ctrl);
    }

    private void unregisterControl(Control ctrl)
    {
        if (ctrl.Controls.Count > 0)
        {
            foreach (Control subctrl in ctrl.Controls)
                unregisterControl(subctrl);
        }
        else
            UnRegisterControl(ctrl);
    }


    /// <summary>
    /// Registriertes Control abmelden
    /// </summary>
    /// <param name="control"></param>
    public override void UnRegisterControl(object control)
    {
        if (control is not Control ctrl) return;

        if (ctrl is BaseEdit edit)
            edit.Enter -= onEnter;
    }

    /// <summary>
    /// Registrieren eines Controls im Editor
    /// </summary>
    /// <param name="control"></param>
    public override void RegisterControl(object control)
    {
        if (control is not Control ctrl) return;

        if (ctrl is BaseEdit edit)
        {
            edit.Properties.AppearanceFocused.From(UI.DefaultStyleController.AppearanceFocused);
            edit.Properties.Appearance.From(UI.DefaultStyleController.Appearance);
            ctrl.Enter += onEnter;
        }
    }

    private void onEnter(object? sender, EventArgs e)
    {
        if (sender is not BaseEdit edit) return;

        edit.Properties.AppearanceFocused.BackColor = edit.Properties.ReadOnly
            ? UI.DefaultStyleController.AppearanceReadOnly.BackColor
            : UI.DefaultStyleController.AppearanceFocused.BackColor;
        edit.Properties.AppearanceFocused.ForeColor = edit.Properties.ReadOnly
            ? UI.DefaultStyleController.AppearanceReadOnly.ForeColor
            : UI.DefaultStyleController.AppearanceFocused.ForeColor;
    }


    /// <inheritdoc />
    public override bool IsValid(ValidationErrorCollection errors)
    {
        errProvider?.ClearErrors();

        if (!Validate()) return false;

        var valid = Model?.IsValid(errors) ?? true;

        if (valid) return valid;

        errProvider ??= new(components) { ContainerControl = this };
        errProvider.FromCollection(errors);

        return valid;
    }

    /// <inheritdoc />
    public override void RefreshDatasource()
    {
        connector?.ResetBindings(false);
    }

    /// <inheritdoc />
    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);

        if (UI.DesignMode) return;

        if (e.Control is AFTablePanel table)
        {
            table.ShowOverflow = true;
            table.Dock = DockStyle.Fill;
        }
    }


    /// <summary>
    /// Popupmenu für Elemente anzeigen (z.B. aus HTML-Template heraus)
    /// </summary>
    /// <param name="showAt">Position</param>
    /// <param name="controller">Controller, der die Commands für das Menü zur Verfügung stellt.</param>
    /// <param name="buffer">Puffer für as Menü (wenn != null, wird das Menü direkt angezeigt - erspart mehrfaches instantieren)</param>
    /// <param name="onclick">wird ausgeführt, wenn der Benutzer einen Menüpunkt anklickt - sender ist das AFCommand-Objekt</param>
    /// <param name="commandcontext">Kontext, dessen Commands benötigt werden (Standard ist DetailContext)</param>
    /// <param name="viewcontext">Kontext des Views/der Anzeige um nur bestimmte, zur aktuellen Anzeige passende Commands anzuzeigen (Standard ist leer = alle anzeige)</param>
    /// <param name="model">Datenobjekt, für das das PopupMenu gelten soll</param>
    /// <exception cref="NotSupportedException">Fehler, wenn der Editor NICHT in einer AFPage angezeigt wird</exception>
    public void ShowPopupMenu(Point showAt, IControllerUI controller, ref PopupMenu? buffer, ItemClickEventHandler onclick, eCommandContext commandcontext = eCommandContext.DetailContext, string viewcontext = "", IModel? model = null)
    {
        if (buffer == null)
        {
            AFPage? page = this.GetParentControl<AFPage>();

            buffer = controller.GetDetailsPopupMenu(page?.BarManager ?? getBarmanager(), onclick, commandcontext, viewcontext, model);
        }

        if (buffer == null) return;

        buffer.ShowPopup(showAt);
    }

    private BarManager getBarmanager()
    {
        barmanager ??= new() { Form = FindForm() }; ;

        return barmanager;
    }
}


/// <summary>
/// Control zur Anzeige von Informationen zu einem Model.
/// 
/// Das Control benötigt KEINEN eigenen AFBindingConnector, 
/// dieser wird intern automatisch generiert!
/// 
/// Dem Control wird das anzuzeigende Model via Model-Property zugewiesen.
/// Die Methode Goto kann aufgerufen werden, um das angezeigte Model zu 
/// öffnen/anzuzeigen (Gehe zu).
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public class AFDetail : AFUserControl, IViewModelDetail
{
    private AFBindingConnector? connector;
    private BarManager? barmanager;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            barmanager?.Dispose();
        }
        base.Dispose(disposing);
    }


    /// <inheritdoc />
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual IModel? Model 
    { 
        get => connector?.DataSource as IModel;
        set 
        {
            if (value == null)
            {
                if (connector != null)
                    connector.DataSource = null;

                return;
            }

            connector ??= new(Container) { StartContainerControl = this };

            connector.DataSource = value;

        } 
    }

    /// <inheritdoc />
    public void Goto()
    {
        if (Model == null) return;

        //UI.ViewManager.Show(Model);
    }

    /// <inheritdoc />
    public void Update(IViewDetail detail)
    {
        Model = detail.CurrentModel;
    }

    /// <summary>
    /// Popupmenu für Elemente anzeigen (z.B. aus HTML-Template heraus)
    /// </summary>
    /// <param name="showAt">Position</param>
    /// <param name="controller">Controller, der die Commands für das Menü zur Verfügung stellt.</param>
    /// <param name="buffer">Puffer, der das Menu zwischenspeichert</param>
    /// <param name="onclick">wird ausgeführt, wenn der Benutzer einen Menüpunkt anklickt - sender ist das AFCommand-Objekt</param>
    /// <param name="commandcontext">Kontext, dessen Commands benötigt werden (Standard ist DetailContext)</param>
    /// <param name="viewcontext">Kontext des Views/der Anzeige um nur bestimmte, zur aktuellen Anzeige passende Commands anzuzeigen (Standard ist leer = alle anzeige)</param>
    /// <param name="model">Datenobjekt, für das das PopupMenu gelten soll</param>
    /// <exception cref="NotSupportedException">Fehler, wenn der Editor NICHT in einer AFPage angezeigt wird</exception>
    public void ShowPopupMenu(Point showAt, IControllerUI controller, ref PopupMenu? buffer, ItemClickEventHandler onclick, eCommandContext commandcontext = eCommandContext.DetailContext, string viewcontext = "", IModel? model = null)
    {
        if (buffer == null)
        {
            AFPage? page = this.GetParentControl<AFPage>();
            
            buffer = controller.GetDetailsPopupMenu(page?.BarManager ?? getBarmanager(), onclick, commandcontext, viewcontext, model);
        }

        if (buffer == null) return;

        buffer.ShowPopup(showAt);
    }

    private BarManager getBarmanager()
    {
        barmanager ??= new() { Form = FindForm() };

        return barmanager;
    }
}

/// <summary>
/// Control zur Anzeige von Informationen zu einem Model.
/// 
/// Dem Control kann das anzuzeigende Model via Model-Property zugewiesen werden.
/// Wird KEIN Model zugewiesen, hat das Control auch keinen BindingConnector, sondern 
/// fügt sich in das Binding des übergeordneten Controls ein.
/// 
/// Die Methode Goto kann aufgerufen werden, um das angezeigte Model zu 
/// öffnen/anzuzeigen (Gehe zu). Diese Methode ist nur verfügbar, wenn ein Model zugewiesen wird.
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
public class AFInfo : AFUserControl
{
    private AFBindingConnector? connector;

    /// <summary>
    /// Connector, der vom Detail verwendet wird (!= null wenn Model zugewiesen wurde).
    /// </summary>
    public AFBindingConnector? Connector => connector;

    /// <summary>
    /// Model das angezeigt werden soll.
    /// 
    /// Kann zugewiesen werden, wenn im Detail NICHT das Model des übergeordneten Controls angezeigt werden soll.
    /// Wird ein Model zugewiesen, verwendet das Control einen eigenen BindingConnector.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual IModel? Model
    {
        get => connector?.DataSource as IModel;
        set
        {
            if (value == null)
            {
                if (connector != null)
                    connector.DataSource = null;

                return;
            }

            connector ??= new(Container) { StartContainerControl = this };

            connector.DataSource = value;

        }
    }

    /// <summary>
    /// Zugewiesenes Model öffnen
    /// </summary>
    public void Goto()
    {
        if (Model == null) return;

        UI.ViewManager.OpenPage(Model.ModelLink);
    }

    /// <summary>
    /// Methode die vom übergeordneten AFDetail aufgerufen wird,
    /// wenn dort das IModel gesetzt wurde.
    /// </summary>
    /// <param name="detail">AFDetail, in dem das Model geändert wurde und dass das CRInfo enthält.</param>
    public virtual void Update(IViewModelDetail detail)
    {
        Model = detail.Model;
    }

}