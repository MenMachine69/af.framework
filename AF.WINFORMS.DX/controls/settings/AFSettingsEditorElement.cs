using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für ein einzelnes Element
/// </summary>
[ToolboxItem(false)]
[SupportedOSPlatform("windows")]
[DesignerCategory("Code")]
public class AFSettingsEditorElement : AFUserControl
{
    private readonly AFLabel pshGoto = null!;
    private readonly AFTablePanel table = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFSettingsEditorElement(AFSettingsElement element, AFSettingsEditor editor)
    {
        if (UI.DesignMode) return;

        Size = new(editor.ClientRectangle.Width, Height);

        var currentElement = element;
        Control? ctrl = null;
        bool hassubelements = element.Elements.Any(e => e.IsSubElement);

        if (element.ValueName != "")
            ctrl = editor.Controller?.GetEditor(element) ?? throw new NullReferenceException("Dem AFSettingsEditor ist kein Controller zugwiesen!");

        table = new() { Dock = DockStyle.Top, UseSkinIndents = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, AutoSize = true };
        table.CustomPaintBackground = true;
        table.BackgroundAppearance = new()
        {
            AutoColors = false,
            PanelColor = UI.TranslateToSkinColor(Color.White).Blend(UI.TranslateSystemToSkinColor(SystemColors.Control), 0.5f),
            Dimmed = false,
            Shadow = true,
            CornerRadius = 8,
        };

        Controls.Add(table);

        table.BeginLayout();
        AFLabel symbol = new() { ImageAlignToText = ImageAlignToText.None };
        symbol.ImageOptions.SvgImageSize = new(32, 32);
        symbol.ImageOptions.Alignment = ContentAlignment.MiddleCenter;
        symbol.AutoSizeMode = LabelAutoSizeMode.Horizontal;
        symbol.ImageOptions.SvgImage = element.Symbol ?? UI.GetImage(Symbol.Settings);
        symbol.ImageOptions.SvgImageColorizationMode = element.SymbolColorization;
        table.Add(symbol, 1, 1, rowspan: 2);
        
        AFLabelBoldText caption = new() { Dock = DockStyle.Top, AutoSizeMode = LabelAutoSizeMode.Vertical, AllowHtmlString = true, Text = element.Caption };
        table.Add(caption, 1, 2);
        AFLabelGrayText description = new() { Dock = DockStyle.Top, AutoSizeMode = LabelAutoSizeMode.Vertical, AllowHtmlString = true, Text = element.Description };
        table.Add(description, 2, 2, colspan: 2);


        pshGoto = new() { ImageAlignToText = ImageAlignToText.None, Padding = new(3) };
        pshGoto.ImageOptions.SvgImageSize = new(16, 16);
        pshGoto.ImageOptions.Alignment = ContentAlignment.MiddleCenter;
        pshGoto.ImageOptions.SvgImage = UI.GetImage(Symbol.ChevronRight);
        pshGoto.AutoSizeMode = LabelAutoSizeMode.Horizontal;
        pshGoto.Visible = element.Elements.Any(e => e.IsSubElement == false);
        pshGoto.EnableHighlightHoover();
        pshGoto.Click += (_, _) =>
        {
            if (currentElement.Elements.Any())
                editor.LoadElements(currentElement);
        };
        table.Add(pshGoto, 1, 4);

        AFLabel pshPopup = new() { ImageAlignToText = ImageAlignToText.None, Padding = new(3) };
        pshPopup.ImageOptions.SvgImageSize = new(16, 16);
        pshPopup.ImageOptions.Alignment = ContentAlignment.MiddleCenter;
        pshPopup.ImageOptions.SvgImage = UI.GetImage(Symbol.ChevronDown);
        pshPopup.AutoSizeMode = LabelAutoSizeMode.Horizontal;
        pshPopup.Visible = element.ShowEditorInPopup;
        pshPopup.EnableHighlightHoover();
        table.Add(pshPopup, 2, 4);

        if (ctrl != null)
        {
            editor.RegisterControl(ctrl);

            if (!element.ShowEditorInPopup)
                table.Add(ctrl, 1, 3);
            else
                table.Add(ctrl, 3, 1, colspan: 4);
        }


        table.SetColumn(2, TablePanelEntityStyle.Relative, 1.0f);
        table.SetRow(3, TablePanelEntityStyle.AutoSize, 1.0f);

        table.EndLayout();


        if (hassubelements)
        {
            var subtable = loadSubElements(element, editor);
            table.Controls.Add(subtable);
            table.SetRow(subtable, 2);
            table.SetColumn(subtable, 1);
            table.SetColumnSpan(subtable, 3);
        }
        else
            table.Rows[2].Visible = false; // Zeile für das Popout unterdrücken

        table.Columns[0].Visible = element.Symbol != null;

        
        Size = new(Width, table.Height);

        UI.StyleChanged += styleChanged;
        
        SizeChanged += (_, _) =>
        {
            Size = new Size(Width, table.Height);
        };
    }

    private AFTablePanel loadSubElements(AFSettingsElement element, AFSettingsEditor editor)
    {
        AFTablePanel subtable = new() { Dock = DockStyle.Top, UseSkinIndents = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, AutoSize = true };

        subtable.BeginLayout();
        
        int row = 1;

        foreach (var subelement in element.Elements)
        {
            if (!subelement.IsSubElement) continue;

            Control? ctrl = null;

            if (subelement.ValueName != "")
                ctrl = editor.Controller?.GetEditor(subelement) ?? throw new NullReferenceException("Dem AFSettingsEditor ist kein Controller zugwiesen!");

            if (ctrl == null) continue;

            if (ctrl is AFEditToggle toggle)
            {
                toggle.Properties.AutoWidth = true;
                toggle.Dock = DockStyle.Right;
            }

            editor.RegisterControl(ctrl);

            subtable.Add(new AFLabel() { Text = subelement.Caption }, row, 1);
            subtable.Add(ctrl, row, 2);

            row++;

        }
        subtable.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
        subtable.EndLayout();

        foreach (var ctrl in subtable.Controls)
        {
            if (ctrl is AFEditToggle toggle)
            {
                toggle.Properties.AutoWidth = true;
                toggle.Dock = DockStyle.Right;
            }
        }

        return subtable;
    }

    private void styleChanged(object? sender, EventArgs e)
    {
        if (table.BackgroundAppearance == null) return;

        table.BackgroundAppearance.PanelColor = UI.TranslateToSkinColor(Color.White)
            .Blend(UI.TranslateSystemToSkinColor(SystemColors.Control), 0.5f);
    }
}