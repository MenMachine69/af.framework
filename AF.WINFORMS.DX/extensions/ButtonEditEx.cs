using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace AF.WINFORMS.DX;

/// <summary>
/// Erweiterungsmethoden für die Klasse AFLabel
/// </summary>
public static class AFLabelEx
{
    /// <summary>
    /// Label mit Highlight-Farben hervorheben, wenn Maus über das Label bewegt wird.
    /// </summary>
    /// <param name="label">Label</param>
    public static void EnableHighlightHoover(this AFLabel label)
    {
        label.AppearanceHovered.BackColor = UI.TranslateSystemToSkinColor(SystemColors.Highlight);
        label.AppearanceHovered.ForeColor = UI.TranslateSystemToSkinColor(SystemColors.HighlightText);
        label.AppearanceHovered.Options.UseBackColor = true;
        label.AppearanceHovered.Options.UseForeColor = true;
    }
}

/// <summary>
/// Erweiterungsmethoden für alle RepositoryItemButtonEdit-Controls (Combobox etc.)
/// </summary>
[SupportedOSPlatform("windows")]
public static class RepositoryItemButtonEditEx
{

    /// <summary>
    /// Fügt einen Button hinzu
    /// </summary>
    /// <param name="name">Name (Standard ist null/kein Name)</param>
    /// <param name="caption">Beschriftung (Standard ist null/keine Beschriftung)</param>
    /// <param name="image">Bild</param>
    /// <param name="tooltip">Tooltip (Standard ist null/kein Tooltip)</param>
    /// <param name="showleft">links anzeigen (Standard ist false)</param>
    /// <param name="pos">Position an der der Button eingefügt werden soll (Standard ist -1 = am Ende)</param>
    /// <param name="editor">editor</param>
    /// <param name="imagesize">Grtöße des Symbols (Standard ist 16,16)</param>
    /// <param name="enabled">Schaltfläche ist enabled (Standard ist true)</param>
    public static EditorButton AddButton(this RepositoryItemButtonEdit editor, Symbol image, Size? imagesize = null, string? name = null, bool showleft = false, string? caption = null, SuperToolTip? tooltip = null, int pos = -1, bool enabled = true)
    {
        return addButton(editor, null, null, image, null, imagesize, name, showleft, caption, tooltip, pos, enabled);
    }

    /// <summary>
    /// Fügt einen Button hinzu
    /// </summary>
    /// <param name="name">Name (Standard ist null/kein Name)</param>
    /// <param name="caption">Beschriftung (Standard ist null/keine Beschriftung)</param>
    /// <param name="image">Bild</param>
    /// <param name="tooltip">Tooltip (Standard ist null/kein Tooltip)</param>
    /// <param name="showleft">links anzeigen (Standard ist false)</param>
    /// <param name="pos">Position an der der Button eingefügt werden soll (Standard ist -1 = am Ende)</param>
    /// <param name="editor">editor</param>
    /// <param name="imagesize">Grtöße des Symbols (Standard ist 16,16)</param>
    /// <param name="enabled">Schaltfläche ist enabled (Standard ist true)</param>
    public static EditorButton AddButton(this RepositoryItemButtonEdit editor, ObjectImages image, Size? imagesize = null, string? name = null, bool showleft = false, string? caption = null, SuperToolTip? tooltip = null, int pos = -1, bool enabled = true)
    {
        return addButton(editor, null, null, null, image, imagesize, name, showleft, caption, tooltip, pos, enabled);
    }

    /// <summary>
    /// Fügt einen Button hinzu
    /// </summary>
    /// <param name="name">Name (Standard ist null/kein Name)</param>
    /// <param name="caption">Beschriftung (Standard ist null/keine Beschriftung)</param>
    /// <param name="image">Bild</param>
    /// <param name="tooltip">Tooltip (Standard ist null/kein Tooltip)</param>
    /// <param name="showleft">links anzeigen (Standard ist false)</param>
    /// <param name="pos">Position an der der Button eingefügt werden soll (Standard ist -1 = am Ende)</param>
    /// <param name="editor">editor</param>
    /// <param name="imagesize">Grtöße des Symbols (Standard ist 16,16)</param>
    /// <param name="enabled">Schaltfläche ist enabled (Standard ist true)</param>
    public static EditorButton AddButton(this RepositoryItemButtonEdit editor, SvgImage image, Size? imagesize = null, string? name = null, bool showleft = false, string? caption = null, SuperToolTip? tooltip = null, int pos = -1, bool enabled = true)
    {
        return addButton(editor, null, image, null, null, imagesize, name, showleft, caption, tooltip, pos, enabled);
    }


