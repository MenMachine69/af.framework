namespace AF.MVC;

/// <summary>
/// Filter for use in browser etc. to hide archived records
/// </summary>
[SupportedOSPlatform("windows")]
public partial class AFFilterArchived : AFUserControl, IViewFilterPlugin
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AFFilterArchived()
    {
        InitializeComponent();
    }

    #region Implementation of IFilterPlugin

    /// <inheritdoc />
    public string GetFilterString(out object[] parameters)
    {
        // only if not checked a filter is required
        string ret = (chkSYS_ARCHIVED.Checked ? "" : @"SYS_ARCHIVED = ?");

        parameters = [chkSYS_ARCHIVED.Checked];

        return ret;
    }

    /// <summary>
    /// raise the FilterChanged event if checked was changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void checkedChanged(object sender, EventArgs e)
    {
        FilterChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public event EventHandler? FilterChanged;

    #endregion
}

