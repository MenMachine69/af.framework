namespace AF.WINFORMS.DX;

/// <summary>
/// Extensions for IVariable
/// </summary>
public static class IVariableEx
{


    /// <summary>
    /// return the control for the given Variable
    /// </summary>
    /// <typeparam name="T">varibale type</typeparam>
    /// <param name="variable">variable</param>
    /// <returns>the control, or null</returns>
    [SupportedOSPlatform("windows")]
    public static Control? GetControl<T>(this IVariable variable)
    {
        //if (!variable.AllowInput) return null;

        //if (variable is VariableScript) return null;

        //if (variable is VariableBool boolvar)
        //{
        //    ToggleSwitch ret = new ();
        //    ret.Properties.OffText = boolvar.DisplayStringOff;
        //    ret.Properties.OnText = boolvar.DisplayStringOn;
        //    ret.IsOn = boolvar.Current;
        //    return ret;
        //}

        //if (variable is VariableDateTime datevar)
        //{
        //    if (datevar.VariableType == eDateTimeVariableType.Date)
        //    {
        //        DateEdit edit = new ();
        //        edit.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.False;
        //        edit.Properties.EditMask = datevar.DisplayMask.IsEmpty() ? @"d" : datevar.DisplayMask;
        //        edit.DateTime = datevar.Current;
        //        return edit;
        //    }
            
        //    if (datevar.VariableType == eDateTimeVariableType.Time)
        //    {
        //        TimeEdit edit = new ();
        //        edit.Properties.EditMask = datevar.DisplayMask.IsEmpty() ? @"t" : datevar.DisplayMask;
        //        edit.Time = datevar.Current;
        //        return edit;
        //    }
            
        //    if (datevar.VariableType == eDateTimeVariableType.DateAndTime)
        //    {
        //        DateEdit edit = new ();
        //        edit.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
        //        edit.Properties.EditMask = datevar.DisplayMask.IsEmpty() ? @"g" : datevar.DisplayMask;
        //        edit.DateTime = datevar.Current;
        //        return edit;
        //    }
        //}

        //if (variable is VariableTimeSpan timespanvar)
        //{
        //    TimeSpanEdit edit = new();
        //    edit.Properties.MinValue = timespanvar.Minimum;
        //    edit.Properties.MaxValue = timespanvar.Maximum;
        //    edit.Properties.EditMask = timespanvar.DisplayMask;
        //    edit.TimeSpan = timespanvar.Current;
        //    return edit;
        //}

        //if (variable is VariableCurrency currvar)
        //{
        //    // CalcEdit edit = new();
        //    SpinEdit edit = new();
        //    edit.Properties.EditMask = currvar.DisplayMask.IsEmpty() ? @"c2" : currvar.DisplayMask;
        //    edit.Properties.UseMaskAsDisplayFormat = true;
        //    edit.Properties.IsFloatValue = true;
        //    edit.Properties.MinValue = currvar.Minimum;
        //    edit.Properties.MaxValue = currvar.Maximum;
        //    edit.Value = currvar.Current;
        //    return edit;
        //}

        //if (variable is VariableDecimal numbervar)
        //{
        //    SpinEdit edit = new();
        //    edit.Properties.EditMask = numbervar.DisplayMask.IsEmpty() ? @"f2" : numbervar.DisplayMask;
        //    edit.Properties.UseMaskAsDisplayFormat = true;
        //    edit.Properties.IsFloatValue = true;
        //    edit.Properties.MinValue = numbervar.Minimum;
        //    edit.Properties.MaxValue = numbervar.Maximum;
        //    edit.Value = numbervar.Current;
        //    return edit;
        //}

        //if (variable is VariableMemo memovar)
        //{
        //    MemoEdit edit = new();
        //    edit.Size = new Size(edit.Width, UI.GetScaled(60));
        //    edit.Properties.ScrollBars = ScrollBars.Vertical;
        //    edit.Text = memovar.Current;
        //    return edit;
        //}

        //if (variable is VariableRichText richvar)
        //{
        //    RichEditControl edit = new();
        //    edit.Size = new Size(edit.Width, UI.GetScaled(60));
        //    edit.ActiveViewType = RichEditViewType.Simple;
        //    edit.RtfText = richvar.Current;
        //    return edit;
        //}

        //if (variable is VariableString stringvar)
        //{
        //    TextEdit edit = new();
        //    edit.Properties.MaxLength = stringvar.MaxLength;
        //    edit.Text = stringvar.Current;
        //    return edit;
        //}

        //if (variable is VariableInt intvar)
        //{
        //    SpinEdit edit = new();
        //    edit.Properties.EditMask = intvar.DisplayMask.IsEmpty() ? @"n" : intvar.DisplayMask;
        //    edit.Properties.UseMaskAsDisplayFormat = true;
        //    edit.Properties.IsFloatValue = false;
        //    edit.Properties.MinValue = intvar.Minimum;
        //    edit.Properties.MaxValue = intvar.Maximum;
        //    edit.Value = intvar.Current;
        //    return edit;
        //}

        //if (variable is VariableList listvar)
        //{
        //    ImageComboBoxEdit edit = new();
        //    edit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

        //    List<ListItem> list = [];
        //    listvar.Entrys.ForEach(e =>
        //    {
        //        list.Add(new()
        //        {
        //            Caption = e.DisplayName,
        //            Value = e.Value
        //        });
        //    });

        //    edit.Fill(list);

        //    if (listvar.Current != null) 
        //        edit.SelectedIndex = Math.Max(0, list.FindIndex(e => e.Value == listvar.Current));
        //    else if (listvar.Default != null)
        //        edit.SelectedIndex = Math.Max(0, list.FindIndex(e => e.Value == listvar.Default));

        //    return edit;
        //}


        //if (variable is VariableQuery queryvar)
        //{
        //    SearchLookUpEdit edit = new SearchLookUpEdit();
        //    edit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        //    edit.Properties.DataSource = queryvar.GetData();
        //    edit.Properties.PopulateViewColumns();
        //    edit.Properties.DisplayMember = queryvar.DisplayColumn;
        //    edit.Properties.ValueMember = queryvar.ValueColumn;
        //    if (!queryvar.DisplayValueColumn)
        //    {
        //        var col = edit.Properties.View.Columns.FirstOrDefault(col => col.FieldName == queryvar.ValueColumn);
                
        //        if (col != null)
        //            edit.Properties.View.Columns.Remove(col);
        //    }

        //    return edit;
        //}

        //if (variable is VariableModel modelvar)
        //{
        //    if (modelvar.ModelType == null) throw new NullReferenceException(@$"No model type assigned to variable {modelvar.Name}.");

        //    var controller = modelvar.ModelType.GetController();

        //    if (controller == null) throw new NullReferenceException(@$"No controller for type {modelvar.ModelType.FullName} found.");

        //    if (controller is not IControllerUI uicontroller) throw new NullReferenceException(@$"Controller for type {modelvar.ModelType.FullName} ({controller.GetType().FullName}) is not a UIController.");

        //    var edit = uicontroller.GetUIElement(eUIElement.Combobox);

        //    if (edit == null) throw new NullReferenceException(@$"No UI element for model selection available ({modelvar.ModelType.FullName}).");

        //    ((SearchLookUpEdit)edit).EditValue = modelvar.Current != Guid.Empty ? modelvar.Current : modelvar.Default;

        //    return (Control)edit;
        //}

        return null;
    }

}

