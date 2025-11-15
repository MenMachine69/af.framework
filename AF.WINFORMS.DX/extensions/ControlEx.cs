using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;

namespace AF.WINFORMS.DX;

/// <summary>
/// Useful extensions for various Controls
/// </summary>
[SupportedOSPlatform("windows")]
public static class ControlsEx
{
    private static readonly Hashtable _boundMethodCache = new();
    private static readonly Hashtable _boundPropertysCache = new();

    /// <summary>
    /// Erzeugt eine Instanz des angegeben Control-Types.
    /// </summary>
    /// <returns>erzeugtes Control</returns>
    /// <exception cref="ArgumentException">Ausnahme, wenn type nicht von Control erbt.</exception>
    public static T CreateInstance<T>() where T : Control
    {
        return (CreateInstance(typeof(T)) as T)!;
    }

    /// <summary>
    /// Erzeugt eine Instanz des angegeben Control-Types.
    /// </summary>
    /// <param name="type">Typ des Controls</param>
    /// <returns>erzeugtes Control</returns>
    /// <exception cref="ArgumentException">Ausnahme, wenn type nicht von Control erbt.</exception>
    public static Control CreateInstance(Type type)
    {
        if (!type.IsSubclassOf(typeof(Control)))
            throw new ArgumentException($"Type {type} must be inherited from System.Windows.Forms.Control.");

        return (Activator.CreateInstance(type) as Control)!;
    }

    /// <summary>
    /// Prüft, ob sich die Maus über dem Control befindet.
    /// </summary>
    /// <param name = "control"></param>
    /// <returns>true, wenn sich die Maus im Control befindet, sonst false</returns>
    public static bool IsMouseOver(this Control control)
    {
        Point pt = control.PointToClient(Cursor.Position);
        return pt is { X: >= 0, Y: >= 0 } && pt.X <= control.Width && pt.Y <= control.Height;
    }

    /// <summary>
    /// Returns the parent control of a control that matches the specified type of T.
    /// </summary>
    /// <typeparam name="T">Type pf the parent control</typeparam>
    /// <param name="ctrl">Control whose parent control is being searched for</param>
    /// <returns>Parent control of type T or NULL if no such control exist</returns>
    public static T? GetParentControl<T>(this Control ctrl) where T : class
    {
        T? parent = null;

        Control? parentCtrl = ctrl.Parent;

        while (parentCtrl != null)
        {
            if (parentCtrl is T ctrl1)
            {
                parent = ctrl1;
                break;
            }

            parentCtrl = parentCtrl.Parent;
        }

        return parent;
    }



    /// <summary>
    /// Remove all controls including the call to Dispose method for every conrtrol
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="dispose"></param>
    public static void Clear(this Control.ControlCollection collection, bool dispose)
    {
        if (collection.Count < 1) return;

        List<Control> ctrls = [];
        ctrls.AddRange(collection.OfType<Control>());

        foreach (Control ctrl in ctrls)
        {
            if (ctrl.Controls.Count > 0)
                ctrl.Controls.Clear(dispose);

            collection.Remove(ctrl);

            if (dispose)
                ctrl.Dispose();
        }
    }

    /// <summary>
    /// Bound a control to a datasource (databinding)
    /// </summary>
    /// <param name="ctrl">Control to which the binding should be done</param>
    /// <param name="property">PropertyInfo, which describes the property of the datasource for which the binding is to be created</param>
    /// <param name="datasource">the data source containing the property to be bound to the control</param>
    /// <returns>a Binding object or NULL if no binding was created</returns>
    public static Binding? Bound(this Control ctrl, PropertyInfo property, object datasource)
    {
        return Bound(ctrl, property, datasource, DataSourceUpdateMode.OnValidation);
    }

    /// <summary>
    /// Bound a control to a datasource (databinding)
    /// </summary>
    /// <param name="ctrl">Control to which the binding should be done</param>
    /// <param name="property">PropertyInfo, which describes the property of the datasource for which the binding is to be created</param>
    /// <param name="datasource">the data source containing the property to be bound to the control</param>
    /// <param name="mode">data update mode, default is OnValidation</param>
    /// <returns>a Binding object or NULL if no binding was created</returns>
    public static Binding? Bound(this Control ctrl, PropertyInfo property, object datasource, DataSourceUpdateMode mode)
    {
        string bindingPropertyName = ctrl.GetBindingPropertyName();

        if (string.IsNullOrEmpty(bindingPropertyName)) return null;

        var binding = new Binding(bindingPropertyName, datasource, property.Name, false, mode);
        ctrl.DataBindings.Add(binding);

        return binding;
    }




