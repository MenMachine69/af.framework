using AF.MVC;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <summary>
/// UI-Controller der Variablen
/// </summary>
public class VariableControllerUI : VariableController, IControllerUI<Variable>
{
    private static VariableControllerUI? instance;

    /// <summary>
    /// Zugriff auf die Instanz des Controllers (Singleton).
    /// </summary>
    public new static VariableControllerUI Instance => instance ??= new();

    private VariableControllerUI() { }

    /// <summary>
    /// Gibt an, ob mehrere Pages für das gleiche Model geöffnet werden können.
    /// </summary>
    public bool AllowMultiplePages => false;

    /// <summary>
    /// Bild (SVG, Bitmap usw.), das den Typ in einer Benutzeroberfläche darstellt.
    /// </summary>
    public override object TypeImage => UI.GetObjectImage(ObjectImages.tag);

    /// <summary>
    /// Liefert ein UIElement oder NULL, wenn der Controller das Element nicht zur Verfügung stellt.
    /// </summary>
    /// <param name="type">Typ des benötigten UIElements</param>
    /// <param name="mastertype">Typ des Masters, der gerade aktiv ist</param>
    /// <param name="detailtype">Typ der Details</param>
    /// <param name="page">Page, die das Element benötigt. 
    /// Bei einigen UI-Elementen muss hier das Page-Objekt übergeben werden! (Detail, PluginDetail, FooterDetail, HeaderDetail, Editor)(</param>
    /// <returns>das UI Element oder NULL</returns>
    public override IUIElement? GetUIElement(eUIElement type, Type? mastertype = null, Type? detailtype = null, IViewPage? page = null)
    {
        if (type == eUIElement.Editor) return new VariableEditor();

        return base.GetUIElement(type, mastertype, detailtype, page);
    }

    /// <summary>
    /// Liefert den Typen eines UIElement oder NULL, wenn der Controller das Element nicht zur Verfügung stellt.
    /// </summary>
    /// <param name="type">Typ des benötigten UIElements</param>
    /// <param name="mastertype">Typ des Masters, der gerade aktiv ist</param>
    /// <param name="detailtype">Typ der Details</param>
    /// <param name="page">Page, die das Element benötigt. 
    /// Bei einigen UI-Elementen muss hier das Page-Objekt übergeben werden! (Detail, PluginDetail, FooterDetail, HeaderDetail, Editor)(</param>
    /// <returns>Type des UI Element oder NULL</returns>
    public override Type? GetUIElementType(eUIElement type, Type? mastertype = null, Type? detailtype = null, IViewPage? page = null)
    {
        if (type == eUIElement.Editor) return typeof(VariableEditor);

        return base.GetUIElementType(type, mastertype, detailtype, page);
    }

    /// <summary>
    /// Diese Methode wird aufgerufen, nachdem ein UI-Element erstellt wurde. Hier können Sie Anpassungen am UIElement vornehmen (z.B. Gittereinstellungen etc.) 
    /// </summary>
    /// <param name="element">das UIElement</param>
    /// <param name="page">die Ansicht, die das Element enthält</param>
    public void SetupUIElement(IUIElement element, IViewPage page)
    {
        if (this is IControllerUI<Variable> uictrl)
            uictrl.SetupUIElement(element, page);
    }



    /// <summary>
    /// Eine neue Variable erzeugen
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    [AFCommand("NEUE VARIABLE", CommandContext = eCommandContext.MasterContext | eCommandContext.ComboBox,
        CommandType = eCommand.New)]
    public CommandResult CmdNew(CommandArgs data)
    {
        var eigenschaft = Create();

        return processNew(eigenschaft, data);
    }


    private CommandResult processNew(Variable variable, CommandArgs data)
    {
        if (data.CommandContext == eCommandContext.MasterContext || data.CommandContext == eCommandContext.Browser)
            UI.ViewManager.OpenPage(variable);
        else
        {
            using AFEditorForm<Variable> editor = new(variable, "NEUE VARIABLE");

            if (editor.ShowDialog((IWin32Window)UI.Shell) == DialogResult.OK)
                return editor.CommandResult ?? CommandResult.None;
        }

        return CommandResult.None;
    }

    /// <summary>
    /// Eine Variable bearbeiten (Dialog)
    /// </summary>
    /// <param name="data">Parameter</param>
    /// <returns>Resultat</returns>
    [AFCommand("VARIABLE BEARBEITEN", CommandContext = eCommandContext.EveryWhere, CommandType = eCommand.Edit)]
    public CommandResult CmdEdit(CommandArgs data)
    {
        if (data.Model == null) return CommandResult.None;

        var eigenschaft = data.Model as Variable;

        if (eigenschaft == null) return CommandResult.Error("Es wurde kein Variable-Objekt übergeben.");

        using AFEditorForm<Variable> editor = new(eigenschaft, "VARIABLE BEARBEITEN");

        if (editor.ShowDialog((IWin32Window)UI.Shell) == DialogResult.OK)
            return editor.CommandResult ?? CommandResult.None;

        return CommandResult.None;
    }

    #region Eingabeformular
 
