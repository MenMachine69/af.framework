namespace AF.MVC;

/// <summary>
/// View, das als DetailView angezeigt wird
/// </summary>
public interface IViewDetail : IUIElement
{
    /// <summary>
    /// Eine Liste aller Modelle in der DetailView
    /// </summary>
    IBindingList? Models { get; set; }

    /// <summary>
    /// Eine Liste aller ausgewählten Modelle in der DetailView
    /// </summary>
    IEnumerable<IModel>? SelectedModels { get; }

    /// <summary>
    /// Das aktuell ausgewählte Modell in der DetailView
    /// </summary>
    IModel? CurrentModel { get; }

    /// <summary>
    /// Die Seite, die diese DetailView enthält
    /// </summary>
    IViewPage Page { get; set; }

    /// <summary>
    /// Plugin zur Anzeige des ausgewählten Details.
    /// 
    /// Wird die Auswahl verändert, wird das Detail informiert.
    /// </summary>
    IViewModelDetail? PluginDetail { get; set; }

    /// <summary>
    /// Validate the current Detail and return a list of errors if invalid.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    bool IsValid(ValidationErrorCollection errors);
}