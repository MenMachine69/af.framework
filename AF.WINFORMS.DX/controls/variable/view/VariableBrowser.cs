using AF.MVC;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraGrid.Views.Grid;

namespace AF.WINFORMS.DX;

/// <summary>
/// Browser für IVariable-Elemente
/// </summary>
/// <typeparam name="TVariable"></typeparam>
[DesignerCategory("Code")]
public class VariableBrowser<TVariable> : VariableBrowserUI where TVariable : class, IVariable
{
    private WeakEvent<EventHandler<EventArgs>>? elementListChanged;
    private readonly IDisposable dragDropBehavior = null!;
    private readonly BehaviorManager behaviorManager1 = null!;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public VariableBrowser()
    {
        if (UI.DesignMode) return;

        behaviorManager1 = new(components);

        mleInfo.AllowHtmlString = true;
        mleInfo.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        mleInfo.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
        mleInfo.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
        mleInfo.Padding = new(5);
        mleInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;

        var setup = viewVariableBrowser.GetSetup(typeof(TVariable));
        foreach (var col in setup.Columns)
        {
            col.AllowSort = false;
            col.AllowMove = false;
        }
        setup.SortOn = nameof(Variable.VAR_PRIORITY);
        setup.SortOrder = eOrderMode.Ascending;
        viewVariableBrowser.Setup(setup);
        viewVariableBrowser.OptionsCustomization.AllowSort = false;
        viewVariableBrowser.OptionsMenu.EnableColumnMenu = false;
        viewVariableBrowser.OptionsMenu.EnableFooterMenu = false;
        viewVariableBrowser.OptionsMenu.EnableGroupPanelMenu = false;
        viewVariableBrowser.OptionsMenu.EnableGroupRowMenu = false;

        dragDropBehavior = behaviorManager1.Attach<DragDropBehavior>(viewVariableBrowser, behavior =>
        {
            behavior.Properties.AllowDrop = true;
            behavior.Properties.InsertIndicatorVisible = true;
            behavior.Properties.PreviewVisible = true;
            behavior.DragDrop += (_, e) =>
            {
                var data = (e.Data as int[])?[0] ?? -1;

                if (data >= 0)
                {
                    var src = viewVariableBrowser.GetRow(viewVariableBrowser.GetVisibleRowHandle(data)) as TVariable;
                    var tar = viewVariableBrowser.GetRowFromLocation(e.Location) as TVariable;

                    if (tar != null && src != null && tar.VAR_PRIORITY != src.VAR_PRIORITY)
                    {
                        int direction = tar.VAR_PRIORITY < src.VAR_PRIORITY ? 1 : 0;

                        int fromrow = src.VAR_PRIORITY;
                        int torow = tar.VAR_PRIORITY;

                        (gridVariableBrowser.DataSource as BindingList<TVariable>)!.RaiseListChangedEvents = false;

                        foreach (var row in (gridVariableBrowser.DataSource as BindingList<TVariable>)!)
                        {
                            if (direction == 1 && row.VAR_PRIORITY >= torow && row.VAR_PRIORITY <= fromrow)
                                row.VAR_PRIORITY += 1;
                            else if (direction == 0 && row.VAR_PRIORITY > fromrow && row.VAR_PRIORITY <= torow)
                                row.VAR_PRIORITY -= 1;
                        }

                        (gridVariableBrowser.DataSource as BindingList<TVariable>)!.RaiseListChangedEvents = true;
                        src.VAR_PRIORITY = torow;

                        elementListChanged?.Raise(this, EventArgs.Empty);
                    }
                }

                e.Handled = true;
            };
        });

        Variables = [];

        menNew.SetSymbol(Symbol.AddCircle);
        menPreview.SetSymbol(Symbol.Search);
        menMore.SetSymbol(Symbol.MoreVertical);

        viewVariableBrowser.FocusedRowChanged += (_, _) =>
        {
            var row = viewVariableBrowser.GetFocusedRow() as IVariable;

            if (row == null) return;

            mleInfo.Text = $"<b>{row.VAR_NAME}</b> ({row.VAR_TYP})\r\n{row.VAR_DESCRIPTION}";
        };

        menNew.ItemClick += (_, _) =>
        {
            HandleResult(typeof(TVariable).GetUIController()!.GetCommand(eCommand.New)!.Execute(new CommandArgs() { CommandContext = eCommandContext.GridButton, CommandSource = this, ParentControl = this}));
        };

        menCorrect.ItemClick += (_, _) =>
        {
            if (MsgBox.ShowQuestionYesNo("REIHENFOLGE KORRIGIEREN\r\nMöchten Sie die Reihenfolge der Elemente in der Liste jetzt korrigieren?") == eMessageBoxResult.No) return;

            int pos = 1;
            foreach (var ele in Variables.OrderBy(v => v.VAR_PRIORITY))
            {
                ele.VAR_PRIORITY = pos;
                pos++;
            }

            elementListChanged?.Raise(this, EventArgs.Empty);
        };

        menPreview.ItemClick += (_, _) =>
        {
            if (Variables.Count < 1 || Variables.Count(v => v.VAR_READONLY == false) < 1)
                HandleResult(CommandResult.Warning("Keine Variablen für Preview definiert (leer oder nur ReadOnly)."));

            using AFFormVariableFormular form = new(Variables);
            form.ShowDialog(FindForm());
        };
    }