    /// <summary>
    /// Determine the name of the property of a control to which the data binding can be made.
    /// This is normally the name defined in the DefaultBindingPropertyAttribute attribute. 
    /// </summary>
    /// <param name="ctrl">the control</param>
    /// <returns>Name of the property for DataBinding</returns>
    /// <exception cref="Exception">if DefaultBindingPropertyAttribute is assigned or the
    /// name in this attribute is empty</exception>
    public static string GetBindingPropertyName(this Control ctrl)
    {
        Type ctrlType = ctrl.GetType();

        if (!_boundPropertysCache.ContainsKey(ctrlType))
        {
            var att = ctrl.GetType().GetCustomAttributes(true)
                .OfType<DefaultBindingPropertyAttribute>().FirstOrDefault();

            if (att == null)
                throw new Exception(@"DefaultBindingPropertyAttribute not found for control " + ctrl.GetType().Name);

            if (att.Name == null)
                throw new Exception(@"DefaultBindingPropertyAttribute.Name is null/empty for control " + ctrl.GetType().Name);

            _boundPropertysCache.Add(ctrlType, att.Name);
        }

        string? ret = _boundPropertysCache[ctrlType] as string;

        if (ctrl is ICustomBindingSupport custom && custom.CustomBindingProperty.IsNotEmpty())
            ret = custom.CustomBindingProperty;

        return ret ?? "";
    }

    /// <summary>
    /// Register a property for data binding that differs from the default (DefaultBindingPropertyAttribute).
    /// </summary>
    /// <param name="ctrl">control to be registered</param>
    /// <param name="name">Name of the property to be used for the binding</param>
    public static void RegisterBindingProperty(this Control ctrl, string name)
    {
        RegisterBindingProperty(ctrl.GetType(), name);
    }

    /// <summary>
    /// Register a property for data binding that differs from the default (DefaultBindingPropertyAttribute).
    /// </summary>
    /// <param name="type">Type of control to be registered</param>
    /// <param name="name">Name of the property to be used for the binding</param>
    public static void RegisterBindingProperty(this Type type, string name)
    {
        if (_boundPropertysCache.ContainsKey(type))
            _boundPropertysCache[type] = name;
        else
            _boundPropertysCache.Add(type, name);
    }

    internal static MethodInfo? getBoundMethod(this Type type)
    {
        if (_boundMethodCache.ContainsKey(type) == false)
            _boundMethodCache.Add(type, type.GetExtensionMethod(@"Bound", true));

        return (MethodInfo?)_boundMethodCache[type];
    }

    /// <summary>
    /// SendMessage-Methode (WIN32)
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="wMsg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

    private const int WM_SETREDRAW = 11;

    /// <summary>
    /// Suppress redrawing the control until ResumeDrawing is called up
    /// </summary>
    /// <param name="ctrl"></param>
    public static void SuspendDrawing(this Control ctrl)
    {
        SendMessage(ctrl.Handle, WM_SETREDRAW, false, 0);
    }

    /// <summary>
    /// Resume redrawing of the control (after calling SuspendDrawing)
    /// </summary>
    /// <param name="ctrl"></param>
    public static void ResumeDrawing(this Control ctrl)
    {
        SendMessage(ctrl.Handle, WM_SETREDRAW, true, 0);
        ctrl.Refresh();
    }

    /// <summary>
    /// Represents the control with an blur effect
    /// </summary>
    /// <param name="ctrl">Control that is to be displayed with a blur effect....</param>
    public static void Blur(this Control ctrl)
    {
        foreach (Control c in ctrl.Controls)
            if (ctrl.Controls.GetChildIndex(c) == 0 && c is PictureBox && c.Dock == DockStyle.Fill) return;

        Bitmap screenshot = ctrl.Screenshot();
        BitmapFilter.GaussianBlur(screenshot, 4);
        BitmapFilter.GaussianBlur(screenshot, 4);
        BitmapFilter.GaussianBlur(screenshot, 4);
        BitmapFilter.GaussianBlur(screenshot, 4);
        BitmapFilter.GaussianBlur(screenshot, 4);
        BitmapFilter.GaussianBlur(screenshot, 4);

        using (Graphics canvas = Graphics.FromImage(screenshot)) 
        {
            using (SolidBrush brush = new(Color.FromArgb(120, Color.White)))
                canvas.FillRectangle(brush, 0, 0, screenshot.Width, screenshot.Height);
        }

        PictureBox pbox = new()
            { Size = new Size(screenshot.Width, screenshot.Height), Dock = DockStyle.Fill, Image = screenshot };
        ctrl.Controls.Add(pbox);
        pbox.BringToFront();
    }

    /// <summary>
    /// Liefert ein Bild des Controls
    /// </summary>
    /// <param name="ctl">Control, dessen Screenshot ermittelt wird</param>
    /// <returns>Grafik (Bitmap) des Controls</returns>
    public static Bitmap Screenshot(this Control ctl)
    {
        Bitmap bmp = new(ctl.Size.Width, ctl.Size.Height);
        Graphics g = Graphics.FromImage(bmp);

        g.CopyFromScreen(ctl is Form ? ctl.Location : ctl.PointToScreen(ctl.Location), new Point(0, 0), ctl.Size);

        return bmp;
    }


