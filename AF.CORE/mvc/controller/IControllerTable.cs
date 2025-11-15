namespace AF.MVC;

/// <summary>
/// Schnittstelle für einen Controller für ITable-Modelle.
/// </summary>
public interface IControllerTable<TModel, TModelLargeView, TModelSmallView> : IController<TModel>
    where TModel : class, ITable, new()
    where TModelLargeView : class, IDataObject, new()
    where TModelSmallView : class, IDataObject, new()
{
    /// <summary>
    /// Liste aller vom Controller verwalteten Objekte lesen
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn keine Objekte vorhanden sind)</returns>
    new BindingList<TModel> ReadList();

    /// <summary>
    /// Liste der vom Controller verwalteten Objekte lesen
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new BindingList<TModel> ReadList(string query, params object[] args);

    /// <summary>
    /// Liste der vom Controller verwalteten Objekte lesen
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new BindingList<TModel> ReadList(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes, vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new TModel? ReadSingle(string query, params object[] args);

    /// <summary>
    /// Einzelnes, vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new TModel? ReadSingle(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes, vom Controller verwaltetes Objekt lesen (anganhd des PrimaryKey des Objekts)
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    new TModel? Load(Guid guid);

    /// <summary>
    /// Liste aller ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn keine Objekte vorhanden sind)</returns>
    new BindingList<TModelLargeView> ReadLargeList();

    /// <summary>
    /// Liste der ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new BindingList<TModelLargeView> ReadLargeList(string query, params object[] args);

    /// <summary>
    /// Liste der ausführlichen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new BindingList<TModelLargeView> ReadLargeList(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new TModelLargeView? ReadLargeSingle(string query, params object[] args);

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new TModelLargeView? ReadLargeSingle(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes ausführliches Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (anhand des PrimaryKey des Objekts)
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    new TModelLargeView? LoadLarge(Guid guid);

    /// <summary>
    /// Liste aller kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <returns>Liste der Objekte</returns>
    new BindingList<TModelSmallView> ReadSmallList();

    /// <summary>
    /// Liste der kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new BindingList<TModelSmallView> ReadSmallList(string query, params object[] args);

    /// <summary>
    /// Liste der kleinen Objekte (meist aus View) lesen, die der Controller verwaltet.
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new BindingList<TModelSmallView> ReadSmallList(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new TModelSmallView? ReadSmallSingle(string query, params object[] args);

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new TModelSmallView? ReadSmallSingle(ReadOptions? options = null, string? query = null, params object[] args);

    /// <summary>
    /// Einzelnes kleines Objekt (meist aus View), vom Controller verwaltetes Objekt lesen (anhand des PrimaryKey des Objekts)
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    new TModelSmallView? LoadSmall(Guid guid);

    /// <summary>
    /// Liste aller Objekte über die Datenbankverbindung des Controllers lesen.
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new BindingList<T> ReadAnyList<T>() where T : class, IDataObject, new();

    /// <summary>
    /// Liste beliebiger Objekte über die Datenbankverbindung des Controllers lesen.
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new BindingList<T> ReadAnyList<T>(string query, params object[] args) where T : class, IDataObject, new();


    /// <summary>
    /// Liste beliebiger Objekte über die Datenbankverbindung des Controllers lesen.
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new BindingList<T> ReadAnyList<T>(ReadOptions? options = null, string? query = null, params object[] args) where T : class, IDataObject, new();

    /// <summary>
    /// Einzelnes beliebiges Objekt über die Datenbankverbindung des Controllers lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new T? ReadAnySingle<T>(string query, params object[] args) where T : class, IDataObject, new();

    /// <summary>
    /// Einzelnes beliebiges Objekt über die Datenbankverbindung des Controllers lesen (erstes Objekt, dass den Bedingungen entspricht)
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="options">Optionen</param>
    /// <param name="query">Bedingung</param>
    /// <param name="args">Parameter</param>
    /// <returns>Liste der Objekte (leere Liste, wenn kein Objekt den Bedingungen entspricht)</returns>
    new T? ReadAnySingle<T>(ReadOptions? options = null, string? query = null, params object[] args) where T : class, IDataObject, new();


    /// <summary>
    /// Beliebiges Objekt über die Datenbankverbindung des Controllers lesen (anhand des PrimaryKey des Objekts)
    /// 
    /// ACHTUNG! Diese Methode sollte nur in Ausnahmefällen (z.B. für zusätzliche Views neben 'Small' und 'Large' verwendet werden. 
    /// Ansonsten ist der Controller des betreffenden Objekt-Typs zu bevorzugen!
    /// </summary>
    /// <param name="guid">PrimaryKey des Objekts</param>
    /// <returns>das geladene Objekt oder NULL</returns>
    new T? LoadAny<T>(Guid guid) where T : class, IDataObject, new();
}