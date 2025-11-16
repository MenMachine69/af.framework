using AF.MVC;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;

namespace AF.WINFORMS.DX;

/// <summary>
/// Dialogfenster, dass die Bearbeitung eines IModel in einem IEditor erlaubt.
/// </summary>
[DesignerCategory("Code")]
public sealed class AFEditorForm<TModel> : FormBase where TModel : class, IModel, new()
{
    private readonly IControllerUI? _currentController;
    private readonly Control? _viewEditor;
    private readonly AFLabel? lblDesc;
    private readonly AFButtonPanel buttons;
    private CommandResult? result;

    /// <summary>
    /// Constructor
    /// </summary>
    public AFEditorForm(IModel model, string caption, string? description = "", Control? editorControl = null)
    {
        _currentController = model.GetType().GetController() as IControllerUI;

        if (_currentController == null) throw new Exception($"Es existiert kein Controller für den Typ {model.GetType().FullName}.");

        _viewEditor = editorControl ?? (Control)_currentController.GetUIElement(eUIElement.Editor)!;

        if (_viewEditor == null) throw new Exception($"Es existiert kein Editor (IEditor) für den Typ {model.GetType().FullName}.");

        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;
        Text = caption;
        
        if (description.IsNotEmpty())
        {
            lblDesc = new()
            {
                Text = description!,
                Padding = new(10),
                AutoSizeMode = LabelAutoSizeMode.Vertical,
                Dock = DockStyle.Top,
                AllowHtmlString = true
            };
            Controls.Add(lblDesc);
        }

        buttons = new() { Dock = DockStyle.Bottom };
        buttons.CaptionOk = "SPEICHERN";
        buttons.CaptionCancel = "ABBRECHEN";
        buttons.ButtonCancel.Click += (_, _) =>
        {
            ((IEditor)_viewEditor).Model?.RollBackChanges();

            DialogResult = DialogResult.Cancel;
            Close();
        };
        buttons.ButtonOk.Click += (_, _) =>
        {
            result = _currentController.GetCommand(eCommand.Save)?.Execute(new() { CommandContext = eCommandContext.MasterContext, CommandSource = _viewEditor, Model = ((IEditor)_viewEditor).Model });
            if (result?.Result == eNotificationType.Success)
            {
                result.ResultObject = ((IEditor)_viewEditor).Model;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                if (result != null)
                    HandleResult(result);
            }
        };


        Controls.Add(buttons);

        AFScrollablePanel panelEditor = new() { Dock = DockStyle.Fill };
        _viewEditor.Dock = DockStyle.Fill;
        panelEditor.Controls.Add(_viewEditor);
        Controls.Add(panelEditor);
        panelEditor.BringToFront();

        ((IEditor)_viewEditor).Model = model;

        Size = new Size(((IEditor)_viewEditor).DefaultEditorWidth + Padding.Horizontal,
            ((IEditor)_viewEditor).DefaultEditorHeight + Padding.Vertical + buttons.Height + (lblDesc?.Height ?? 0) + (Height - ClientRectangle.Height));

        if (_currentController.TypeImage is not SvgImage svg) return;

        IconOptions.SvgImage = svg;
        IconOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;


    }

    /// <summary>
    /// Ergebnis der Ausführung des Save-Commands des Controllers
    /// </summary>
    public CommandResult? CommandResult => result;
}