    /// <summary>
    /// Removes the blur effect created with Blur() from the Control...
    /// </summary>
    /// <param name="ctrl">Control whose blur effect is to be removed</param>
    public static void UnBlur(this Control ctrl)
    {
        foreach (Control c in ctrl.Controls)
        {
            if (ctrl.Controls.GetChildIndex(c) != 0 || c is not PictureBox) continue;

            ctrl.Controls.Remove(c);
            c.Dispose();
            break;
        }
    }

    /// <summary>
    /// Data binding to the 'IsOn' property of a ToggleSwitch as the default binding.
    /// </summary>
    /// <param name="ctrl">Control to bind to</param>.
    /// <param name="datasource">Data source</param>.
    /// <param name="property">Name of property</param>
    public static Binding Bound(this ToggleSwitch ctrl, PropertyInfo property, object datasource)
    {
        return new Binding(nameof(ToggleSwitch.IsOn), datasource, property.Name, false, DataSourceUpdateMode.OnValidation, false);
    }

    /// <summary>
    /// Füllt die Listbox mit einer Liste von ListItem-Objekten
    /// 
    /// Der Anzeigename des Items entspricht dabei dem Ergebnis der ToSTring() Methode des Items. 
    /// Der ValueMember (Rückgabewert von SelectedValue) entspricht standardmässig der Value-Eigenschaft des Items und 
    /// muss nach dem zuweisen der Liste NICHT manuell gesetzt werden.
    /// </summary>
    /// <param name="items">Liste der zur Auswahl anzuzeigenden Objekte</param>
    /// <param name="cmb">ComboBox, die gefüllt werden soll</param>
    /// <param name="addValues">true = zu vorhandenen Werten hinzufügen</param>
    public static void Fill(this ComboBoxEdit cmb, IEnumerable<ListItem> items, bool addValues = false)
    {
        if (!addValues)
            cmb.Properties.Items.Clear();

        if (cmb is AFEditComboImageList imagecmb)
            imagecmb.ValueMember = nameof(ListItem.Value);
        else if (cmb is AFEditCombo cmbox)
            cmbox.ValueMember = nameof(ListItem.Value);

        if (cmb is ImageComboBoxEdit)
        {
            foreach (ListItem o in items)
                cmb.Properties.Items.Add(new ImageComboBoxItem(o, o.ImageIndex));
        }
        else
            cmb.Properties.Items.AddRange(items.Cast<object>().ToArray());
    }

    /// <summary>
    /// Fills the ListBoxControl with a list of ListItem objects.
    /// </summary>
    /// <param name="items">List of items to be displayed for selection</param>.
    /// <param name="ctrl">listbox to fill</param>
    public static void Fill(this ListBoxControl ctrl, IEnumerable<ListItem> items)
    {
        ctrl.Items.Clear();
        ctrl.DataSource = items;
        ctrl.ValueMember = @"Value";
        ctrl.DisplayMember = @"Caption";
    }

    /// <summary>
    /// Fills the ListBoxControl with a list of ListItem objects.
    /// </summary>
    /// <param name="items">List of items to be displayed for selection</param>.
    /// <param name="ctrl">listbox to fill</param>
    /// <param name="valuemember">Name of the property to be used as the value member</param>
    public static void Fill(this ListBoxControl ctrl, IEnumerable<object> items, string valuemember)
    {
        ctrl.Items.Clear();
        ctrl.DataSource = items;
        ctrl.ValueMember = valuemember;
    }


    /// <summary>
    /// Fills the list box with a list of ListItem objects.
    /// 
    /// The display name of the item corresponds to the result of the ToString() method of the item. 
    /// The ValueMember (return value of SelectedValue) corresponds by default to the Value property of the item and 
    /// does NOT need to be set manually after assigning the list.
    ///
    /// Uses 'Value' as the ValueMember if no ValueMember is set.
    /// </summary>
    /// <param name="edit">ComboBox to be expanded</param>.
    /// <param name="items">List of items to be displayed for selection</param>
    public static void Fill(this AFEditCombo edit, List<ListItem> items)
    {
        edit.Properties.Items.Clear();
        edit.Properties.Items.AddRange(items.Cast<object>().ToArray());

        if (edit.HasSet(@"ValueMember"))
            edit.InvokeSet(@"ValueMember", @"Value");
    }

