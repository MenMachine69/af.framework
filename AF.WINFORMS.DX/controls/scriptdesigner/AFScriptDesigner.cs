using Alternet.Syntax;
using AF.BUSINESS;
using AF.MVC;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;

namespace AF.WINFORMS.DX;

/// <summary>
/// Designer für Skripte
/// </summary>
public partial class AFScriptDesigner : AFEditorBase, IVariableConsumer
{
    private readonly AFVariableBrowser variableBrowser = null!;
    private readonly AFEditMultilineSyntax source = null!;
    private IScript? _script;
    private IScriptSnippet? _snippet;
    private bool _snippetMode;
    private readonly BindingList<SyntaxError> errors = [];

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFScriptDesigner(bool snippetMode = false)
    {
        InitializeComponent();
        
        if (UI.DesignMode) return;

        _snippetMode = snippetMode;

        components ??= new Container();

        crDockManager1.Panels.ForEach(p =>
        {
            p.Options.AllowFloating = false;
            p.Options.ShowCloseButton = false;
        });

        if (snippetMode)
        {
            panelContainer2.ActiveChild = dockSnippets;
            panelContainer1.Visibility = DockVisibility.Hidden;
            dockOutput.Visibility = DockVisibility.Hidden;
            dockErrors.Visibility = DockVisibility.Hidden;
            dockVariablen.Visibility = DockVisibility.Hidden;

            menMainCheckCode.Visibility = BarItemVisibility.Never;
            standaloneBarDockControl1.Visible = false;
        }

        source = new() { Dock = DockStyle.Fill };
        source.SetMode(eSyntaxMode.CSharp);
        source.AllowDrop = true;
        if (snippetMode)
        {
            source.TextSource.CheckSpelling = false;
            source.TextSource.UnhighlightSyntaxErrors();
            source.CSharpParser!.Options =
                SyntaxOptions.Outline |
                SyntaxOptions.SmartIndent |
                SyntaxOptions.CodeCompletion |
                SyntaxOptions.QuickInfoTips |
                SyntaxOptions.AutoComplete |
                SyntaxOptions.FormatCase |
                SyntaxOptions.FormatSpaces |
                SyntaxOptions.EvaluateConditionals |
                SyntaxOptions.NotifyOnParse |
                SyntaxOptions.StructureGuideLines |
                SyntaxOptions.CodeFixes |
                SyntaxOptions.CodeRefactors |
                SyntaxOptions.CodeCompletionTabs |
                SyntaxOptions.OutlineBlocks;
        }
        else
        {
            source.CSharpParser!.TextReparsed += (_, _) =>
            {
                if (source.TextSource.SyntaxErrors.Count > 0)
                    showSyntaxErrors();
                else
                    clearSyntaxErrors();
            };
        }

        panelMain.Controls.Add(source);
        source.BringToFront();
        
        variableBrowser = new() { Dock = DockStyle.Fill };
        dockVariablen.Controls.Add(variableBrowser);

        mleOutput.Font = UI.ConsoleFont;

        AFGridSetup setup = new()
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
            phliste.AddRange(AFCore.Placeholder.Values.ToArray() ?? []);

        gridPlaceholder.ForceInitialize();

        gridPlaceholder.DataSource = phliste;

        var cmdAddSnippet = AFCore.App.ScriptingService?.CmdAdd;

        setup = new()
        {
            AllowAddNew = false,
            AllowEdit = false,
            AllowMultiSelect = false,
            CmdGoto = AFCore.App.ScriptingService?.CmdGoto,
            Columns =
            [
                new AFGridColumn() { Caption = "Name", Bold = true, ColumnFieldname = nameof(IScriptSnippet.Name) },
                new AFGridColumn() { Caption = "Beschreibung", Bold = false, ColumnFieldname = nameof(IScriptSnippet.Description) }
            ]
        };


        viewSnippets.Setup(setup);

        BindingList<IScriptSnippet> snippetliste = AFCore.App.ScriptingService?.GetSnippets() ?? [];

        gridSnippets.ForceInitialize();

        gridSnippets.DataSource = snippetliste;

        setup = new()
        {
            AllowAddNew = false,
            AllowEdit = false,
            AllowMultiSelect = false,
            Columns =
            [
                new AFGridColumn() { Caption = "Code", Width = 100, FixedWidth = true, Bold = true, ColumnFieldname = nameof(SyntaxError.ErrorCode) },
                new AFGridColumn() { Caption = "Typ", Width = 100, FixedWidth = true, Bold = true, ColumnFieldname = nameof(SyntaxError.ErrorType) },
                new AFGridColumn() { Caption = "Beschreibung", Bold = false, ColumnFieldname = nameof(SyntaxError.ErrorMessage), AutoFill = true },
                new AFGridColumn() { Caption = "Zeile", Width = 60, FixedWidth = true, Bold = false, ColumnFieldname = nameof(SyntaxError.Line) },
            ]
        };


        viewErrors.Setup(setup);

        gridErrors.ForceInitialize();

        gridErrors.DataSource = errors;

        viewErrors.DoubleClick += (_, _) =>
        {
            if (viewErrors.GetFocusedRow() is SyntaxError error && error.Line > 0)
            {
                source.Editor.Focus();
                //source.Editor.MoveToLine(error.Line - 1);
                source.Editor.Position = error.Error?.Position ?? Point.Empty;
            }

        };

        extenderSnippet.Grid = gridSnippets;
        extenderSnippet.SupportDragDrop = true;
        extenderSnippet.RequestDragData += (_, e) =>
        {
            if (e.DraggedRow is IScriptSnippet snippet)
                e.DragData = snippet.Code;
        };

        extenderPlaceholder.Grid = gridPlaceholder;
        extenderPlaceholder.SupportDragDrop = true;
        extenderPlaceholder.RequestDragData += (_, e) =>
        {
            if (e.DraggedRow is IPlaceholder pholder)
                e.DragData = pholder.Name;
        };

        // TODO: Besser machen! Keine Hardcoded Namespaces.
        crClassBrowser1.RegisterDefaultNamespaces();
        crClassBrowser1.NameSpaces.Add("crm.core");
        crClassBrowser1.NameSpaces.Add("crm.data");
        crClassBrowser1.NameSpaces.Add("crm.gui");

        menMainCheckCode.ItemClick += (_, _) =>
        {
            // TODO: Überrprüfen des Codes hinzufügen
            //ScriptManager.Instance.TryToCompile()
        };

        lblDropSnippet.DragEnter += (_, e) =>
        {
            if (e.Data == null)
            {    
                e.Effect = DragDropEffects.None;
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.Text) ||
                e.Data.GetDataPresent(DataFormats.OemText) ||
                e.Data.GetDataPresent(DataFormats.UnicodeText))
                e.Effect = DragDropEffects.All;

            else
                e.Effect = DragDropEffects.None;

        };
        
