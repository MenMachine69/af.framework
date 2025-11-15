namespace AF.MVC;

/// <summary>
/// Interface für einen ModelInspektor
/// </summary>
public interface IModelInspektor : IDisposable
{
    /// <summary>
    /// Das im Inspektor anzuzeigende Model
    /// </summary>
    IModel? CurrentModel { get; set; }

    /// <summary>
    /// den Inspektor anzeigen
    /// </summary>
    /// <param name="owner">übergeordnetes UI Element (z.B. Form)</param>
    void ShowInspektor(object? owner);
}