    /// <summary>
    /// Fill the list of selectable values using an enumeration
    /// </summary>
    /// <typeparam name="TEnum">Typ/Enumeration</typeparam>
    /// <typeparam name="TEdit">Typ des Controls, dessen Enum gesetzt wird.</typeparam>
    /// <param name="edit">ComboBox to expand</param>.
    /// <param name="valueAsInt">Specifies whether the values of the combo box are to be displayed as Int values, if not, the values are displayed as enum values</param>
    /// <param name="exclude">Liste mit auszuschliessende Werten</param>
    public static TEdit SetEnumeration<TEnum, TEdit>(this TEdit edit, bool valueAsInt = false, Enum[]? exclude = null)
        where TEnum : struct, IConvertible
        where TEdit : ComboBoxEdit
    {
        return SetEnumeration(edit, typeof(TEnum), valueAsInt: valueAsInt, exclude: exclude);
    }

    /// <summary>
    /// Fill the list of selectable values using an enumeration
    /// </summary>
    /// <typeparam name="TEdit">Typ des Controls, dessen Enum gesetzt wird.</typeparam>
    /// <param name="edit">ComboBox to expand</param>.
    /// <param name="enumType">Typ/Enumeration</param>
    /// <param name="valueAsInt">Specifies whether the values of the combo box are to be displayed as Int values, if not, the values are displayed as enum values</param>
    /// <param name="addValues">true = zu vorhandenen Werten hinzufügen</param>
    /// <param name="exclude">Liste mit auszuschliessende Werten</param>
    /// <param name="include">Liste der einzuschliessenden Werte</param>
    public static TEdit SetEnumeration<TEdit>(this TEdit edit, Type enumType, bool valueAsInt = false, bool addValues = false, Enum[]? exclude = null, Enum[]? include = null)
        where TEdit : ComboBoxEdit
    {
        if (enumType.IsEnum == false)
            throw new Exception($@"Der Typ {enumType} ist keine Aufzählung.");

        List<ListItem> items = [];
        foreach (Enum val in Enum.GetValues(enumType))
        {
            if (include != null && !include.Contains(val))
                continue;

            if (exclude != null && exclude.Contains(val))
                continue;

            items.Add(valueAsInt
                ? new ListItem() { Caption = val.GetEnumDescription(), Value = Convert.ToInt32(val) }
                : new ListItem() { Caption = val.GetEnumDescription(), Value = val });
        }

        edit.Fill(items.OrderBy(item => item.Caption).ToList(), addValues: addValues);

        edit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

        return edit;
    }

    /// <summary>
    /// Fills the combobox with a list of ListItem objects.
    ///
    /// Uses 'Value' as the ValueMember and 'Caption' as the DisplayMember.
    /// </summary>
    /// <param name="items">List of items to be displayed for selection</param>.
    /// <param name="ctrl">combo box to be filled</param>
    public static void Fill(this CheckedComboBoxEdit ctrl, IEnumerable<ListItem> items)
    {
        ctrl.Properties.Items.Clear();
        ctrl.Properties.DataSource = items;
        ctrl.Properties.ValueMember = @"Value";
        ctrl.Properties.DisplayMember = @"Caption";
        ctrl.Properties.SynchronizeEditValueWithCheckedItems = false;
        var selecteditems = items.Where(item => item.Checked).Select(item => item.Value).ToList();
        ctrl.SetEditValue(selecteditems);
        //ctrl.CheckAll();
    }

    /// <summary>
    /// Fills the combobox with a list of ListItem objects.
    ///
    /// Uses 'Value' as the ValueMember and 'Caption' as the DisplayMember.
    /// Uses 'Checked' as the CheckMember.
    /// </summary>
    /// <param name="items">List of items to be displayed for selection</param>.
    /// <param name="ctrl">combo box to be filled</param>
    public static void Fill(this CheckedListBoxControl ctrl, IEnumerable<ListItem> items)
    {
        ctrl.Items.Clear();
        ctrl.DataSource = items;
        ctrl.ValueMember = @"Value";
        ctrl.DisplayMember = @"Caption";
        ctrl.CheckMember = @"Checked";
    }

    /// <summary>
    /// Fills the listbox with a list of ListItem objects.
    ///
    /// Uses 'Value' as the ValueMember and 'Caption' as the DisplayMember.
    /// Uses 'ImageIndex' as the ImageIndexMember.
    /// </summary>
    /// <param name="items">List of items to be displayed for selection</param>.
    /// <param name="ctrl">listbox to be filled</param>
    public static void Fill(this ImageListBoxControl ctrl, IEnumerable<ListItem> items)
    {
        ctrl.Items.Clear();
        ctrl.DataSource = items;
        ctrl.ValueMember = @"Value";
        ctrl.ImageIndexMember = @"ImageIndex";
        ctrl.DisplayMember = @"Caption";
    }

    /// <summary>
    /// Fills the combobox with a list of ListItem objects.
    ///
    /// Uses 'Value' as the ValueMember and 'Caption' as the DisplayMember.
    /// Uses 'ImageIndex' as the ImageIndexMember.
    /// </summary>
    /// <param name="items">List of items to be displayed for selection</param>.
    /// <param name="ctrl">combobox to be filled</param>
    public static void Fill(this ImageComboBoxEdit ctrl, IEnumerable<ListItem> items)
    {
        ctrl.Properties.Items.Clear();

        foreach (var item in items)
        {
            ctrl.Properties.Items.Add(new ImageComboBoxItem
            {
                Value = item.Value,
                Description = item.Caption,
                ImageIndex = item.ImageIndex
            });
        }
    }