    /// <summary>
    /// Liefert den passenden Editor für eine Variable
    /// </summary>
    /// <param name="variable">IVariable, für die der Editor benötigt wird</param>
    /// <returns>Control, dass als Editor verwendet wird</returns>
    public Control? GetEditor(IVariable variable)
    {
        VariableBase detail = variable.GetVariable();

        if (variable.VAR_TYP == (int)eVariableType.Section)
            return null;

        switch (variable.VAR_TYP)
        {
            case (int)eVariableType.Bool:
                var detailBool = detail as VariableBool;
                return new AFEditToggle()
                {
                    Name = variable.VAR_NAME,
                    Dock = DockStyle.Left,
                    IsOn = detailBool?.Default ?? false,
                    Properties ={
                        AutoWidth = true,
                        OffText = detailBool?.DisplayStringOff ?? "ja",
                        OnText = detailBool?.DisplayStringOn ?? "nein"
                    }
                };
            case (int)eVariableType.Int:
                var detailInt = detail as VariableInt;
                return new AFEditSpinInt()
                {
                    Name = variable.VAR_NAME,
                    Value = detailInt?.Default ?? 0,
                    Properties ={
                        MinValue = detailInt?.Minimum ?? 0,
                        MaxValue = detailInt?.Maximum ?? int.MaxValue,
                        DisplayFormat = {
                            FormatType = FormatType.Numeric,
                            FormatString = detailInt?.DisplayMask ?? "f0"
                        },
                        EditFormat = {
                            FormatType = FormatType.Numeric,
                            FormatString = detailInt?.DisplayMask ?? "f0"
                        }
                    }
                };
            case (int)eVariableType.Decimal:
                var detailDec = detail as VariableDecimal;
                return new AFEditSpin()
                {
                    Name = variable.VAR_NAME,
                    Value = detailDec?.Default ?? 0,
                    Properties ={
                        MinValue = detailDec?.Minimum ?? 0,
                        MaxValue = detailDec?.Maximum ?? decimal.MaxValue,
                        DisplayFormat = {
                            FormatType = FormatType.Numeric,
                            FormatString = detailDec?.DisplayMask ?? "f2"
                        },
                        EditFormat = {
                            FormatType = FormatType.Numeric,
                            FormatString = detailDec?.DisplayMask ?? "f2"
                        }
                    }
                };
            case (int)eVariableType.DateTime:
                var detailDate = detail as VariableDateTime;
                return new AFEditDate()
                {
                    Name = variable.VAR_NAME,
                    DateTime = detailDate?.GetDefault() ?? DateTime.Today,
                    Properties ={
                        MinValue = detailDate?.GetMinimum() ?? DateTime.MinValue,
                        MaxValue = detailDate?.GetMaximum() ?? DateTime.MinValue,
                        CalendarDateEditing = detailDate?.VariableType == eDateTimeVariableType.Date || detailDate?.VariableType != eDateTimeVariableType.Time,
                        CalendarTimeEditing = detailDate?.VariableType == eDateTimeVariableType.Date ? DefaultBoolean.False : DefaultBoolean.True,
                        MaskSettings = {
                            MaskExpression = detailDate?.VariableType == eDateTimeVariableType.Date
                            ? "d"
                            : detailDate?.VariableType == eDateTimeVariableType.Time
                                ? "t"
                                : "g"},
                        DisplayFormat = {
                            FormatType = FormatType.DateTime,
                            FormatString = detailDate?.VariableType == eDateTimeVariableType.Date
                                            ? "d"
                                            : detailDate?.VariableType == eDateTimeVariableType.Time
                                                ? "t"
                                                : "g"
                        },
                        EditFormat = {
                            FormatType = FormatType.DateTime,
                            FormatString = detailDate?.VariableType == eDateTimeVariableType.Date
                                ? "d"
                                : detailDate?.VariableType == eDateTimeVariableType.Time
                                    ? "t"
                                    : "g"
                        }
                    }
                };
            case (int)eVariableType.Memo:
                var detailMemo = detail as VariableMemo;
                return new AFEditMultiline()
                {
                    Name = variable.VAR_NAME,
                    Text = detailMemo?.Default ?? "",
                    Properties =
                    {
                        MaxLength = detailMemo?.MaxLength < 1 ? 0 : detailMemo?.MaxLength ?? 0
                    }
                };
            case (int)eVariableType.RichText:
                var detailRtf = detail as VariableRichText;
                return new AFRichEditSimple()
                {
                    Name = variable.VAR_NAME,
                    RichText = detailRtf?.Default ?? ""
                };
            case (int)eVariableType.String:
                var detailString = detail as VariableString;
                return new AFEditSingleline()
                {
                    Name = variable.VAR_NAME,
                    Text = detailString?.Default ?? "",
                    Properties =
                    {
                        MaxLength = detailString!.MaxLength > 0 ? detailString.MaxLength : 0
                    }
                };
            case (int)eVariableType.Month:
                var detailMonth = detail as VariableMonth;
                var retSleMonth = new AFEditSpinInt()
                {
                    Name = variable.VAR_NAME
                };
                retSleMonth.Properties.MinValue = detailMonth?.GetMinimum() ?? 1;
                retSleMonth.Properties.MaxValue = detailMonth?.GetMaximum() ?? 12;
                retSleMonth.ValueInt = detailMonth?.GetDefault() ?? (detailMonth?.GetMinimum() ?? 1);
                return retSleMonth;
            case (int)eVariableType.Year:
                var detailYear = detail as VariableYear;
                var retSleYear = new AFEditSpinInt()
                {
                    Name = variable.VAR_NAME
                };
                retSleYear.Properties.MinValue = detailYear?.GetMinimum() ?? 1000;
                retSleYear.Properties.MaxValue = detailYear?.GetMaximum() ?? 3999;
                retSleYear.ValueInt = detailYear?.GetDefault() ?? (detailYear?.GetMinimum() ?? 1000);
                return retSleYear;
            case (int)eVariableType.Guid:
                var detailGuid = detail as VariableGuid;
                var retSleGuid = new AFEditSingleline()
                {
                    Name = variable.VAR_NAME,
                    Text = detailGuid?.Default ?? Guid.Empty.ToString()
                };
                retSleGuid.SetMask(eSinglelineEditMask.Guid);
                return retSleGuid;
            case (int)eVariableType.List:
                var detailList = detail as VariableList;
                List<ListItem> items = [];
                VariableListEntry? defaultEntry = null;
                foreach (var value in detailList!.Entrys)
                {
                    items.Add(new() { Caption = value.DisplayName, Value = value.GetValue() });

                    if (value.IsDefault) defaultEntry = value;
                }

                if (variable.VAR_MULTIPLE)
                {
                    var retSleToken = new AFEditToken()
                    {
                        Name = variable.VAR_NAME,
                        Properties =
                        {
                            DataSource = items,
                            DisplayMember = nameof(ListItem.Caption),
                            ValueMember = nameof(ListItem.Value)
                        }
                    };

                    return retSleToken;
                }

                var retCmbList = new AFEditCombo()
                {
                    Name = variable.VAR_NAME,
                    Properties = { TextEditStyle = TextEditStyles.DisableTextEditor }
                };

                retCmbList.Fill(items);

                if (defaultEntry != null) { retCmbList.SelectedValue = defaultEntry.GetValue(); }

                return retCmbList;
            case (int)eVariableType.Model:
                if (detail is not VariableModel detailModel) throw new Exception($"Falsches Variablen-Model übergeben. (Soll: VariableModel, ist {detail.GetType().Name}");

                var gentype = typeof(AFComboBoxModel<>).MakeGenericType(detailModel.ModelType);
                var retModelCombo = Activator.CreateInstance(gentype) as Control;

                if (retModelCombo == null) throw new Exception($"Der Typ {gentype.FullName} repräsentiert kein Control.");

                return retModelCombo;
            case (int)eVariableType.Query:
                if (detail is not VariableQuery detailQuery) throw new Exception($"Falsches Variablen-Model übergeben. (Soll: VariableQuery, ist {detail.GetType().Name}");
                
                var lookup = new AFComboboxLookup()
                {
                    Name = variable.VAR_NAME,
                    
                    
                };
                lookup.Properties.ValueMember = detailQuery.ValueColumn;
                lookup.Properties.DisplayMember = detailQuery.DisplayColumn;
                lookup.Properties.DataSource = detailQuery.GetData([]);
                return lookup;
            default:
                {
                    var customtype = (typeof(VariableBase).GetController() as VariableBaseController)!.CustomTypes.Values.FirstOrDefault(t => t.VariableTypIndex == variable.VAR_TYP);
                    
                    if (customtype != null && Activator.CreateInstance(customtype.VariableEditorType) is Control ctrl)
                    {
                        ctrl.Name = variable.VAR_NAME;

                        if (ctrl is ICustomVariableEditor custom)
                            custom.SetVariable(detail);

                        return ctrl;
                    }

                    throw new Exception($"Für den Variablentypen {variable.VAR_TYP} existiert kein passender Editor.");
                }
        }
    }

    /// <summary>
    /// aktuellen Wert des Eingabecontrol setzen (aus string)
    /// </summary>
    /// <param name="ctrl">Eingabecontrol für die Variable</param>
    /// <param name="value">Wert setzen</param>
    public void SetEditorValue(Control ctrl, object value)
    {
        if (ctrl is TokenEdit edit)
        {
            if (value is IEnumerable<object> objects)
                foreach (object obj in objects)
                    AddToken(obj);
            else
                AddToken(value);

            edit.EditValue = value;

            void AddToken(object obj)
            {
                if (edit.Properties.Tokens.Count < 1 || edit.Properties.Tokens.All(x => !x.Value.Equals(obj)))
                    edit.Properties.Tokens.Add(new TokenEditToken(obj));
            }
        }
        else
        {
            string propertyname = ctrl.GetBindingPropertyName();
            if (propertyname.IsNotEmpty())
                ctrl.InvokeSet(propertyname, value);
        }
    }
    
    #endregion
}
