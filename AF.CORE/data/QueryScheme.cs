using System.Drawing;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AF.DATA;

/// <summary>
/// Modell zum Speichern einer Query Konfiguration (z.B. im Designer)
/// </summary>
[Serializable]
public class QueryScheme
{
    /// <summary>
    /// Constructor
    /// </summary>
    public QueryScheme() { }


    /// <summary>
    /// Im Schema verwendete Tabellen
    /// </summary>
    public BindingList<QuerySchemeTable> TABLES { get; set; } = [];

    /// <summary>
    /// Im Schema verwendete Joins
    /// </summary>
    public BindingList<QuerySchemeJoin> JOINS { get; set; } = [];


    /// <summary>
    /// Tabelleninformationen
    /// </summary>
    [Serializable]
    public class QuerySchemeTable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public QuerySchemeTable() { }

        /// <summary>
        /// Standort im Designer
        /// </summary>
        public Point? LOCATION { get; set; }

        /// <summary>
        /// Größe des Designer-Elements
        /// </summary>
        public Size? SIZE { get; set; }

        /// <summary>
        /// Tabellenname
        /// </summary>
        public string TABLENAME { get; set; } = string.Empty;

        /// <summary>
        /// Tabellenname
        /// </summary>
        public string TABLESCHEME { get; set; } = string.Empty;

        /// <summary>
        /// Alias name
        /// </summary>
        public string TABLEALIAS { get; set; } = string.Empty;


        /// <summary>
        /// Alias name
        /// </summary>
        public Guid TABLEID { get; set; } = Guid.Empty;



        /// <summary>
        /// Felder
        /// </summary>
        public BindingList<QuerySchemeField> FIELDS { get; set; } = [];

        /// <inheritdoc />
        public override string ToString()
        {
            return $@"{TABLENAME} ({TABLEALIAS})";
        }
    }

    /// <summary>
    /// Feld in einer Tabelle
    /// </summary>
    [Serializable]
    public class QuerySchemeField
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public QuerySchemeField() { }

        /// <summary>
        /// Feldname
        /// </summary>
        public string FIELDNAME { get; set; } = string.Empty;

        /// <summary>
        /// ID des Feldes
        /// </summary>
        public Guid FIELDID { get; set; } = Guid.Empty;


        /// <summary>
        /// Where Bedingung für das Feld
        /// </summary>
        public string FIELDWHERE { get; set; } = string.Empty;

        /// <summary>
        /// Alias
        /// </summary>
        public string FIELDALIAS { get; set; } = string.Empty;

        /// <summary>
        /// Feldtyp
        /// </summary>
        public string FIELDTYPE { get; set; } = string.Empty;

        /// <summary>
        /// ist ein berechnetes Feld
        /// </summary>
        public bool FIELDISCALCULATED { get; set; }

        /// <summary>
        /// Ist ausgewähltes Feld
        /// </summary>
        public bool FIELDISSELECTED { get; set; }

        /// <summary>
        /// Ausdruck für das Feld
        /// </summary>
        public string FIELDEXPRESSION { get; set; } = string.Empty;

        /// <summary>
        /// Formel die auf den gelesenen Wert angewendet werden soll
        /// </summary>
        public string FIELDPOSTEXPRESSION { get; set; } = string.Empty;

        /// <summary>
        /// Gibt an, ob nach dem Feld gruppiert werden soll.
        /// </summary>
        public bool FIELDGROUPBY{ get; set; }

        /// <summary>
        /// max. Länge des Feldes in der Datenbank
        /// </summary>
        public int FIELDLENGTH{ get; set; }

        /// <summary>
        /// System-Typ des Feldes in der Datenbank
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public Type FIELDSYSTEMTYPE { get; set; } = typeof(Nullable);

        /// <summary>
        /// Typ des Models als string (für Serialisierung notwendig!)
        /// </summary>
        public string FIELDSYSTEMTYPE_TEXT
        {
            get => FIELDSYSTEMTYPE.FullName ?? "";
            set
            {
                if (string.IsNullOrEmpty(value))
                    FIELDSYSTEMTYPE = typeof(Nullable);
                else
                {
                    var type = TypeEx.FindType(value);
                    if (type != null)
                        FIELDSYSTEMTYPE = type;
                    else
                        throw new NotSupportedException($"Unbekannter Typ {value}.");
                }
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return FIELDNAME + (FIELDALIAS.IsEmpty() ? "" : $@" ({FIELDALIAS})");
        }
    }

    /// <summary>
    /// Definition einer Verknüpfung zwischen zwei Tabellen
    /// </summary>
    [Serializable]
    public class QuerySchemeJoin
    {
        /// <summary>
        /// Zieltabelle
        /// </summary>
        public Guid TABLETARGET { get; set; } = Guid.Empty;

        /// <summary>
        /// Quelltabelle
        /// </summary>
        public Guid TABLESOURCE { get; set; } = Guid.Empty;

        /// <summary>
        /// Quellfeld
        /// </summary>
        public Guid FIELDSOURCE { get; set; } = Guid.Empty;

        /// <summary>
        /// Zielfeld
        /// </summary>
        public Guid FIELDTARGET { get; set; } = Guid.Empty;

        /// <summary>
        /// Art des Joins...
        /// </summary>
        public eJoinType JOINTYPE { get; set; } = eJoinType.LeftJoin;
    }
}