    /// <summary>
    /// Fügt einen Button hinzu
    /// </summary>
    /// <param name="name">Name (Standard ist null/kein Name)</param>
    /// <param name="caption">Beschriftung (Standard ist null/keine Beschriftung)</param>
    /// <param name="image">Bild</param>
    /// <param name="tooltip">Tooltip (Standard ist null/kein Tooltip)</param>
    /// <param name="showleft">links anzeigen (Standard ist false)</param>
    /// <param name="pos">Position an der der Button eingefügt werden soll (Standard ist -1 = am Ende)</param>
    /// <param name="editor">editor</param>
    /// <param name="imagesize">Grtöße des Symbols (Standard ist 16,16)</param>
    /// <param name="enabled">Schaltfläche ist enabled (Standard ist true)</param>
    public static EditorButton AddButton(this RepositoryItemButtonEdit editor, Image image, Size? imagesize = null, string? name = null, bool showleft = false, string? caption = null, SuperToolTip? tooltip = null, int pos = -1, bool enabled = true)
    {
        return addButton(editor, image, null, null, null, imagesize, name, showleft, caption, tooltip, pos, enabled);
    }

    private static EditorButton addButton(RepositoryItemButtonEdit editor, Image? image, SvgImage? svg, Symbol? symbol, ObjectImages? objectimage, Size? imagesize, string? name, bool showleft, string? caption, SuperToolTip? tooltip, int pos, bool enabled)
    {
        EditorButton button = new(ButtonPredefines.Glyph)
        {
            Tag = name
        };

        if (symbol != null)
            button.ImageOptions.SvgImage = UI.GetImage((Symbol)symbol);
        else if (objectimage != null)
            button.ImageOptions.SvgImage = UI.GetObjectImage((ObjectImages)objectimage);
        else if (svg != null)
            button.ImageOptions.SvgImage = svg;
        else if (image != null)
            button.ImageOptions.Image = image;

        button.ImageOptions.SvgImageSize = imagesize ?? new Size(16, 16);

        button.SuperTip = tooltip;
        button.IsLeft = showleft;
        button.Caption = caption;
        button.Enabled = enabled;

        if (pos > -1)
            editor.Buttons.Insert(pos, button);
        else
            editor.Buttons.Add(button);

        button.Enabled = enabled;

        return button;
    }
}

/// <summary>
/// Erweiterungsmethoden für alle ButtonEdit-Controls (Combobox etc.)
/// </summary>
[SupportedOSPlatform("windows")]
public static class ButtonEditEx
{
    /// <summary>
    /// Fügt einen Button hinzu
    /// </summary>
    /// <param name="name">Name (Standard ist null/kein Name)</param>
    /// <param name="caption">Beschriftung (Standard ist null/keine Beschriftung)</param>
    /// <param name="image">Bild</param>
    /// <param name="tooltip">Tooltip (Standard ist null/kein Tooltip)</param>
    /// <param name="showleft">links anzeigen (Standard ist false)</param>
    /// <param name="pos">Position an der der Button eingefügt werden soll (Standard ist -1 = am Ende)</param>
    /// <param name="editor">editor</param>
    /// <param name="imagesize">Grtöße des Symbols (Standard ist 16,16)</param>
    /// <param name="enabled">Schaltfläche ist enabled (Standard ist true)</param>
    public static EditorButton AddButton(this ButtonEdit editor, Symbol image, Size? imagesize = null, string? name = null, bool showleft = false, string? caption = null, SuperToolTip? tooltip = null, int pos = -1, bool enabled = true)
    {
        return addButton(editor, null, null, image, null, imagesize, name, showleft, caption, tooltip, pos, enabled);
    }

    /// <summary>
    /// Fügt einen Button hinzu
    /// </summary>
    /// <param name="name">Name (Standard ist null/kein Name)</param>
    /// <param name="caption">Beschriftung (Standard ist null/keine Beschriftung)</param>
    /// <param name="tooltip">Tooltip (Standard ist null/kein Tooltip)</param>
    /// <param name="showleft">links anzeigen (Standard ist false)</param>
    /// <param name="pos">Position an der der Button eingefügt werden soll (Standard ist -1 = am Ende)</param>
    /// <param name="editor">editor</param>
    /// <param name="enabled">Schaltfläche ist enabled (Standard ist true)</param>
    public static EditorButton AddButton(this ButtonEdit editor, string? name = null, bool showleft = false, string? caption = null, SuperToolTip? tooltip = null, int pos = -1, bool enabled = true)
    {
        return addButton(editor, null, null, null, null, null, name, showleft, caption, tooltip, pos, enabled);
    }