    /// <summary>
    /// Fills the combobox with a list of ListItem objects.
    ///
    /// Uses 'Value' as the ValueMember and 'Caption' as the DisplayMember.
    /// Uses 'ImageIndex' as the ImageIndexMember.
    /// </summary>
    /// <param name="items">List of items to be displayed for selection</param>.
    /// <param name="ctrl">combobox to be filled</param>
    public static void Fill(this RepositoryItemImageComboBox ctrl, IEnumerable<ListItem> items)
    {
        ctrl.Items.Clear();

        foreach (var item in items)
        {
            ctrl.Items.Add(new ImageComboBoxItem
            {
                Value = item.Value,
                Description = item.Caption,
                ImageIndex = item.ImageIndex
            });
        }
    }



    /// <summary>
    /// Setzt die 'ReadOnly'-Eigenschaft auf true.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <returns>das Control</returns>
    public static BaseEdit ReadOnly(this BaseEdit edit)
    {
        edit.ReadOnly = true;
        return edit;
    }

    /// <summary>
    /// Setzt die 'Name'-Eigenschaft auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="name">Wert/Name</param>
    /// <returns>das Control</returns>
    public static T Name<T>(this T edit, string name) where T : Control
    {
        edit.Name = name;
        return edit;
    }

    /// <summary>
    /// Setzt die 'Text'-Eigenschaft auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="text">Wert/Text</param>
    /// <returns>das Control</returns>
    public static T Text<T>(this T edit, string text) where T : Control
    {
        edit.Text = text;
        return edit;
    }

    /// <summary>
    /// Setzt die 'MinValue/Minimum'-Eigenschaft auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="minvalue">minimal zulässiger Wert</param>
    /// <returns>das Control</returns>
    public static T Minimum<T>(this T edit, object minvalue) where T : Control
    {
        if (edit is SpinEdit spin && minvalue is decimal spindec)
            spin.Properties.MinValue = spindec;
        else if (edit is SpinEdit spin2 && minvalue is int spinint)
            spin2.Properties.MinValue = spinint;
        else if (edit is TrackBarControl track && minvalue is int trackint)
            track.Properties.Minimum = trackint;
        else if (edit is DateEdit date && minvalue is DateTime datedt)
            date.Properties.MinDate = datedt;
        else if (edit is DateEdit dateonly && minvalue is DateOnly datedo)
            dateonly.Properties.MinDate = datedo;
        else
            throw new ArgumentException($"Method 'Minimum' not supported for Control Type {typeof(T).FullName}.");

        return edit;
    }

    /// <summary>
    /// Setzt die 'MaxValue/Maximum'-Eigenschaft auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="maxvalue">maximal zulässiger Wert</param>
    /// <returns>das Control</returns>
    public static T Maximum<T>(this T edit, object maxvalue) where T : Control
    {
        if (edit is SpinEdit spin && maxvalue is decimal spindec)
            spin.Properties.MaxValue = spindec;
        else if (edit is SpinEdit spin2 && maxvalue is int spinint)
            spin2.Properties.MaxValue = spinint;
        else if (edit is TrackBarControl track && maxvalue is int trackint)
            track.Properties.Maximum = trackint;
        else if (edit is DateEdit date && maxvalue is DateTime datedt)
            date.Properties.MaxDate = datedt;
        else if (edit is DateEdit dateonly && maxvalue is DateOnly datedo)
            dateonly.Properties.MaxDate = datedo;
        else
            throw new ArgumentException($"Method 'Maximum' not supported for Control Type {typeof(T).FullName}.");
        
        return edit;
    }

    /// <summary>
    /// Setzt die 'MaxLength'-Eigenschaft auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="maxlength">maximal zulässiger Länge</param>
    /// <returns>das Control</returns>
    public static T MaxLength<T>(this T edit, int maxlength) where T : BaseEdit
    {
        if (edit is TextEdit txt)
            txt.Properties.MaxLength = maxlength;
        else if (edit is MemoEdit memo)
            memo.Properties.MaxLength = maxlength;
        
        return edit;
    }

    /// <summary>
    /// Setzt die 'Size.Width'-Eigenschaft (Breite) auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="width">Breite des Controls</param>
    /// <returns>das Control</returns>
    public static T Width<T>(this T edit, int width) where T : Control
    {
        edit.Size = new(width, edit.Size.Height);

        return edit;
    }

    /// <summary>
    /// Setzt die 'Size.Height'-Eigenschaft (Höhe) auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="height">Höhe des Controls</param>
    /// <returns>das Control</returns>
    public static T Height<T>(this T edit, int height) where T : Control
    {
        edit.Size = new(edit.Size.Width, height);

        return edit;
    }

