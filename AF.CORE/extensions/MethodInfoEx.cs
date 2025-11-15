using System.Reflection;
using System.Text;
using System.Xml;

namespace AF.CORE;

/// <summary>
/// Erweiterungsmethoden für die Klasse System.Reflection.MethodInfo
/// </summary>
public static class MethodInfoEx
{
    /// <summary>
    /// Liefert den Summary-Text der Methode, wenn die XML-Dokumentation der Assembly im gleichen Verzeichnis liegt.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="parameterdesc">Beschreibung der Aufrufparameter</param>
    /// <param name="returndesc">Beschreibung des return Wertes</param>
    /// <returns></returns>
    public static string GetSummary(this MethodInfo method, out List<Tuple<string, string>> parameterdesc, out string returndesc)
    {
        StringBuilder ret = new();
        parameterdesc = [];
        returndesc = "";

        if (method.DeclaringType == null) return ret.ToString();

        string path = method.DeclaringType.Assembly.Location;
        
        if (path.Contains(".") == false) return ret.ToString();

        path = path[..path.LastIndexOf(".", StringComparison.Ordinal)] + ".xml";

        if (!File.Exists(path)) return ret.ToString();

        XmlDocument doc = new();
        doc.Load(path);
        string search = "M:" + method.DeclaringType.FullName + "." + method.Name;
        XmlNode? node = doc.SelectSingleNode("//member[starts-with(@name, '" + search + "')]");

        if (node == null) return ret.ToString();

        foreach (XmlNode childnode in node.ChildNodes)
        {
            if (childnode.Name == "summary")
                ret.Append(childnode.InnerText.Trim());

            if (childnode.Name == "param")
                parameterdesc.Add(new(childnode.Attributes?["name"]?.Value ?? "<unbekannt>", childnode.InnerText.Trim()));

            if (childnode.Name == "returns")
                returndesc = childnode.InnerText.Trim();
        }

        ret.Replace("    ", " ");
        ret.Replace("   "," ");
        ret.Replace("  "," ");

        return string.Join(Environment.NewLine, ret.ToString().Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()));
    }

    /// <summary>
    /// Liefert den Summary-Text des Propertys, wenn die XML-Dokumentation der Assembly im gleichen Verzeichnis liegt.
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static string GetSummary(this PropertyInfo property)
    {
        StringBuilder ret = new();

        if (property.DeclaringType == null) return ret.ToString();

        string path = property.DeclaringType.Assembly.Location;
        
        if (path.Contains(".") == false) return ret.ToString();

        path = path[..path.LastIndexOf(".", StringComparison.Ordinal)] + ".xml";

        if (!File.Exists(path)) return ret.ToString();

        XmlDocument doc = new();
        doc.Load(path);
        string search = "P:" + property.DeclaringType.FullName + "." + property.Name;
        XmlNode? node = doc.SelectSingleNode("//member[starts-with(@name, '" + search + "')]");

        if (node == null) return ret.ToString();

        foreach (XmlNode childnode in node.ChildNodes)
        {
            if (childnode.Name == "summary")
                ret.Append(childnode.InnerText.Trim());
        }

        ret.Replace("  "," ");
        ret.Replace("  "," ");

        return string.Join(Environment.NewLine, ret.ToString().Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()));
    }

    /// <summary>
    /// Liefert die vollstündige Signatur einer Methode
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static string GetSignature(this MethodInfo method)
    {
        string signature = method.Name + "(";
        bool first = true;
        foreach (ParameterInfo paraifo in method.GetParameters().OrderBy(p => p.Position))
        {
            signature += (first ? "" : ", ") + (paraifo.ParameterType.Name.EndsWith("&") ? "ref " : "") + paraifo.ParameterType.Name.Replace("&","") + " " + paraifo.Name;
            first = false;
        }
        signature += ")";

        return signature;
    }
}