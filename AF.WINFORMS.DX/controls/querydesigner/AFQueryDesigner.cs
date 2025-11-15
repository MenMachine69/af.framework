using System.Diagnostics;
using System.Text;
using AF.BUSINESS;
using AF.MVC;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using Npgsql;

// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable

namespace AF.WINFORMS.DX;

/// <summary>
/// Designer für SQL-Abfragen
/// </summary>
public partial class AFQueryDesigner : AFDesigner, IVariableConsumer
{
    private readonly AFQueryDesignerCanvas _canvas = new() { Dock = DockStyle.Fill };
    private AFEditMultilineSyntax? _synedit;
    private readonly AFVariableBrowser variableBrowser = null!;
    private readonly AFEditMultilineSyntax source = null!;
    private readonly AFBindingConnector connectorTable = null!;
    private readonly AFBindingConnector connectorField = null!;
    private readonly AFBindingConnector connectorJoin = null!;
    private IQuery? _query;
    private System.Windows.Forms.Timer? recreationTimer;
    private bool needRecreate;
    private bool suspendCodeGeneration;

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFQueryDesigner()
    {
        InitializeComponent();

        if (UI.DesignMode) return;

        components ??= new Container();

        dockDatabase.Size = new Size(300, dockDatabase.Size.Height);

        panelContainerRight.Size = new Size(350, dockDatabase.Size.Height);
        panelContainerBottom.Size = new Size(dockDatabase.Size.Width, 200);

        crDockManager1.Panels.ForEach(p =>
        {
            p.Options.AllowFloating = false;
            p.Options.ShowCloseButton = false;
        });

        variableBrowser = new() { Dock = DockStyle.Fill };
        dockVariablen.Controls.Add(variableBrowser);

        mleOutput.Font = UI.ConsoleFont;

        source = new() { Dock = DockStyle.Fill };
        source.SetMode(eSyntaxMode.SQL);
        source.Editor.ReadOnly = true;
        dockSource.Controls.Add(source);
        source.BringToFront();

        _canvas.Designer = this;
        crPanel1.Controls.Add(_canvas);
        _canvas.BringToFront();

        _canvas.OnJoinSelected = (_, j) =>
        {
            connectorJoin.DataSource = j;
        };

        AFTablePanel tableField = new() { Dock = DockStyle.Fill, UseSkinIndents = true };
        dockFieldProp.Controls.Add(tableField);
        tableField.BeginLayout();
        tableField.FromModel(typeof(DatabaseSchemeField));
        tableField.EndLayout();

        connectorField = new(components) { StartContainerControl = tableField, BindingMode = DataSourceUpdateMode.OnPropertyChanged };

        AFTablePanel tableTable = new() { Dock = DockStyle.Fill, UseSkinIndents = true };
        dockTableProp.Controls.Add(tableTable);
        tableTable.BeginLayout();
        tableTable.FromModel(typeof(DatabaseSchemeTable));
        tableTable.EndLayout();

        connectorTable = new(components) { StartContainerControl = tableTable, BindingMode = DataSourceUpdateMode.OnPropertyChanged };

        AFTablePanel tableJoin = new() { Dock = DockStyle.Fill, UseSkinIndents = true };
        dockJoinProp.Controls.Add(tableJoin);
        tableJoin.BeginLayout();
        tableJoin.FromModel(typeof(DatabaseSchemeJoin));
        tableJoin.EndLayout();

        connectorJoin = new(components) { StartContainerControl = tableJoin, BindingMode = DataSourceUpdateMode.OnPropertyChanged};

        extenderTree.Grid = crdbSchemeBrowser1.TreeView;
        extenderTables.Grid = crdbSchemeBrowser1.GridControlTable;
        extenderFields.Grid = crdbSchemeBrowser1.GridControlFields;
        extenderVariablen.Grid = variableBrowser.ViewVariablen.GridControl;
        extenderPlaceholder.Grid = gridPlaceholder;
        extenderFunctions.Grid = gridFunctions;
        extenderResult.Grid = gridResult;


        extenderTree.SupportDragDrop = true;
        extenderTables.SupportDragDrop = true;
        extenderFields.SupportDragDrop = true;
        extenderVariablen.SupportDragDrop = true;
        extenderPlaceholder.SupportDragDrop = true;
        extenderFunctions.SupportDragDrop = true;

        extenderTree.RequestDragData += (_, e) =>
        {
            if (_canvas.Visible)
                e.DragData = e.DragNode?.Tag as DatabaseSchemeTable;
            else
            {
                if (e.DragNode?.Tag is DatabaseSchemeTable table)
                    e.DragData = table.TABLE_SCHEME.IsEmpty() ? table.TABLE_NAME : $"{table.TABLE_SCHEME}.{table.TABLE_NAME}";

                else if (e.DragNode?.Tag is DatabaseSchemeField field)
                    e.DragData = field.FIELD_NAME;
            }
        };

        extenderTables.RequestDragData += (_, e) =>
        {
            if (_canvas.Visible) return;

            if (e.DraggedRow is DatabaseSchemeTable table)
                e.DragData = table.TABLE_SCHEME.IsEmpty() ? table.TABLE_NAME : $"{table.TABLE_SCHEME}.{table.TABLE_NAME}";
        };

        extenderFields.RequestDragData += (_, e) =>
        {
            if (_canvas.Visible) return;

            if (e.DraggedRow is DatabaseSchemeField field)
                e.DragData = field.FIELD_NAME;
        };

        extenderVariablen.RequestDragData += (_, e) =>
        {
            if (e.DraggedRow is IVariable variable)
                e.DragData = "{"+variable.VAR_NAME+"}";
        };

        extenderPlaceholder.RequestDragData += (_, e) =>
        {
            if (e.DraggedRow is IPlaceholder pholder)
                e.DragData = pholder.Name;
        };

        extenderFunctions.RequestDragData += (_, e) =>
        {
            if (e.DraggedRow is StringParserSnippet snippet)
                e.DragData = snippet.FullName;
        };

        AFGridSetup setup = new()
        {
            AllowAddNew = false,
            AllowEdit = false,
            AllowMultiSelect = false,
            Columns =
            [
                new AFGridColumn() { Caption = "Funktion", Bold = true, ColumnFieldname = nameof(StringParserSnippet.FullName) },
                new AFGridColumn() { Caption = "Beschreibung", Bold = false, ColumnFieldname = nameof(StringParserSnippet.SnippetDescription) }
            ]
        };

        viewFunctions.Setup(setup);

        setup = new()
        {
            AllowAddNew = false,
            AllowEdit = false,
            AllowMultiSelect = false,
            Columns =
            [
                new AFGridColumn() { Caption = "Platzhalter", Bold = true, ColumnFieldname = nameof(IPlaceholder.Name) },
                new AFGridColumn() { Caption = "Beschreibung", Bold = false, ColumnFieldname = nameof(IPlaceholder.Description) }
            ]
        };

        viewPlaceholder.Setup(setup);

        BindingList<IPlaceholder> phliste = [];
        if (AFCore.App.QueryService != null)
            phliste.AddRange(AFCore.Placeholder.Values.ToArray());

        gridPlaceholder.ForceInitialize();

        gridPlaceholder.DataSource = phliste;
        
        menExecute.ImageOptions.SvgImageSize = new(16, 16);
        menExecute.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        menExecute.ImageOptions.SvgImage = UI.GetImage(Symbol.PlayCircle);

        menExecute.ItemClick += (_, _) => { ExecuteQuery(); };

        menExportResult.PaintStyle = BarItemPaintStyle.CaptionGlyph;
        menExportResult.ImageOptions.SvgImageSize = new(16, 16);
        menExportResult.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        menExportResult.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowExportLtr);

