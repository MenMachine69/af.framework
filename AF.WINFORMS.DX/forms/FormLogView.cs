using DevExpress.XtraGrid.Views.Grid;

namespace AF.WINFORMS.DX;


/// <summary>
/// Form zum Anzeigen eines AF.CORE.Log
/// </summary>
public sealed class FormLogView : FormBase
{
    private readonly AFGridControl grid = null!;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="log"></param>
    public FormLogView(Log log)
    {
        if (UI.DesignMode) return;

        StartPosition = FormStartPosition.CenterParent;
        Size = new(900, 600);
        Text = "Log";

        AFGridExtender gridextender = new();

        AFGridSetup setup = new()
        {
            PreviewField = nameof(CORE.LogEntry.Description),
            GridCellStyler = (args, row) =>
            {
                if (args is not RowCellStyleEventArgs rowargs) return;

                if (row is not CORE.LogEntry entry) return;

                if (entry.MsgType == eNotificationType.Error)
                    rowargs.Appearance.BackColor = Color.FromArgb(100, Color.Red);
                else if (entry.MsgType == eNotificationType.Error)
                    rowargs.Appearance.BackColor = Color.FromArgb(100, Color.Orange);
                else if (entry.MsgType == eNotificationType.Success)
                    rowargs.Appearance.BackColor = Color.FromArgb(100, Color.LimeGreen);
            }
        };
        setup.Columns.Add(new() { Bold = true, ColumnFieldname = nameof(CORE.LogEntry.MsgType), Caption = "Typ" });
        setup.Columns.Add(new() { ColumnFieldname = nameof(CORE.LogEntry.Timestamp), Caption = "Zeitpunkt" });
        setup.Columns.Add(new() { ColumnFieldname = nameof(CORE.LogEntry.Message), Caption = "Nachricht", AutoFill = true });


        AFTablePanel table = new() { Dock = DockStyle.Fill, UseSkinIndents = false };

        table.BeginLayout();
        grid = table.Add<AFGridControl>(1, 1);
        grid.Setup(setup);

        grid.MainView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

        gridextender.Grid = grid;

        table.SetRow(1, DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 1.0f);
        table.SetColumn(1, DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 1.0f);

        table.EndLayout();


        Controls.Add(table);

        grid.DataSource = log.Entries;
    }

    /// <summary>
    /// Zugriff auf das Grid
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AFGridControl Grid => grid;
}

