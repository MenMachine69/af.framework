using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Eingabe numerischer Integer Werte mit auf/ab Pfeilen zum Erhöhen/Verkleinern des Wertes.
///
/// <see cref="DevExpress.XtraEditors.SpinEdit"/>
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
public class AFEditSpin : SpinEdit
{
    /// <summary>
    /// Auswählbaren Bereich festlegen.
    /// </summary>
    /// <param name="von"></param>
    /// <param name="bis"></param>
    /// <returns></returns>
    public AFEditSpin SetRange(decimal von, decimal bis)
    {
        Properties.MaxValue = bis;
        Properties.MinValue = von;

        return this;
    }
}

/// <summary>
/// Eingabe numerischer Integer Werte mit auf/ab Pfeilen zum Erhöhen/Verkleinern des Wertes.
/// 
/// Dieses Control verfügt über die Möglichkeit der automatischen Datenbindung über die Eigenschaft 'ValueInt'.
/// <see cref="DevExpress.XtraEditors.SpinEdit"/>
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("ValueInt")]
public class AFEditSpinInt : AFEditSpin
{
    /// <inheritdoc />
    public AFEditSpinInt()
    {
        if (UI.DesignMode) return;
        Properties.IsFloatValue = false;
    }

    /// <summary>
    /// Überschriebene Eigenschaft Value, um mit Int32-Werten arbeiten zu können
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int ValueInt
    {
        get => Convert.ToInt32(base.Value);
        set => base.Value = Convert.ToDecimal(value);
    }


}

/// <summary>
/// Eingabe numerischer Prozentwerte mit auf/ab Pfeilen zum Erhöhen/Verkleinern des Wertes.
///
/// Verwendet automatisch die Maske 'p2' (prozentualer Wert mit zwei Kommastellen)
/// <see cref="DevExpress.XtraEditors.SpinEdit"/>
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("ValueInt")]
public class AFEditSpinPercent : AFEditSpin
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditSpinPercent()
    {
        Properties.EditMask = "p2";
        Properties.Mask.UseMaskAsDisplayFormat = true;
        Properties.Mask.EditMask = "p2";
        Properties.Increment = 0.01M;
    }

    /// <summary>
    /// Überschriebene Eigenschaft Value, um mit Prozentwerten-Werten arbeiten zu können
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal ValueInt
    {
        get => base.Value;
        set => base.Value = Convert.ToDecimal(value);
    }
}