        lblDropSnippet.DragDrop += (_, e) =>
        {
            e.Effect = DragDropEffects.All;

            if (e.Data == null) return;
            
            string? code = null;

            if (e.Data.GetDataPresent(DataFormats.Text))
                code = e.Data.GetData(DataFormats.Text, true) as string;
            else if (e.Data.GetDataPresent(DataFormats.OemText))
                code = e.Data.GetData(DataFormats.OemText, true) as string;
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
                code = e.Data.GetData(DataFormats.UnicodeText, true) as string;

            if (code != null)
            {
                if (cmdAddSnippet != null)
                {
                    var result = cmdAddSnippet.Execute(new CommandArgs() { Tag = code });
                    if (result.Result == eNotificationType.Success && result.ResultObject is IScriptSnippet newsnippet)
                        (gridSnippets.DataSource as BindingList<IScriptSnippet>)?.Add(newsnippet);

                    HandleResult(result);
                }
                else
                    MsgBox.ShowErrorOk("CREATE SNIPPET\r\nAnlegen des Snippets nicht möglich. App hat keinen ScriptingService oder dieser stellt kein Command zur Verfügung.");
            }
        };


    }

    private void clearSyntaxErrors()
    {
        errors.Clear();
    }

    private void showSyntaxErrors()
    {
        errors.Clear();
        errors.RaiseListChangedEvents = false;

        foreach (var error in source.TextSource.SyntaxErrors)
        {
            if (error.ErrorType == SyntaxErrorType.Hidden) continue;

            errors.Add(new()
            {
                ErrorType = error.ErrorType,
                ErrorCode = error.ErrorCode,
                ErrorMessage = error.Description,
                Line = error.Position.Y + 1,
                Column = error.Position.X,
                Error = error
            });
            
        }

        errors.RaiseListChangedEvents = true;
        gridErrors.RefreshDataSource();
    }

    /// <summary>
    /// Zugriff auf die Liste der Variablen
    /// </summary>
    public BindingList<Variable> Variables => variableBrowser.Variables;

    /// <summary>
    /// Query, die aktuell im Designer bearbeitet wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IScript? Script
    {
        get => _script;
        set
        {
            _script = value;

            _script ??= new DefaultScript();

            source.SourceCode = _script.SourceCode;

            variableBrowser.Variables = _script.Variablen;
        }
    }


    /// <summary>
    /// Query, die aktuell im Designer bearbeitet wird.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IScriptSnippet? Snippet
    {
        get => _snippet;
        set
        {
            _snippet = value;

            _snippet ??= new DefaultCodeSnippet();

            source.SourceCode = _snippet.Code;
        }
    }


    /// <summary>
    /// Validiert die Daten im Designer und überträgt diese in das IScript-Objekt.
    /// </summary>
    /// <param name="errors">Liste, die ggf. auftretende Fehler aufnehmen kann</param>
    /// <returns>true, wenn valide - sonst false</returns>
    public override bool IsValid(ValidationErrorCollection errors)
    {
        bool ret = true;
        
        if (_snippetMode)
        {
            if (Snippet == null) return base.IsValid(errors);

            Snippet.Code = source.SourceCode;

            return ret && base.IsValid(errors);
        }

        if (Script == null) return base.IsValid(errors);
        
        Script.SourceCode = source.SourceCode;

        return ret && base.IsValid(errors);
    }

    /// <summary>
    /// Zugriff auf den ORM Browser (z.B. zur Registrierung)
    /// </summary>
    public AFORMBrowser OrmBrowser => ormBrowser;

    /// <summary>
    /// Registriert eine Datenquelle/Datenbank für den DB-Browser
    /// </summary>
    /// <param name="ds"></param>
    public void RegisterDatasource(IDatabaseConnection ds)
    {
        crdbSchemeBrowser1.RegisterDatasource(ds);
        
        if (ds is IDatabase db)
            ormBrowser.RegisterDatabase(db, db.DatabaseName);
    }

    /// <summary>
    /// Registriert Assemblies/DLLs für Script.
    ///
    /// Die Namen werden OHNE Dateiendung angegeben. Gesucht wird die
    /// Assembly im Programmverzeichnis und den Unterverzeichnissen 'Assemblies' und 'Assemblys'
    /// </summary>
    /// <param name="assemblies">Namen der Assemblies</param>
    public void RegisterAssemblies(string[] assemblies)
    {
        source.RegisterAssemblies(assemblies);
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

    internal class SyntaxError
    {
        /// <summary>
        /// Fehlermeldung
        /// </summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>
        /// Fehlercode
        /// </summary>
        public string ErrorCode { get; set; } = "";

        /// <summary>
        /// Typ des Fehlers
        /// </summary>
        public SyntaxErrorType ErrorType { get; set; } = SyntaxErrorType.Error;

        /// <summary>
        /// Zeile
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Zeile
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// der komplette Fehler
        /// </summary>
        public ISyntaxError? Error { get; set; }
    }

    /// <summary>
    /// Zugriff auf den SyntaxEditor (AFMultilineSyntax)
    /// </summary>
    public AFEditMultilineSyntax Editor => source;

    /// <summary>
    /// Unterstützte Syntax im Editor setzen (Standard: eSyntaxMode.CSharp)
    /// </summary>
    /// <param name="mode"></param>
    public void SetSyntaxMode(eSyntaxMode mode)
    {
        source.SetMode(mode);
    }
}
