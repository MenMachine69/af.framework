using System.Text;

namespace AF.MVC;

/// <summary>
/// Liste von ModelLinks, die direkt in DB gespeichert werden kann.
/// </summary>
public class ModelLinkCollection : IList<ModelLink>
{
    private readonly List<ModelLink> links = [];

    /// <inheritdoc/>
    public int Count => links.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(ModelLink item)
    {
        if (!links.Contains(item))
            links.Add(item);
    }
    /// <inheritdoc/>
    public void Clear()
    {
        links.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(ModelLink item)
    {
        return links.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(ModelLink[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (array is not { } linkArray)
            throw new ArgumentException(string.Format(CoreStrings.ERR_WRONGARRAYTYPE, typeof(ModelLink).FullName), nameof(array));

        ((ICollection<ModelLink>)this).CopyTo(linkArray, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<ModelLink> GetEnumerator()
    {
        return links.GetEnumerator();
    }

    /// <inheritdoc/>
    public bool Remove(ModelLink item)
    {
        return links.Remove(item);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return links.GetEnumerator();
    }


    /// <inheritdoc />
    public int IndexOf(ModelLink item)
    {
        return links.IndexOf(item);
    }

    /// <inheritdoc />
    public void Insert(int index, ModelLink item)
    {
        links.Insert(index, item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        links.RemoveAt(index);
    }

    /// <inheritdoc />
    public ModelLink this[int index]
    {
        get => links[index];
        set => links[index] = value;
    }

    /// <summary>
    /// Wandelt den Inhalt der Collection in ein ByteArray um
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        StringBuilder sb = StringBuilderPool.GetStringBuilder();
        Dictionary<Type, int> buffered = [];

        foreach (ModelLink link in links)
        {
            if (!buffered.ContainsKey(link.ModelType))
                buffered.Add(link.ModelType, link.ModelType.GetTypeDescription().Table?.TableId ?? -1);
            
            sb.Append(buffered[link.ModelType]);
            sb.Append('\t');
            sb.Append('"');
            sb.Append(link.ModelID);
            sb.Append('"');
            sb.Append('\t');
            sb.Append('"');
            sb.Append(link.ModelCaption.ToSingleLine());
            sb.Append('"');
            sb.Append('\r');
            sb.Append('\n');
        }

        var ret = sb.ToString();
        StringBuilderPool.ReturnStringBuilder(sb);

        return ret;
    }

    /// <summary>
    /// Stellt den Inhalt aus einem String wieder her, der vorher mit ToString erzeugt wurde.
    /// </summary>
    /// <param name="data">einzulesende Daten (CSV der ModelLinks, dass mit ToString gespeichert wurde).</param>
    /// <param name="silent">true = Fehlermeldungen unterdrücken (Standard)</param>
    public void FromString(string data, bool silent = true)
    {
        Dictionary<string, Type?> buffered = [];

        string[] lines = data.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries );
        foreach (var line in lines)
        {
            var rowdata = line.SplitCsv(separator: '\t');
            
            if (rowdata.Length < 3)
                continue;

            try
            {
                if (!buffered.ContainsKey(rowdata[0]))
                {
                    var tdesc = TypeDescription.GetTypeDescriptionById(int.Parse(rowdata[0]));

                    buffered.Add(rowdata[0], tdesc?.Type);
                }

                if (buffered[rowdata[0]] != null)
                    Add(new ModelLink(new Guid(rowdata[1][1..^1]), rowdata[2].Length > 2 ? rowdata[2][1..^1] : "", buffered[rowdata[0]]!));
            }
            catch
            {
                if (silent)
                    continue;

                AFCore.App.ShowErrorOk("FEHLER\r\nBeim Wiederherstellen einer Verknüpfung (ModelLink) mit einem Datensatz trat ein Fehler auf.");
            }
        }
    }
}