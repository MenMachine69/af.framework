using DevExpress.Utils;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor zum Bearbeiten von Filterbedingungen für Models
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(false)]
[DefaultBindingProperty("FilterString")]
public sealed class AFEditFilterExpression : DevExpress.DataAccess.UI.FilterEditorControl
{
    private Type? _modelType = null;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditFilterExpression(Type modelType)
    {
        if (UI.DesignMode) return;

        ActiveView = DevExpress.XtraFilterEditor.FilterEditorActiveView.Visual;
        ShowGroupCommandsIcon = true;
        ShowOperandTypeIcon = true;
        ShowDateTimeFunctions = DevExpress.XtraEditors.DateTimeFunctionsShowMode.Advanced;
        ShowFunctions = true;
        ShowCustomFunctions = DefaultBoolean.True;

        ModelType = modelType;
    }

    /// <summary>
    /// Typ der Models, die zur Auswahl angezeigt werden sollen.
    ///
    /// Der Typ muss dein Feld mit dem Attribut eSystemFieldFlag.Folder enthalten, in dem die Namen der Ordner abgelegt sind.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Type? ModelType
    {
        get => _modelType;
        set
        {
            _modelType = value;

            if (_modelType == null || _modelType == typeof(Nullable))
            {
                SourceControl = null;
                return;
            }

            SourceControl = Activator.CreateInstance(_modelType);
        }
    }
}