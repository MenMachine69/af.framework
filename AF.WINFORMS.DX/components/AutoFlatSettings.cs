using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

[Serializable]
internal class AutoFlatSettings
{
    public AutoFlatSettings()
    {
        AutoFlat = false;
        AutoRightAlign = false;
        AutoBold = false;
        ConnectedLabel = null;
    }

    public LabelControl? ConnectedLabel { get; set; }

    public bool AutoFlat { get; set; }

    public bool AutoRightAlign { get; set; }

    public bool AutoBold { get; set; }
}

