using AF.MVC;

namespace AF.WINFORMS.DX;

/// <summary>
/// Model des Moduls 'Suchergebnisse'
/// </summary>
[AFContext("Suchergebnisse")]
public class ModulSearchResults : DataObjectBase
{
    private static readonly Guid _id = new("{A3D37E8F-6050-425B-A9CF-271CB1DEB32E}");

    /// <summary>
    /// GUID des Moduls
    /// </summary>
    public override Guid PrimaryKey => _id;

    /// <inheritdoc />
    public override string ToString() { return "Suchergebnisse"; }
}