    /// <summary>
    /// Fügt einen Button hinzu
    /// </summary>
    /// <param name="name">Name (Standard ist null/kein Name)</param>
    /// <param name="caption">Beschriftung (Standard ist null/keine Beschriftung)</param>
    /// <param name="image">Bild</param>
    /// <param name="tooltip">Tooltip (Standard ist null/kein Tooltip)</param>
    /// <param name="showleft">links anzeigen (Standard ist false)</param>
    /// <param name="pos">Position an der der Button eingefügt werden soll (Standard ist -1 = am Ende)</param>
    /// <param name="editor">editor</param>
    /// <param name="imagesize">Grtöße des Symbols (Standard ist 16,16)</param>
    /// <param name="enabled">Schaltfläche ist enabled (Standard ist true)</param>
    public static EditorButton AddButton(this ButtonEdit editor, ObjectImages image, Size? imagesize = null, string? name = null, bool showleft = false, string? caption = null, SuperToolTip? tooltip = null, int pos = -1, bool enabled = true)
    {
        return addButton(editor, null, null, null, image, imagesize, name, showleft, caption, tooltip, pos, enabled);
    }

    /// <summary>
    /// Fügt einen Button hinzu
    /// </summary>
    /// <param name="name">Name (Standard ist null/kein Name)</param>
    /// <param name="caption">Beschriftung (Standard ist null/keine Beschriftung)</param>
    /// <param name="image">Bild</param>
    /// <param name="tooltip">Tooltip (Standard ist null/kein Tooltip)</param>
    /// <param name="showleft">links anzeigen (Standard ist false)</param>
    /// <param name="pos">Position an der der Button eingefügt werden soll (Standard ist -1 = am Ende)</param>
    /// <param name="editor">editor</param>
    /// <param name="imagesize">Grtöße des Symbols (Standard ist 16,16)</param>
    /// <param name="enabled">Schaltfläche ist enabled (Standard ist true)</param>
    public static EditorButton AddButton(this ButtonEdit editor, SvgImage image, Size? imagesize = null, string? name = null, bool showleft = false, string? caption = null, SuperToolTip? tooltip = null, int pos = -1, bool enabled = true)
    {
        return addButton(editor, null, image, null, null, imagesize, name, showleft, caption, tooltip, pos, enabled);
    }


    /// <summary>
    /// Fügt einen Button hinzu
    /// </summary>
    /// <param name="name">Name (Standard ist null/kein Name)</param>
    /// <param name="caption">Beschriftung (Standard ist null/keine Beschriftung)</param>
    /// <param name="image">Bild</param>
    /// <param name="tooltip">Tooltip (Standard ist null/kein Tooltip)</param>
    /// <param name="showleft">links anzeigen (Standard ist false)</param>
    /// <param name="pos">Position an der der Button eingefügt werden soll (Standard ist -1 = am Ende)</param>
    /// <param name="editor">editor</param>
    /// <param name="imagesize">Grtöße des Symbols (Standard ist 16,16)</param>
    /// <param name="enabled">Schaltfläche ist enabled (Standard ist true)</param>
    public static EditorButton AddButton(this ButtonEdit editor, Image image, Size? imagesize = null, string? name = null, bool showleft = false, string? caption = null, SuperToolTip? tooltip = null, int pos = -1, bool enabled = true)
    {
        return addButton(editor, image, null, null, null, imagesize, name, showleft, caption, tooltip, pos, enabled);
    }

    private static EditorButton addButton(ButtonEdit editor, Image? image, SvgImage? svg, Symbol? symbol, ObjectImages? objectimage, Size? imagesize, string? name, bool showleft, string? caption, SuperToolTip? tooltip, int pos, bool enabled)
    {
        EditorButton button = new(ButtonPredefines.Glyph)
        {
            Tag = name
        };

        if (symbol != null)
            button.ImageOptions.SvgImage = UI.GetImage((Symbol)symbol);
        else if (objectimage != null)
            button.ImageOptions.SvgImage = UI.GetObjectImage((ObjectImages)objectimage);
        else if (svg != null)
            button.ImageOptions.SvgImage = svg;
        else if (image != null)
            button.ImageOptions.Image = image;

        button.ImageOptions.SvgImageSize = imagesize ?? new Size(16, 16);

        button.SuperTip = tooltip;
        button.IsLeft = showleft;
        button.Caption = caption;
        button.Enabled = enabled;

        if (pos > -1)
            editor.Properties.Buttons.Insert(pos, button);
        else
            editor.Properties.Buttons.Add(button);

        button.Enabled = enabled;

        return button;
    }
}