    /// <summary>
    /// Setzt die 'Size'-Eigenschaft (Breite und Höhe) auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="width">Breite des Controls</param>
    /// <param name="height">Höhe des Controls</param>
    /// <returns>das Control</returns>
    public static T Size<T>(this T edit, int width, int height) where T : Control
    {
        edit.Size = new(width, height);

        return edit;
    }

    /// <summary>
    /// Setzt die 'Margin.Left'-Eigenschaft (Einrückung von links) auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="indent">Neue Einrückung</param>
    /// <returns>das Control</returns>
    public static T Indent<T>(this T edit, int indent) where T : Control
    {
        edit.Margin = new(indent, edit.Margin.Top, edit.Margin.Right, edit.Margin.Bottom);

        return edit;
    }

    /// <summary>
    /// Setzt die 'Margin.Right'-Eigenschaft (Einrückung von rechts) auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="indent">Neue Einrückung</param>
    /// <returns>das Control</returns>
    public static T IndentRight<T>(this T edit, int indent) where T : Control
    {
        edit.Margin = new(edit.Margin.Left, edit.Margin.Top, indent, edit.Margin.Bottom);

        return edit;
    }

    /// <summary>
    /// Setzt die 'Margin.Top'-Eigenschaft (Einrückung von oben) auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="indent">Neue Einrückung</param>
    /// <returns>das Control</returns>
    public static T IndentTop<T>(this T edit, int indent) where T : Control
    {
        edit.Margin = new(edit.Margin.Left, indent, edit.Margin.Right, edit.Margin.Bottom);

        return edit;
    }

    /// <summary>
    /// Setzt die 'Margin.Bottom'-Eigenschaft (Einrückung/Abstand von unten) auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="indent">Neue Einrückung</param>
    /// <returns>das Control</returns>
    public static T IndentBottom<T>(this T edit, int indent) where T : Control
    {
        edit.Margin = new(edit.Margin.Left, edit.Margin.Top, edit.Margin.Right, indent);

        return edit;
    }

    /// <summary>
    /// Setzt die 'Margin'-Eigenschaft (Einrückungen alle Seiten) auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="all">Neue Einrückung</param>
    /// <returns>das Control</returns>
    public static T Margin<T>(this T edit, int all) where T : Control
    {
        edit.Margin = new(all);

        return edit;
    }

    /// <summary>
    /// Setzt die 'Margin'-Eigenschaft (Einrückungen alle Seiten) auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="left">Neue Einrückung von links</param>
    /// <param name="right">Neue Einrückung von rechts</param>
    /// <param name="top">Neue Einrückung von oben</param>
    /// <param name="bottom">Neue Einrückung von unten</param>
    /// <returns>das Control</returns>
    public static T Margin<T>(this T edit, int left, int right, int top, int bottom) where T : Control
    {
        edit.Margin = new(left, top, right, bottom);

        return edit;
    }


    /// <summary>
    /// Setzt die 'Padding'-Eigenschaft auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Label, dessen Eigenschaft gesetzt wird</param>
    /// <param name="padding">Abstand (alle Seiten)</param>
    /// <returns>das Label</returns>
    public static T Padding<T>(this T edit, int padding) where T : Control
    {
        edit.Padding = new(padding);

        return edit;
    }

    /// <summary>
    /// Setzt die 'Padding'-Eigenschaft auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Label, dessen Eigenschaft gesetzt wird</param>
    /// <param name="paddingLeft">Abstand link</param>
    /// <param name="paddingTop">Abstand oben</param>
    /// <param name="paddingRight">Abstand rechts</param>
    /// <param name="paddingBottom">Abstand unten</param>
    /// <returns>das Label</returns>
    public static T Padding<T>(this T edit, int paddingLeft, int paddingTop, int paddingRight, int paddingBottom) where T : Control
    {
        edit.Padding = new(paddingLeft, paddingTop, paddingRight, paddingBottom);

        return edit;
    }

    /// <summary>
    /// Setzt die 'AutoSizeMode'-Eigenschaft auf den angegebenen Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="mode">Modus</param>
    /// <returns>das Control</returns>
    public static T AutoSizeMode<T>(this T edit, LabelAutoSizeMode mode) where T : AFLabel
    {
        edit.AutoSizeMode = mode;
        return edit;
    }

    /// <summary>
    /// Setzt die 'Enabled'-Eigenschaft auf den Wert false.
    /// </summary>
    /// <param name="edit">Label, dessen Eigenschaft gesetzt wird</param>
    /// <returns>das Label</returns>
    public static T Disable<T>(this T edit) where T : Control
    {
        edit.Enabled = false;
        return edit;
    }

