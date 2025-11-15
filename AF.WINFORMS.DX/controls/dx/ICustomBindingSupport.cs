namespace AF.WINFORMS.DX;

/// <summary>
/// Interface von Controls, die ein CustomBindingProperty haben,
/// dass statt dem DefaultBindingProperty verwendet werden soll
/// </summary>
public interface ICustomBindingSupport
{  
    /// <summary>
    /// Gibt das CustomBindingProperty zurück 
    /// (Eigenschaft an welche die Datenbindung erfolgen soll)
    /// </summary>
    string? CustomBindingProperty { get; set; } 
}