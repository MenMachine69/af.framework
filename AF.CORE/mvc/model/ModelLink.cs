namespace AF.MVC;

/// <summary>
/// Beschreibung eines Modells
/// </summary>
public class ModelLink
{
    private IModel? _model;

    /// <summary>
    /// Ein leerer ModelLink
    /// </summary>
    public static ModelLink Empty => new(Guid.Empty, "", typeof(Nullable)); 

    /// <summary>
    /// Erstellt einen neuen Link für ein Modell
    /// </summary>
    /// <param name="modelID">ID des Modells</param>
    /// <param name="modelCaption">Beschriftung des Modells (um den Link an verschiedenen Stellen anzuzeigen)</param>
    /// <param name="modelType">Typ des Modells</param>
    public ModelLink(Guid modelID, string modelCaption, Type modelType)
    {
        ModelType = modelType;
        ModelCaption = modelCaption;
        ModelID = modelID;
    }


    /// <summary>
    /// Model type
    /// </summary>
    public Type ModelType { get; set; }

    /// <summary>
    /// Text für das Modell anzeigen
    /// </summary>
    public string ModelCaption { get; set; }

    /// <summary>
    /// Gibt eine Zeichenkette zurück, die das aktuelle Objekt darstellt.
    /// </summary>
    /// <returns>Eine Zeichenkette, die das aktuelle Objekt darstellt.</returns>
    public override string ToString()
    {
        return ModelCaption;
    }

    /// <summary>
    /// Type Name
    /// </summary>
    public string ModelTypeName => ModelType.GetTypeDescription().Name;

    /// <summary>
    /// ID des Modells
    /// </summary>
    public Guid ModelID { get; set; }


    /// <summary>
    /// das Modell selbst - kann Null sein
    /// </summary>
    public IModel? Model
    {
        get
        {
            if (_model != null) return _model;

            if (_model == null && ModelID.IsEmpty()) return null;
            
            _model = ModelType.GetController().ReadSingle(ModelID);

            return _model;
        }
        set => _model = value;
    }


    /// <summary>
    /// Aktuelles Detail, wenn eine Detailansicht aktiv ist
    /// </summary>
    public Type? CurrentDetailType { get; set; }

    /// <summary>
    /// Erstellen eines Lesezeichens für das Modell
    /// </summary>
    /// <returns>das Lesezeichen</returns>
    public ModelBookmark? GetBookmark()
    {
        if (ModelID == Guid.Empty) return null;

        return new ModelBookmark(ModelType, ModelID, ModelCaption)
        {
            CurrentDetailType = CurrentDetailType,
            TypeName = ModelType.GetTypeDescription().Name
        };
    }
}