    /// <summary>
    /// Setzt die 'ReadOnly'-Eigenschaft auf den Wert true.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <returns>das Control</returns>
    public static T ReadOnly<T>(this T edit) where T : BaseEdit
    {
        edit.ReadOnly = true;
        return edit;
    }

    /// <summary>
    /// Setzt die 'ReadOnly'-Eigenschaft auf den Wert false.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <returns>das Control</returns>
    public static T ReadWrite<T>(this T edit) where T : BaseEdit
    {
        edit.ReadOnly = false;
        return edit;
    }


    /// <summary>
    /// Stellt ein LabelControl so ein, dass Texte umgebrochen werden. Die Ausrichtung des Textes ist oben links, die Höhe wist automatisch.
    /// </summary>
    /// <param name="label">Label, dessen Eigenschaft gesetzt wird</param>
    /// <returns>das Label</returns>
    public static T SetMultiline<T>(this T label) where T : LabelControl
    {
        label.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        label.Dock = DockStyle.Top;
        label.AutoSizeMode = LabelAutoSizeMode.Vertical;
        label.Appearance.TextOptions.VAlignment = VertAlignment.Top;
        label.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
        label.Appearance.Options.UseTextOptions = true;
        label.AutoEllipsis = true;
        label.UseMnemonic = false;

        return label;
    }

    /// <summary>
    /// Stellt ein LabelControl so ein, dass Texte rechtsbündig angezeigt wird.
    /// </summary>
    /// <param name="label">Label, dessen Eigenschaft gesetzt wird</param>
    /// <returns>das Label</returns>
    public static T AlignRight<T>(this T label) where T : LabelControl
    {
        label.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        label.Appearance.Options.UseTextOptions = true;

        return label;
    }

    /// <summary>
    /// Stellt ein LabelControl so ein, dass Texte zentriert angezeigt wird.
    /// </summary>
    /// <param name="label">Label, dessen Eigenschaft gesetzt wird</param>
    /// <returns>das Label</returns>
    public static T AlignCenter<T>(this T label) where T : LabelControl
    {
        label.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        label.Appearance.Options.UseTextOptions = true;

        return label;
    }

    /// <summary>
    /// Stellt ein LabelControl so ein, dass Texte linksbündig angezeigt wird.
    /// </summary>
    /// <param name="label">Label, dessen Eigenschaft gesetzt wird</param>
    /// <returns>das Label</returns>
    public static T AlignLeft<T>(this T label) where T : LabelControl
    {
        label.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
        label.Appearance.Options.UseTextOptions = true;

        return label;
    }

    /// <summary>
    /// Stellt ein LabelControl so ein, dass Texte linksbündig angezeigt wird.
    /// </summary>
    /// <param name="label">Label, dessen Eigenschaft gesetzt wird</param>
    /// <returns>das Label</returns>
    public static T ShowLine<T>(this T label) where T : LabelControl
    {
        label.LineOrientation = LabelLineOrientation.Horizontal;
        label.LineLocation = LineLocation.Bottom;
        label.LineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        label.LineVisible = true;
        label.LineColor = UI.TranslateSystemToSkinColor(SystemColors.ControlText);

        return label;
    }

    /// <summary>
    /// Ausrichtung des Textes anpassen.
    /// </summary>
    /// <typeparam name="T">Type des Labels</typeparam>
    /// <param name="label">Label</param>
    /// <param name="alignment">Ausrichtung</param>
    /// <returns>Label</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static T AdjustAlignment<T>(this T label, ContentAlignment alignment) where T : LabelControl
    {
        switch (alignment)
        {
            case ContentAlignment.BottomCenter:
                label.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
                label.Appearance.TextOptions.VAlignment = VertAlignment.Bottom;
                break;
            case ContentAlignment.TopLeft:
                label.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
                label.Appearance.TextOptions.VAlignment = VertAlignment.Top;
                break;
            case ContentAlignment.TopCenter:
                label.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
                label.Appearance.TextOptions.VAlignment = VertAlignment.Top;
                break;
            case ContentAlignment.TopRight:
                label.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
                label.Appearance.TextOptions.VAlignment = VertAlignment.Top;
                break;
            case ContentAlignment.MiddleLeft:
                label.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
                label.Appearance.TextOptions.VAlignment = VertAlignment.Center;
                break;
            case ContentAlignment.MiddleCenter:
                label.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
                label.Appearance.TextOptions.VAlignment = VertAlignment.Center;
                break;
            case ContentAlignment.MiddleRight:
                label.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
                label.Appearance.TextOptions.VAlignment = VertAlignment.Center;
                break;
            case ContentAlignment.BottomLeft:
                label.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
                label.Appearance.TextOptions.VAlignment = VertAlignment.Bottom;
                break;
            case ContentAlignment.BottomRight:
                label.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
                label.Appearance.TextOptions.VAlignment = VertAlignment.Bottom;
                break;
            default:
                label.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
                label.Appearance.TextOptions.VAlignment = VertAlignment.Center;
                throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
        }

        label.Appearance.Options.UseTextOptions = true;

        return label;
    }