        menExportResult.ItemClick += (_, _) => { HandleResult(extenderResult.InvokeExport()); };

        menSchemaExport.ImageOptions.SvgImageSize = new(16, 16);
        menSchemaExport.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        menSchemaExport.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowExportLtr);

        menSchemaImport.ImageOptions.SvgImageSize = new(16, 16);
        menSchemaImport.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        menSchemaImport.ImageOptions.SvgImage = UI.GetImage(Symbol.ArrowImport);

        menSchemaClear.ImageOptions.SvgImageSize = new(16, 16);
        menSchemaClear.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        menSchemaClear.ImageOptions.SvgImage = UI.GetImage(Symbol.Broom);

        menSchemaRemoveTable.ImageOptions.SvgImageSize = new(16, 16);
        menSchemaRemoveTable.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        menSchemaRemoveTable.ImageOptions.SvgImage = UI.GetImage(Symbol.TableDismiss);

        menSchemaRemoveJoin.ImageOptions.SvgImageSize = new(16, 16);
        menSchemaRemoveJoin.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        menSchemaRemoveJoin.ImageOptions.SvgImage = UI.GetImage(Symbol.LinkDismiss);

        menSourceGenerate.ImageOptions.SvgImageSize = new(16, 16);
        menSourceGenerate.ImageOptions.SvgImageColorizationMode = SvgImageColorizationMode.Default;
        menSourceGenerate.ImageOptions.SvgImage = UI.GetImage(Symbol.ReceiptPlay);

        menSchemaExport.ItemClick += (_, _) => { ExportScheme(); };
        menSchemaImport.ItemClick += (_, _) => { ImportScheme(); };

        menSchemaClear.ItemClick += (_, _) => { ClearScheme(); };
        menSchemaRemoveTable.ItemClick += (_, _) => { RemoveTable(); };
        menSchemaRemoveJoin.ItemClick += (_, _) => { RemoveJoin(); };

        menSourceGenerate.ItemClick += (_, _) => { GenerateCode(); };

        crdbSchemeBrowser1.DatabaseSchemeLoaded += (_, _) => { schemaChanged(); };

        toggleLiveGenerate.Checked = true;
    }

    private void schemaChanged()
    {
        if (crdbSchemeBrowser1.CurrentConnection is IDatabase db)
        {
            gridFunctions.DataSource = db.Translator.CustomFunctions;
            dockFunctions.Visibility = DockVisibility.Visible;
        }
        else
            dockFunctions.Visibility = DockVisibility.Hidden;
    }

    /// <summary>
    /// Query ausführen
    /// </summary>
    private void ExecuteQuery()
    {
        if (crdbSchemeBrowser1.CurrentConnection == null)
        {
            MsgBox.ShowErrorOk("ABFRAGE AUSFÜHREN\r\nKeine Datenbank ausgewählt.\r\nWählen SIe eine Datenbank aus, bevor Sie die Abfrage ausführen.");
            return;
        }

        string? query;

        if (DesignerMode)
        {
            if (!GenerateCode(true))
            {
                MsgBox.ShowErrorOk("ABFRAGE AUSFÜHREN\r\nDas Schema für die Abfrage ist ungültig. Bitte korrigieren.");
                return;
            }

            query = source.SourceCode;
        }
        else
            query = _synedit?.SourceCode;

        if (string.IsNullOrEmpty(query))
        {
            MsgBox.ShowErrorOk("ABFRAGE AUSFÜHREN\r\nEs wurde keine auszuführende Abfrage gefunden.");
            return;
        }

        List<VariableUserValue> variablen = [];

        if (Variables.Count > 0 && Variables.OfType<IVariable>().Count(v => v.VAR_READONLY == false) > 0)
        {
            using (AFFormVariableFormular form = new(Variables, description: "Geben Sie die Parameter für die Variablen die in der Abfrage verwendet werden ein, um die Abfrage mit diesen Parametern auszuführen."))
            {
                if (form.ShowDialog(FindForm()) == DialogResult.OK)
                    variablen = form.Result;
                else
                {
                    HandleResult(CommandResult.Warning("Ausführung abgebrochen."));
                    return;
                }
            }
        }
        else
        {


        }

        ToConsole("Beginne mit Abfrage...");

        UI.ShowWait("BITTE WARTEN", "Daten werden geladen..."); // "Loading databases scheme...");
     
        try
        {
            gridResult.DataSource = null;
            viewResult.Columns.Clear();

            // Variablen als Parameter umsetzen...
            Stopwatch watch = new();

            using var conn = crdbSchemeBrowser1.CurrentConnection.Connect();
            using var command = crdbSchemeBrowser1.CurrentConnection.GetCommand(query!, variablen: variablen);

            AddToConsole("Abfrage:\r\n" + command.CommandText);
            watch.Start();
            var table = crdbSchemeBrowser1.CurrentConnection.ExecuteTable(conn, command);
            watch.Stop();
            AddToConsole($"Ergebnis: {table.Rows.Count} Zeilen (in {watch.ElapsedMilliseconds} ms)");

            gridResult.DataSource = table;
            UI.HideWait();

            crDockManager1.ActivePanel = dockResult;
            gridResult.Focus();
        }
        catch (Exception ex)
        {
            UI.HideWait();
            // MsgBox.ShowErrorOk("ABFRAGE AUSFÜHREN\r\nBeim Ausführen der Abfrage trat ein Fehler auf.", ex.Message);

            if (ex is PostgresException pgex)
            {
                AddToConsole("Fehler:\r\n" + pgex.MessageText + "\r\n" + (pgex.Hint ?? ""));
                if (query != null)
                {
                    AddToConsole("ZEILE:");
                    AddToConsole(query.ExtractLine(pgex.Position, out int startpos).Replace("\t", " "));
                    AddToConsole(" ".PadLeft(pgex.Position - startpos - 1) + "^");
                }
            }
            else
                AddToConsole("Fehler:\r\n" + ex.Message);

            crDockManager1.ActivePanel = dockOutputConsole;
        }
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        recreationTimer = new() { Interval = 500 };
        recreationTimer.Tick += (_, _) =>
        {
            recreationTimer.Stop();
            if (needRecreate && !suspendCodeGeneration)
            {
                if (toggleLiveGenerate.Checked)
                    GenerateCode(true);

                needRecreate = false;
            }
            recreationTimer.Start();
        };
        recreationTimer.Start();
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        recreationTimer?.Stop();
        recreationTimer?.Dispose();

        base.OnHandleDestroyed(e);
    }

    /// <summary>
    /// Ausgabe-Console zurücksetzen.
    /// </summary>
    public void ResetConsole()
    {
        mleOutput.Text = "OK";
    }

    /// <summary>
    /// Text zur Ausgabe hinzufügen...
    /// </summary>
    /// <param name="value"></param>
    public void AddToConsole(string value)
    {
        mleOutput.Text += "\r\n" + value;
    }

    /// <summary>
    /// Text in Ausgabe ersetzen
    /// </summary>
    /// <param name="value"></param>
    public void ToConsole(string value)
    {
        mleOutput.Text = value;
    }

    /// <summary>
    /// Den Quellcode für das Schema generieren
    /// </summary>
    /// <param name="silent">Quellcode ohne Rückmeldung generieren.</param>
    /// <param name="errors">Collection, die die Fehler aufnehmen soll</param>
    public bool GenerateCode(bool silent = false, ValidationErrorCollection? errors = null)
    {
        errors ??= [];

        if (_canvas.Elements.Count < 1)
        {
            ResetConsole();
            return true;
        }

        if (suspendCodeGeneration)
            return true;

        var result = QueryDesignerModelController.Instance.GenerateCode(_canvas.Elements.Values.Where(e => e is AFQueryDesignerTable).Select(t => (t as AFQueryDesignerTable)!.Table!).ToArray(), _canvas.Joins, errors);
        bool ret = false;

        if (result.ResultObject is string query)
        {
            source.SourceCode = query;
            mleOutput.Text = "OK";
            ret = true;

            crDockManager1.ActivePanel = dockSource;
        }
        else
        {
            source.SourceCode = "";
            StringBuilder sbout = new();
            sbout.AppendLine("Fehler beim Generieren des Quellcodes.");
            
            foreach (var error in errors)
                sbout.AppendLine(error.Message);

            mleOutput.Text = sbout.ToString();
            source.SourceCode = "<FEHLER IM SCHEMA>";

            crDockManager1.ActivePanel = dockOutputConsole;
        }

        needRecreate = false;

        if (!silent)
            HandleResult(result);

        return ret;
    }

    /// <summary>
    /// Umschalten zwischen Designer und Editor
    /// </summary>
    public void ToggleMode()
    {
        if (!toggleShowCode.Checked && _canvas.Visible)
            return;

        if (toggleShowCode.Checked && (_synedit?.Visible ?? false))
            return;

        if (toggleShowCode.Checked && _canvas.Visible && _canvas.Elements.Count > 0)
        {
            if (MsgBox.ShowQuestionYesNo(WinFormsStrings.MSG_SWITCHTOCODE) == eMessageBoxResult.No)
            {
                toggleShowCode.Checked = false;
                return;
            }
        }

        if (!toggleShowCode.Checked && _synedit != null && _synedit.Visible && _synedit.SourceCode.Trim().Length > 0)
        {
            if (MsgBox.ShowQuestionYesNo(WinFormsStrings.MSG_SWITCHTODESIGNER) == eMessageBoxResult.No)
            {
                toggleShowCode.Checked = true;
                return;
            }
        }

        if (DesignerMode && _synedit == null)
        {
            UI.ShowWait(Properties.Resources.LBL_WAIT, Properties.Resources.LBL_LOADING);

            _synedit = new() { Dock = DockStyle.Fill, Visible = false };
            _synedit.SetMode(eSyntaxMode.SQL);
            crPanel1.Controls.Add(_synedit);
            _synedit.BringToFront();

            if (GenerateCode(true))
                _synedit.SourceCode = source.SourceCode;

            UI.HideWait();
        }

        _synedit!.Visible = !_synedit!.Visible;
        _canvas.Visible = !_canvas.Visible;

        menSchema.Visibility = _canvas.Visible ? BarItemVisibility.Always : BarItemVisibility.Never;
        

        dockFieldProp.Visibility = DesignerMode ? DockVisibility.Visible : DockVisibility.Hidden;
        dockJoinProp.Visibility = DesignerMode ? DockVisibility.Visible : DockVisibility.Hidden;
        dockTableProp.Visibility = DesignerMode ? DockVisibility.Visible : DockVisibility.Hidden;
        dockSource.Visibility = DesignerMode ? DockVisibility.Visible : DockVisibility.Hidden;
    }

    
    /// <summary>
    /// Tabelle löschen...
    /// </summary>
    public void RemoveTable()
    {
        if (!_canvas.Visible) return;

        if (_canvas.ActiveElement == null || _canvas.ActiveElement is not AFQueryDesignerTable table)
        {
            HandleResult(CommandResult.Info("Es ist keine Tabelle im Designer ausgewählt."));
            return;
        }

        if (MsgBox.ShowWarningOkCancel($"TABELLE LÖSCHEN\r\nDie Tabelle <b>{table.Table!.TABLE_NAME} (Alias: {table.Table.TABLE_ALIAS}) und alle sich auf die Tabelle beziehenden Joins werden aus dem Schema gelöscht. Ausführen?") == eMessageBoxResult.Cancel)
            return;

        suspendCodeGeneration = true;

        CurrentTable = null;

        connectorTable.DataSource = null;

        _canvas.Remove(table);

        suspendCodeGeneration = false;
        RaiseNeedRecreate();
    }


    /// <summary>
    /// Join löschen...
    /// </summary>
    public void RemoveJoin()
    {
        if (!_canvas.Visible) return;

        if (_canvas.ActiveJoin == null || _canvas.ActiveJoin is not DatabaseSchemeJoin join)
        {
            HandleResult(CommandResult.Info("Es ist kein Join im Designer ausgewählt."));
            return;
        }

        if (MsgBox.ShowWarningOkCancel($"JOIN LÖSCHEN\r\nDen ausgewählten JOIN aus dem Schema entfernen. Ausführen?") == eMessageBoxResult.Cancel)
            return;

        suspendCodeGeneration = true;

        connectorJoin.DataSource = null;

        _canvas.ActiveJoin = null;

        _canvas.Remove(join);

        suspendCodeGeneration = false;
        RaiseNeedRecreate();
    }

    /// <summary>
    /// Alles löschen...
    /// </summary>
    public void ClearScheme()
    {
        if (!_canvas.Visible) return;

        if (_canvas.Elements.Count < 1)
        {
            HandleResult(CommandResult.Info("Designer enthält keine zu löschende Struktur."));
            return;
        }

        if (MsgBox.ShowWarningOkCancel("SCHEMA LÖSCHEN\r\nEs werden alle aktuell im Schema vorhandenen Tabellen und Joins gelöscht. Ausführen?") == eMessageBoxResult.Cancel)
            return;

        CurrentTable = null;

        connectorTable.DataSource = null;
        connectorJoin.DataSource = null;
        connectorField.DataSource = null;

        _canvas.Clear();

        source.SourceCode = "";
        gridResult.DataSource = null;
        mleOutput.Text = "OK";
    }

    /// <summary>
    /// Schema exportieren
    /// </summary>
    public void ExportScheme()
    {
        if (!_canvas.Visible) return;

        if (_canvas.Elements.Count < 1)
        {
            HandleResult(CommandResult.Error("Designer enthält keine exportierbare Struktur."));
            return;
        }

        QueryScheme schema = SaveScheme();
        using XtraSaveFileDialog dlg = new();
        dlg.Filter = "QuerySchema|*.crquery|JSON-Datei|*.json|XML-Datei|*.xml|Alle Dateien|*.*";
        dlg.DefaultExt = "crquery";
        dlg.Title = "Query-Schema exportieren";
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            try
            {
                if (dlg.FileName.ToLower().EndsWith("crquery"))
                    File.WriteAllBytes(dlg.FileName, schema.ToJsonBytes());
                else if (dlg.FileName.ToLower().EndsWith("json"))
                    File.WriteAllText(dlg.FileName, schema.ToJsonString());
                else if (dlg.FileName.ToLower().EndsWith("xml"))
                    File.WriteAllText(dlg.FileName, schema.ToXmlString());
                else
                    File.WriteAllBytes(dlg.FileName, schema.ToJsonBytes());

                HandleResult(CommandResult.Success("Datei wurde gespeichert."));
            }
            catch (Exception ex)
            {
                MsgBox.ShowErrorOk($"SCHEMA EXPORTIEREN\r\nBeim Export des Schemas in Datei <b>{dlg.FileName}</b> trat ein Fehler auf.\r\n{ex.Message}");
            }
        }
    }

    /// <summary>
    /// Schema importieren
    /// </summary>
    public void ImportScheme()
    {
        if (!_canvas.Visible) return;

        QueryScheme? schema = null;

        using XtraOpenFileDialog dlg = new();
        dlg.Filter = "QuerySchema|*.crquery|JSON-Datei|*.json|XML-Datei|*.xml|Alle Dateien|*.*";
        dlg.DefaultExt = "crquery";
        dlg.Title = "Query-Schema importieren";
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            try
            {
                if (dlg.FileName.ToLower().EndsWith("crquery"))
                    schema = Functions.DeserializeJsonBytes<QueryScheme>(File.ReadAllBytes(dlg.FileName));
                else if (dlg.FileName.ToLower().EndsWith("json"))
                    schema = Functions.DeserializeJsonString<QueryScheme>(File.ReadAllText(dlg.FileName));
                else if (dlg.FileName.ToLower().EndsWith("xml"))
                    schema = Functions.DeserializeXmlString<QueryScheme>(File.ReadAllText(dlg.FileName));
                else
                    schema = Functions.DeserializeJsonBytes<QueryScheme>(File.ReadAllBytes(dlg.FileName));
            }
            catch (Exception ex)
            {
                MsgBox.ShowErrorOk($"SCHEMA IMPORTIEREN\r\nBeim Import des Schemas in Datei <b>{dlg.FileName}</b> trat ein Fehler auf.\r\n{ex.Message}");
                return;
            }
        }

        if (schema == null)
        {
            HandleResult(CommandResult.Error("Kein gültiges Schema erkannt."));
        }

        if (_canvas.Elements.Count > 0)
        {
            if (MsgBox.ShowWarningOkCancel("SCHEMA ERSETZEN\r\nDas geladene Schema wird das vorhandene Schema komplett ersetzen. Das vorhandene Schema geht damit verloren. Soll das Schema nun ersetzt werden?") == eMessageBoxResult.Cancel)
                return;
        }

        LoadScheme(schema);
    }

    /// <summary>
    /// Designer anzeigen (WYSIWYG)
    /// </summary>
    public void ShowDesigner()
    {
        if (_synedit != null)
            _synedit.Visible = false;

        _canvas.Visible = true;

        dockFieldProp.Visibility = DockVisibility.Visible;
        dockJoinProp.Visibility = DockVisibility.Visible;
        dockTableProp.Visibility = DockVisibility.Visible;
        dockSource.Visibility = DockVisibility.Visible;

        toggleShowCode.Checked = false;
        
        menSchema.Visibility = BarItemVisibility.Always;
    }

    /// <summary>
    /// Editor anzeigen (SQL Quellcode)
    /// </summary>
    public void ShowEditor()
    {
        if (_synedit == null)
        {
            UI.ShowWait(Properties.Resources.LBL_WAIT, Properties.Resources.LBL_LOADING);

            _synedit = new() { Dock = DockStyle.Fill, Visible = false };
            _synedit.SetMode(eSyntaxMode.SQL);
            crPanel1.Controls.Add(_synedit);
            _synedit.BringToFront();

            UI.HideWait();
        }

        _synedit!.Visible = true;
        _canvas.Visible = false;
        
        dockFieldProp.Visibility = DockVisibility.Hidden;
        dockJoinProp.Visibility = DockVisibility.Hidden;
        dockTableProp.Visibility = DockVisibility.Hidden;
        dockSource.Visibility = DockVisibility.Hidden;

        toggleShowCode.Checked = true;

        menSchema.Visibility = BarItemVisibility.Never;
    }

    /// <summary>
    /// Gibt an, ob gerade der Designer-Modus aktiv ist
    /// </summary>
    public bool DesignerMode => _canvas.Visible;
    

    /// <summary>
    /// Registriert eine Datenquelle/Datenbank für den DB-Browser
    /// </summary>
    /// <param name="ds"></param>
    public void RegisterDatasource(IDatabaseConnection ds)
    {
        crdbSchemeBrowser1.RegisterDatasource(ds);
    }

    /// <summary>
    /// Fügt der Zeichenfläche eine Tabelle hinzu.
    /// </summary>
    /// <param name="table">Tabelle</param>
    /// <param name="pt">Punkt, an dem die Tabelle eingefügt wird</param>
    /// <param name="alias">Alias-Name der Tabelle/des Views</param>
    public void AddTable(DatabaseSchemeTable table, Point? pt, string alias)
    {
        if (pt == null)
        {
            int posX = 0;
            int posY = 0;

            foreach (var element in _canvas.Elements.Values)
            {
                posX = Math.Max(posX, element.Location.X + element.Width + 10);
                posY = Math.Max(posY, element.Location.Y);
            }

            pt = new(posX, posY);
        }

        _canvas.AddTable(table, (Point)pt, alias).OnNeedRecreate += (t, _) =>
        {
            RaiseNeedRecreate();
        }; 
    }

    /// <summary>
    /// Code-Generierung erzwingen...
    /// </summary>
    public override void RaiseNeedRecreate()
    {
        needRecreate = true;

        // Timer zum Refresh resetten um unnötige Aktualisierungen zu vermeiden
        recreationTimer?.Stop();
        recreationTimer?.Start();
    }

    private void toogleMode(object sender, ItemClickEventArgs e)
    {
        ToggleMode();
    }

    /// <summary>
    /// eine Tabelle wurde ausgewählt
    /// </summary>
    /// <param name="element">ausgewählte Tabelle</param>
    public override void ElementSelected(AFDesignerCanvasElement? element)
    {
        AFQueryDesignerTable? table = element as AFQueryDesignerTable;
        connectorTable.DataSource = table?.Table;
        CurrentTable = table;

        crDockManager1.ActivePanel = dockTableProp;
    }

    /// <summary>
    /// Momentan aktive Tabelle
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFQueryDesignerTable? CurrentTable { get; set; }

    /// <summary>
    /// Zugriff auf die eigentliche Zeichenfläche.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFQueryDesignerCanvas Canvas => _canvas;

    /// <summary>
    /// ein Join wurde ausgewählt
    /// </summary>
    /// <param name="join"></param>
    public override void JoinSelected(IJoin? join)
    {
        connectorJoin.DataSource = join as DatabaseSchemeJoin;

        crDockManager1.ActivePanel = dockJoinProp;
    }
        
    /// <summary>
    /// Ein Feld wurde ausgewählt...
    /// </summary>
    /// <param name="detail"></param>
    public override void DetailSelected(object? detail)
    {
        DatabaseSchemeField? field = detail as DatabaseSchemeField;
        connectorField.DataSource = field;

        crDockManager1.ActivePanel = dockFieldProp;
    }

    /// <summary>
    /// Schema in den Designer laden.
    /// </summary>
    /// <param name="scheme">zu ladendes Schema</param>
    public void LoadScheme(QueryScheme? scheme)
    {
        CurrentTable = null;
        
        connectorTable.DataSource = null;
        connectorJoin.DataSource = null;
        connectorField.DataSource = null;

        _canvas.Clear();

        if (scheme == null) { return; }

        int xOffset = 0;
        int yOffset = 0;

        foreach (var table in scheme.TABLES)
        {
            if (table.LOCATION == null)
                continue;

            if (table.LOCATION!.Value.X < 10)
                xOffset = Math.Max(xOffset, Math.Abs(table.LOCATION!.Value.X) + 10);
            
            if (table.LOCATION!.Value.Y < 10)
                yOffset = Math.Max(yOffset, Math.Abs(table.LOCATION!.Value.Y) + 10);

        }


        foreach (var table in scheme.TABLES)
        {
            DatabaseSchemeTable dbTable = new()
            {
                TABLE_ALIAS = table.TABLEALIAS,
                TABLE_NAME = table.TABLENAME,
                TABLE_SCHEME = table.TABLESCHEME,
                Id = table.TABLEID
            };

            foreach (var field in table.FIELDS)
            {
                DatabaseSchemeField dbField = new()
                {
                    FIELD_ALIAS = field.FIELDALIAS,
                    FIELD_NAME = field.FIELDNAME,
                    FIELD_TYPE = field.FIELDTYPE,
                    FIELD_CALCULATED = field.FIELDISCALCULATED,
                    IsSelected = field.FIELDISSELECTED,
                    FIELD_EXPRESSION = field.FIELDEXPRESSION,
                    FIELD_POSTREADEXPRESSION = field.FIELDEXPRESSION,
                    FIELD_WHERE = field.FIELDWHERE,
                    FIELD_GROUPBY = field.FIELDGROUPBY,
                    FIELD_LENGTH = field.FIELDLENGTH,
                    FIELD_SYSTEMTYPE = field.FIELDSYSTEMTYPE,
                    Id = field.FIELDID
                };
                dbTable.Fields.Add(dbField);
            }

            AddTable(dbTable, table.LOCATION == null ? null : new Point(table.LOCATION.Value.X + xOffset, table.LOCATION.Value.Y + yOffset), table.TABLEALIAS);
        }


        foreach (var join in scheme.JOINS)
        {
            AFQueryDesignerTable? tableSource = (AFQueryDesignerTable?)Canvas.Elements.Values.FirstOrDefault(e => e.Id.Equals(join.TABLESOURCE));
            AFQueryDesignerTable? tableTarget = (AFQueryDesignerTable?)Canvas.Elements.Values.FirstOrDefault(e => e.Id.Equals(join.TABLETARGET));

            if (tableSource == null || tableTarget == null) continue;

            if (tableSource.Table == null || tableTarget.Table == null) continue;

            DatabaseSchemeField? fieldSource = tableSource.Table?.Fields.FirstOrDefault(f => f.Id.Equals(join.FIELDSOURCE));
            DatabaseSchemeField? fieldTarget = tableTarget.Table?.Fields.FirstOrDefault(f => f.Id.Equals(join.FIELDTARGET));

            if (fieldSource == null || fieldTarget == null) continue;

            DatabaseSchemeJoin dbJoin = new(tableSource.Table!, fieldSource, tableTarget.Table!, fieldTarget);
            
            dbJoin.PropertyChanged += (_, _) =>
            {
                RaiseNeedRecreate();
                Refresh();
            };


            Canvas.Joins.Add(dbJoin);
        }

        _canvas.Refresh();
        GenerateCode(true);
    }

    /// <summary>
    /// Schema im Designer speichern.
    /// </summary>
    /// <returns>gespeichertes Schema</returns>
    public QueryScheme SaveScheme()
    {
        QueryScheme ret = new();

        foreach (var element in Canvas.Elements.Values.Where(t => t is AFQueryDesignerTable).OfType<AFQueryDesignerTable>())
        {
            QueryScheme.QuerySchemeTable table = new()
            {
                SIZE = new(element.Width, element.Height),
                LOCATION = new(element.Left, element.Top),
                TABLENAME = element.Table!.TABLE_NAME,
                TABLEALIAS = element.Table!.TABLE_ALIAS,
                TABLESCHEME = element.Table!.TABLE_SCHEME,
                TABLEID = element.Table!.Id,
                FIELDS = []
            };
            foreach (var field in element.Table.Fields)
            {
                QueryScheme.QuerySchemeField newfield = new()
                {
                    FIELDALIAS = field.FIELD_ALIAS,
                    FIELDEXPRESSION = field.FIELD_EXPRESSION,
                    FIELDISCALCULATED = field.FIELD_CALCULATED,
                    FIELDISSELECTED = field.IsSelected,
                    FIELDNAME = field.FIELD_NAME,
                    FIELDTYPE = field.FIELD_TYPE,
                    FIELDWHERE = field.FIELD_WHERE,
                    FIELDPOSTEXPRESSION = field.FIELD_POSTREADEXPRESSION,
                    FIELDID = field.Id,
                    FIELDGROUPBY = field.FIELD_GROUPBY,
                    FIELDLENGTH = field.FIELD_LENGTH,
                    FIELDSYSTEMTYPE = field.FIELD_SYSTEMTYPE
                };
                table.FIELDS.Add(newfield);
            }

            ret.TABLES.Add(table);
        }

        foreach (var join in Canvas.Joins.Where(j => j is DatabaseSchemeJoin).OfType<DatabaseSchemeJoin>())
        {
            QueryScheme.QuerySchemeJoin newJoin = new()
            {
                JOINTYPE = join.JoinType,
                TABLESOURCE = join.ElementSource,
                TABLETARGET = join.ElementTarget,
                FIELDSOURCE = join.FromField,
                FIELDTARGET = join.ToField
            };

            ret.JOINS.Add(newJoin);
        }

        return ret;
    }


    /// <summary>
    /// Validiert die Daten im Designer und überträgt diese in das IQuery-Objekt.
    /// </summary>
    /// <param name="errors">Liste, die ggf. auftretende Fehler aufnehmen kann</param>
    /// <returns>true, wenn valide - sonst false</returns>
    public override bool IsValid(ValidationErrorCollection errors)
    {
        if (Query == null) return base.IsValid(errors);


        bool ret = true;

        Query.UseScheme = _canvas.Visible;

        if (Query.UseScheme)
        {
            if (!GenerateCode(true, errors))
                ret = false;
            else
                Query.QuerySchemeModel = SaveScheme();

        }
        else if (_synedit != null && _synedit.Visible)
            Query.Query = _synedit!.SourceCode;

        return ret && base.IsValid(errors);
    }

    /// <summary>
    /// Zugriff auf die Liste der Variablen
    /// </summary>
    public BindingList<Variable> Variables => variableBrowser.Variables;

    /// <summary>
    /// Query, die aktuell im Designer bearbeitet wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IQuery? Query
    {
        get => _query; 
        set
        {
            _query = value;
            
            _query ??= new DefaultQuery();

            if (_query.UseScheme)
            {
                ShowDesigner();
                LoadScheme(_query.QuerySchemeModel);
            }
            else
            {
                ShowEditor();
                _synedit!.SourceCode = _query.Query;
            }

            variableBrowser.Variables = _query.Variablen;
        }
    }
}
