using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AF.WINFORMS.DX;

/// <summary>
/// Suche für Grids (wie 'Finden'-Funktion)
/// </summary>
public class AFEditGridFind : SearchControl
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditGridFind()
    {
        if (UI.DesignMode) return;

        Properties.NullValuePrompt = "Suche nach...";
        Properties.TextEditStyle = TextEditStyles.Standard;
        Properties.ShowSearchButton = false;

        this.AddButton(UI.GetImage(Symbol.Search), showleft: true, enabled: false);
    }

}

