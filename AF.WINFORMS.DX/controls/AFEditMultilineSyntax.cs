using System.Text;
using Alternet.Common;
using Alternet.Editor;
using Alternet.Editor.TextSource;
using Alternet.Syntax;
using Alternet.Syntax.Parsers.Advanced;
using Alternet.Syntax.Parsers.Roslyn.CodeCompletion;
using DevExpress.Utils;
using CsParser = Alternet.Syntax.Parsers.Roslyn.CsParser;

namespace AF.WINFORMS.DX;

/// <summary>
/// Editor für Quelltexte mit Syntax-Highlighting etc.
/// </summary>
[SupportedOSPlatform("windows")]
[ToolboxItem(true)]
[ToolboxTabName("AF Common")]
[DefaultBindingProperty("SourceCode")]
public class AFEditMultilineSyntax : AFUserControl
{
    private readonly SyntaxEdit syntaxEdit1 = null!;
    private readonly TextSource textSource1 = null!;
    private XmlParser? xmlParser1;
    private HtmlParser? htmlParser1;
    private CsParser? csParser1;
    private MSSQLParser? mssqlParser1;
    private SqlParser? sqlParser2;
    private JSONParser? jsonParser;
    private JsParser? jsParser;
    private Parser? genericParser;

    //private Alternet.Syntax.Parsers.Roslyn.Par
    private eSyntaxMode currentMode = eSyntaxMode.XML;

    private readonly IContainer components = null!;

    private readonly string[] defaultNamespaces = ["System", "System.Collections", "System.Collections.Generic", "System.Threading", "System.Data", "System.Xml", "System.Text.Json", "System.Linq", "System.IO", "AF.CORE", "AF.DATA", "AF.MVC"];

    /// <summary>
    /// Konstruktor
    /// </summary>
    public AFEditMultilineSyntax()
    {
        if (UI.DesignMode) return;

        components = new Container();


        syntaxEdit1 = new(components) { Dock = DockStyle.Fill, Name = "syntaxEdit1" };
        textSource1 = new(components);

        syntaxEdit1.BorderStyle = EditBorderStyle.None;
        syntaxEdit1.Braces.BracesOptions = BracesOptions.Highlight | BracesOptions.HighlightBounds;
        syntaxEdit1.Gutter.Options = GutterOptions.PaintLineNumbers | GutterOptions.PaintBookMarks | GutterOptions.PaintLineModificators;
        syntaxEdit1.LineSeparator.HighlightBackColor = Color.Empty;
        syntaxEdit1.Minimap.CurrentFrameBorderColor = Color.FromArgb(62, 62, 62);
        syntaxEdit1.Selection.SelectedWordsBackColor = Color.FromArgb(224, 225, 222);
        syntaxEdit1.SyntaxPaint.ReadonlyBackColor = Color.FromArgb(240, 240, 240);
        syntaxEdit1.VisualThemeType = VisualThemeType.Light;

        textSource1.BracesOptions = BracesOptions.Highlight | BracesOptions.HighlightBounds;
        syntaxEdit1.Source = textSource1;

        Controls.Add(syntaxEdit1);

        SetMode(eSyntaxMode.XML);
        syntaxEdit1.SetSkin();
    }

    /// <summary>
    /// Zugriff auf den internen Editor
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SyntaxEdit Editor => syntaxEdit1;

