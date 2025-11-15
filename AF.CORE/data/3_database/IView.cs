namespace AF.DATA;

/// <summary>
/// Schnittstelle für Klassen, die als Views in einer Datenbank gespeichert werden.
///
/// Wenn BaseViewData verwendet wird, ist es nicht notwendig, diese Schnittstelle
/// in eigenen Klassen zu implementieren, da sie bereits in BaseViewData implementiert ist. 
/// </summary>
public interface IView : IDataObject
{

}