    /// <summary>
    /// Liste der Variablen im Browser
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public BindingList<TVariable> Variables
    {
        get => (BindingList<TVariable>)gridVariableBrowser.DataSource;
        set { gridVariableBrowser.DataSource = value; gridVariableBrowser.RefreshDataSource(); }
    }

    /// <summary>
    /// Zugriff auf das GridView der Variablen
    /// </summary>
    public GridView ViewVariablen => viewVariableBrowser;

    /// <summary>
    /// Zugriff auf das Grid der Variablen
    /// </summary>
    public AFGridControl GridVariablen => gridVariableBrowser;


    /// <summary>
    /// Variablendetails können die Variable über dieses Event benachrichtigen, 
    /// so dass die Variable reagieren kann.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public event EventHandler<EventArgs> OnElementListChanged
    {
        add
        {
            elementListChanged ??= new();
            elementListChanged.Add(value);
        }
        remove => elementListChanged?.Remove(value);
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        dragDropBehavior.Dispose();
        base.OnHandleDestroyed(e);
    }
    
    /// <summary>
    /// Element wurde hinzugefügt
    /// </summary>
    /// <param name="element"></param>
    public void ElementAdded(TVariable element)
    {
       
        if (element.VAR_PRIORITY < 1)
            element.VAR_PRIORITY = 1;

        if (element.VAR_PRIORITY > Variables.Count)
            element.VAR_PRIORITY = Variables.Count + 1;
        else
        {
            foreach (var ele in Variables)
            {
                if (ele.VAR_PRIORITY >= element.VAR_PRIORITY)
                    ele.VAR_PRIORITY += 1;
            }
        }

        Variables.Add(element);
       
        elementListChanged?.Raise(this, EventArgs.Empty);
    }

    /// <summary>
    /// Element wurde geändert
    /// </summary>
    /// <param name="element"></param>
    public void ElementChanged(TVariable element)
    {
        // TODO: Reihenfolge anpassen
        if (element.VAR_PRIORITY > Variables.Count)
            element.VAR_PRIORITY = Variables.Count;

        if (element.VAR_PRIORITY < 1)
            element.VAR_PRIORITY = 1;

        int posNeeded = 1;
        int anomalie = 0;

        foreach (var ele in Variables.OrderBy(v => v.VAR_PRIORITY))
        {
            if (ele.VAR_PRIORITY == posNeeded)
            {
                ++posNeeded;
                continue;
            }

            if (ele == element)
                continue;

            if (ele.VAR_PRIORITY >  posNeeded || ele.VAR_PRIORITY < posNeeded)
            {
                anomalie = posNeeded; break;
            }

            ++posNeeded;
        }

        if (anomalie > element.VAR_PRIORITY) // nach unten
        {
            foreach (var ele in Variables.OrderBy(v => v.VAR_PRIORITY))
            {
                if (ele.VAR_PRIORITY > anomalie) break;

                if (ele.VAR_PRIORITY >= element.VAR_PRIORITY)
                {
                    if (ele != element)
                        ele.VAR_PRIORITY += 1;
                }
            }
        }
        else if (anomalie < element.VAR_PRIORITY) // nach oben
        {
            foreach (var ele in Variables.OrderBy(v => v.VAR_PRIORITY))
            {
                if (ele.VAR_PRIORITY > element.VAR_PRIORITY) break;

                if (ele.VAR_PRIORITY >= anomalie)
                {
                    if (ele != element)
                        ele.VAR_PRIORITY -= 1;
                }
            }
        }

        elementListChanged?.Raise(this, EventArgs.Empty);
    }

    /// <summary>
    /// Element wurde entfernt
    /// </summary>
    /// <param name="element"></param>
    public void ElementRemoved(TVariable element)
    {
        int pos = 1;
        
        foreach (var ele in Variables.OrderBy(v => v.VAR_PRIORITY))
        {
            ele.VAR_PRIORITY = pos;
            ++pos;
        }

        elementListChanged?.Raise(this, EventArgs.Empty);
    }
}