    /// <summary>
    /// Zugriff auf die interne TextSource
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TextSource TextSource => textSource1;


    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing) components.Dispose();

        base.Dispose(disposing);
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (UI.DesignMode) return;

        UI.StyleChanged += styleChanged;

        styleChanged(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        if (UI.DesignMode) return;

        UI.StyleChanged -= styleChanged;
    }

    private void styleChanged(object? sender, EventArgs e)
    {
        syntaxEdit1.SetSkin();
    }


    /// <summary>
    /// Modus setzen (welche Syntax wird genutzt)
    /// </summary>
    /// <param name="mode">Modus des Editors</param>
    /// <param name="loadAssemblies">Liste der Namen der Assemblies, die geladen werden sollen (nur CSHarp)
    /// DIESE OPTION WIRD NUR BEIM ERSTMALIGEN SETZEN DES MODUS CSharp BEACHTET! </param>
    /// <param name="namespaces">Liste der zu registrierenden Namespaces (nur CSharp).
    /// Folgende Namespaces werden immer registriert:
    ///    System
    ///    System.Collections
    ///    System.Collections.Generic
    ///    System.Threading
    ///    System.Data
    ///    System.Linq
    ///    System.IO
    ///    System.Xml
    ///    System.Text.Json
    ///    AF.CORE
    ///    AF.MVC
    /// DIESE OPTION WIRD NUR BEIM ERSTMALIGEN SETZEN DES MODUS CSharp BEACHTET! 
    /// </param>
    public void SetMode(eSyntaxMode mode, string[]? loadAssemblies = null, string[]? namespaces = null)
    {
        currentMode = mode;

        syntaxEdit1.SetMode(mode);

        if (mode == eSyntaxMode.CSharp)
        {
            if (csParser1 == null)
            {
                csParser1 = new(new CsSolution())
                {
                    XmlScheme = Properties.Resources.csParser1_XmlScheme,
                    Options =
                        SyntaxOptions.Outline |
                        SyntaxOptions.SmartIndent |
                        SyntaxOptions.CodeCompletion |
                        SyntaxOptions.SyntaxErrors |
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
                        SyntaxOptions.OutlineBlocks
                };


                csParser1.Repository.RegisterDefaultAssemblies(TechnologyEnvironment.System, true);
                csParser1.Repository.RegisterDefaultNamespaces(TechnologyEnvironment.System);
                csParser1.Repository.RegisterAssembly("AF.CORE");

                if (loadAssemblies != null)
                    csParser1.Repository.RegisterAssemblies(loadAssemblies);
    
                csParser1.Repository.RegisterNamespaces(defaultNamespaces.Concat(namespaces ?? []).ToArray());
            }
            syntaxEdit1.Lexer = csParser1;
            textSource1.Lexer = csParser1;
        }
        else if (mode == eSyntaxMode.HTML || mode == eSyntaxMode.CSS)
        {
            htmlParser1 ??= new()
            {
                Options = SyntaxOptions.SmartIndent |
                          SyntaxOptions.SyntaxErrors |
                          SyntaxOptions.ReparseOnLineChange |
                          SyntaxOptions.FormatCase |
                          SyntaxOptions.FormatSpaces |
                          SyntaxOptions.FormatOnLineChange,
                XmlScheme = Properties.Resources.htmlParser1_XmlScheme
            };

            syntaxEdit1.Lexer = htmlParser1;
            textSource1.Lexer = htmlParser1;
        }
        else if (mode == eSyntaxMode.XML)
        {
            xmlParser1 ??= new()
            {
                Options = SyntaxOptions.SmartIndent |
                          SyntaxOptions.SyntaxErrors |
                          SyntaxOptions.ReparseOnLineChange |
                          SyntaxOptions.FormatCase |
                          SyntaxOptions.FormatSpaces |
                          SyntaxOptions.FormatOnLineChange
                // XmlScheme = Properties.Resources.xmlParser1_XmlScheme
            };

            syntaxEdit1.Lexer = xmlParser1;
            textSource1.Lexer = xmlParser1;
        }
        else if (mode == eSyntaxMode.Json)
        {
            jsonParser ??= new()
            {
                Options = SyntaxOptions.SmartIndent |
                          SyntaxOptions.SyntaxErrors |
                          SyntaxOptions.ReparseOnLineChange |
                          SyntaxOptions.FormatCase |
                          SyntaxOptions.FormatSpaces |
                          SyntaxOptions.FormatOnLineChange
            };

            syntaxEdit1.Lexer = jsonParser;
            textSource1.Lexer = jsonParser;
        }
        else if (mode == eSyntaxMode.JavaScript)
        {
            jsParser ??= new()
            {
                Options = SyntaxOptions.SmartIndent |
                          SyntaxOptions.SyntaxErrors |
                          SyntaxOptions.ReparseOnLineChange |
                          SyntaxOptions.FormatCase |
                          SyntaxOptions.FormatSpaces |
                          SyntaxOptions.FormatOnLineChange
            };

            syntaxEdit1.Lexer = jsParser;
            textSource1.Lexer = jsParser;
        }
        else if (mode == eSyntaxMode.MSSQL)
        {
            mssqlParser1 ??= new()
            { 
                Options = SyntaxOptions.SmartIndent |
                          SyntaxOptions.SyntaxErrors |
                          SyntaxOptions.ReparseOnLineChange |
                          SyntaxOptions.FormatCase |
                          SyntaxOptions.FormatSpaces |
                          SyntaxOptions.FormatOnLineChange,
                XmlScheme = Properties.Resources.mssqlParser1_XmlScheme
            };

            syntaxEdit1.Lexer = mssqlParser1;
            textSource1.Lexer = mssqlParser1;
        }
        else if (mode == eSyntaxMode.SQL)
        {
            if (sqlParser2 == null)
            {
                sqlParser2 = new();
                sqlParser2.Options = SyntaxOptions.SmartIndent |
                                     // SyntaxOptions.SyntaxErrors |
                                     SyntaxOptions.ReparseOnLineChange | 
                                     SyntaxOptions.FormatCase |
                                     SyntaxOptions.FormatSpaces |
                                     SyntaxOptions.FormatOnLineChange;

                sqlParser2.XmlScheme = Properties.Resources.sqlParser2_XmlScheme;
            }

            syntaxEdit1.Lexer = sqlParser2;
            textSource1.Lexer = sqlParser2;
        }
        else if (mode == eSyntaxMode.PowerShell)
        {
            using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(WinFormsStrings.SYNTAX_POWERSHELL));
            genericParser ??= new();
            genericParser.Scheme.LoadStream(memStream, Encoding.UTF8);

            syntaxEdit1.Lexer = genericParser;
            textSource1.Lexer = genericParser;
        }
    }

    /// <summary>
    /// Registrierung von Assemblies (nur CSharp)
    /// </summary>
    /// <param name="assemblies">Liste der zu registrierenden Assemblies (Name der Datei ohne Pfad und Endung). Die Assembly muss sich im Programmverzeichnis oder im Unterverzeichnis Assembly/Assemblies befinden.</param>
    /// <exception cref="Exception">Ausnahme, wenn Fehler bei der Registrierung auftreten.</exception>
    public void RegisterAssemblies(string[] assemblies)
    {
        if (csParser1 == null) throw new Exception("Registrierung nur im CSharp Modus verfügbar.");

        foreach (var name in assemblies)
        {
            try
            {
                csParser1.Repository.RegisterAssembly(name);
            }
            catch (Exception e)
            {
                throw new Exception($"Assembly {name} konnte nicht registriert werden. Überprüfen Sie die Verfügbarkeit.", e);
            }
        }
    }

    /// <summary>
    /// Registrierung von Namespaces (nur CSharp)
    /// </summary>
    /// <param name="namespaces">Liste der zu registrierenden Namespaces</param>
    /// <exception cref="Exception">Ausnahme, wenn Fehler bei der Registrierung auftreten.</exception>
    public void RegisterNamespace(string[] namespaces)
    {
        if (csParser1 == null) throw new Exception("Registrierung nur im CSharp Modus verfügbar.");

        try
        {
            csParser1.Repository.RegisterNamespaces(namespaces);
        }
        catch (Exception e)
        {
            throw new Exception($"Namespaces konnten nicht registriert werden. Überprüfen Sie die Verfügbarkeit.", e);
        }
    }

    /// <summary>
    /// CSharpParser des Editors
    /// </summary>
    public CsParser? CSharpParser => csParser1;

    /// <summary>
    /// Generischer Parser (z.B. für eigene Sprachen)
    /// </summary>
    public Parser? GenericParser => genericParser;

    /// <summary>
    /// der bearbeitbare Quelltext
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string SourceCode
    {
        get => textSource1.Text;
        set
        {
            textSource1.Text = value;
            
            if (currentMode != eSyntaxMode.CSharp) return;

            // Assemblies nachladen...
            var assemblies = value.GetAssembliesFromCode();
            RegisterAssemblies(assemblies.ToArray());
        }
    }
}


