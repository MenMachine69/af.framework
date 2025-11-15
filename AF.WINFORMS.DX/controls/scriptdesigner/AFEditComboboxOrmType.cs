namespace AF.WINFORMS.DX;

/// <summary>
/// Combobox zur Auswahl eines ORM-Typs
/// </summary>
[DesignerCategory("Code")]
public class AFEditComboboxOrmType : AFEditCombo
{
    /// <summary>
    /// Lädt die verfügbaren Einträge
    /// </summary>
    /// <param name="databases">Datenbanken, deren Einträge geladen werden sollen</param>
    /// <param name="onTypeAdded">Action, die nach dem Hinzufügen eines Typs ausgeführt wird (erlaubt Anpassungen).</param>
    public void Load(Tuple<IDatabase, string>[] databases, Action<TypeDescription, Type>? onTypeAdded = null)
    {
        List<ListItem> items = [];

        // Filtern nach Namespaces, FOR Schleife ist schneller als foreach!
        foreach (var database in databases.OrderBy(db => db.Item2))
        {
            foreach (var baseTableType in database.Item1.Configuration.BaseTableTypes)
            {
                foreach (var tableType in baseTableType.GetChildTypesOf().OrderBy(typ => typ.FullName))
                {
                    var tdesc = tableType.GetTypeDescription();
                    items.Add(new() { Caption = tdesc.Name + " (" + tableType.FullName + ")", Value = tableType });
                    onTypeAdded?.Invoke(tdesc, tableType);
                }
            }
        }

        this.Fill(items);
    }
}