    /// <summary>
    /// Stellt ein LabelControl so ein, dass Texte umgebrochen werden. Die Ausrichtung des Textes ist oben links, die Höhe wist automatisch.
    /// </summary>
    /// <param name="label">Label, dessen Eigenschaft gesetzt wird</param>
    /// <param name="image">das anzuzeigende Symbol</param>
    /// <param name="imgSize">Größe des Bildes (default: 16,16)</param>
    /// <param name="alignment">Ausrichtung zum Text (default: LeftCenter)</param>
    /// <param name="colorizationMode">Colorierung des Symbols (default: full)</param>
    /// <returns>das Label</returns>
    public static T WithImage<T>(this T label, SvgImage image, Size? imgSize = null, ImageAlignToText alignment = ImageAlignToText.LeftCenter, SvgImageColorizationMode colorizationMode = SvgImageColorizationMode.Full) where T : LabelControl
    {
        label.ImageOptions.SvgImage = image;
        label.ImageOptions.SvgImageSize = imgSize ?? new Size(16, 16);
        label.ImageAlignToText = alignment;
        label.ImageOptions.SvgImageColorizationMode = colorizationMode;

        return label;
    }

    /// <summary>
    /// Weisst einem BaseControl (DevExpress) einen SuperToolTip zu.
    /// </summary>
    /// <param name="edit">Control</param>
    /// <param name="supertipp">SuperToolTip</param>
    /// <returns>Control</returns>
    public static T SuperTip<T>(this T edit, SuperToolTip supertipp) where T : BaseControl
    {
        edit.SuperTip = supertipp;
        return edit;
    }

    /// <summary>
    /// Setzt die 'Enabled'-Eigenschaft auf den Wert true.
    /// </summary>
    /// <param name="edit">Label, dessen Eigenschaft gesetzt wird</param>
    /// <returns>das Label</returns>
    public static T Enable<T>(this T edit) where T : Control
    {
        edit.Enabled = true;
        return edit;
    }


    /// <summary>
    /// Setzt die 'Dock'-Eigenschaft auf den Wert.
    /// </summary>
    /// <param name="edit">Control, dessen Eigenschaft gesetzt wird</param>
    /// <param name="style">Dock-Style</param>
    /// <returns>das Control</returns>
    public static T Dock<T>(this T edit, DockStyle style) where T : Control
    {
        edit.Dock = style;
        return edit;
    }


    /// <summary>
    /// Standard-Control für ein  Property ermitteln
    /// </summary>
    /// <param name="item">PropertyInfo der Eigenschaft, für die das Control benötigt wird.</param>
    /// <returns>das geeignete Control oder NULL</returns>
    public static Control? GetDefaultControl(PropertyInfo item)
    {
        var type = item.PropertyType;

        if (type == typeof(DateTime)) { return new AFEditDate().Name(item.Name); }

        if (type == typeof(bool)) { return new AFEditToggle { Properties = { AutoWidth = true, Name = item.Name } }; }

        if (type == typeof(string)) { return new AFEditSingleline().Name(item.Name); }

        if (type == typeof(Type)) { return new AFEditSingleline() { ReadOnly = true, Name = item.Name }; }

        if (type == typeof(int)) { return new AFEditSpinInt().Name(item.Name); }
        
        if (type == typeof(decimal) || type == typeof(float)) { return new AFEditCalc().Name(item.Name); }

        if (type.IsEnum) { return new AFEditCombo().SetEnumeration(type).Name(item.Name); }

        return null;
    }

    /// <summary>
    /// Element zu einem TabPage hinzufügen.
    /// </summary>
    /// <param name="tabpage">TabPage zu der das Element hinzugefügt wird</param>
    /// <param name="ctrl">Element, das hinzugefügt wird</param>
    /// <param name="dock">DockStyle</param>
    /// <param name="islastelement">ist letztes Element (keinen Splitter hinzufügen)</param>
    /// <returns>das hinzugefügte Element</returns>
    public static T AddElement<T>(this XtraTabPage tabpage, T ctrl, DockStyle dock, bool islastelement = false) where T : Control
    {
        ctrl.Dock = dock;
        

        if (ctrl is AFEditorDetail detail)
        {
            if (dock == DockStyle.Left || dock == DockStyle.Right)
                detail.Size = new(detail.DefaultEditorWidth, tabpage.Height);
            else
                detail.Size = new(tabpage.Width, detail.DefaultEditorHeight);
        }
        
        tabpage.Controls.Add(ctrl);
        ctrl.BringToFront();

        if (!islastelement)
        {
            var splitter = new AFNavSplitter { Dock = dock, ShowSplitGlyph = DefaultBoolean.True };
            tabpage.Controls.Add(splitter);
            splitter.BringToFront();
        }

        return ctrl;
    }

}