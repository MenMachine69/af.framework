using AF.MVC;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Timer = System.Windows.Forms.Timer;

namespace AF.WINFORMS.DX;

/// <summary>
/// Komponente zum anzeigen von ToastNotifications
/// </summary>
public class AFToastNotificator
{
    private readonly List<formToast> _toasts = [];

    /// <summary>
    /// Constructor
    /// </summary>
    public AFToastNotificator() {  }

    /// <summary>
    /// Einen neuen Toast anzeigen...
    /// </summary>
    /// <param name="type">Typ der Benachrichtigung</param>
    /// <param name="caption">Überschrift</param>
    /// <param name="description">Mitteilung (kann HTML enthalten)</param>
    /// <param name="linkaction">auszuführende Aktion</param>
    /// <param name="link">ModelLink auf den hingewiesen wird</param>
    /// <param name="timeoutSeconds">Timeout, standard = 15</param>
    public void ShowToast(eNotificationType type, string caption, string description, Action<ModelLink>? linkaction = null, ModelLink? link = null, int timeoutSeconds = 15)
    {
        formToast toast = new(this, caption, description, type, linkaction, link, timeoutSeconds);
       
        // Position ermitteln
        int x = SystemInformation.PrimaryMonitorSize.Width - toast.Width - 1;
        int y = SystemInformation.WorkingArea.Top;

        foreach (formToast t in _toasts)
        {
            if (t.Location.Y < (y + toast.Height))
                y = t.Location.Y + t.Height + 1;
        }

        toast.Location = new(x, y);
        toast.Show();
        Application.DoEvents();
    }

    internal void registerToast(formToast formToast)
    {
        if (!_toasts.Contains(formToast))
            _toasts.Add(formToast);
    }

    internal void unregisterToast(formToast formToast)
    {
        if (_toasts.Contains(formToast))
            _toasts.Remove(formToast);
    }

    /// <summary>
    /// Einen Toast der via Timer geschlossen wurde auf die Liste der nicht beachteten/ungelesenen Toasts setzen 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="caption"></param>
    /// <param name="content"></param>
    /// <param name="link"></param>
    internal void addMissedToast(eNotificationType type, string caption, string content, ModelLink? link)
    {

    }


    /// <summary>
    /// ToastNotification-Form
    /// </summary>
    internal class formToast : FormBase
    {
        private Timer? _timer;
        private readonly AFButton pshClose = null!;
        private readonly AFButton pshAction = null!;
        private readonly AFToastNotificator notificator = null!;

        /// <summary>
        /// Constructor
        /// </summary>
        public formToast(AFToastNotificator owner, string caption, string content, eNotificationType type, Action<ModelLink>? action = null, ModelLink? link = null, int timeoutSeconds = 15)
        {
            if (UI.DesignMode) return;

            notificator = owner;

            FormBorderStyle = FormBorderStyle.None;
            //LookAndFeel.SkinName = "Office 2019 Black";
            //LookAndFeel.UseDefaultLookAndFeel = false;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "formToast";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;

            Size = new(UI.GetScaled(330), UI.GetScaled(130));

            AFLabel lblColorCode = new()
            {
                Text = "",
                AutoSizeMode = LabelAutoSizeMode.None,
                Dock = DockStyle.Left,
                BackColor = UI.TranslateToSkinColor(Color.Blue),
                Size = new(20, 50)
            };
            Controls.Add(lblColorCode);

            AFTablePanel table = new() { Dock = DockStyle.Fill, UseSkinIndents = true };
            table.BeginLayout();
            table.Add<AFLabelBoldText>(1, 1).Text = caption;

            pshClose = table.Add<AFButton>(1, 2);
            pshClose.PaintStyle = PaintStyles.Light;
            pshClose.Size = new(26, 26);
            pshClose.ImageOptions.AllowGlyphSkinning = DefaultBoolean.True;
            pshClose.ImageOptions.ImageToTextAlignment = ImageAlignToText.RightCenter;
            pshClose.ImageOptions.SvgImageSize = new(16, 16);
            pshClose.ImageOptions.SvgImage = UI.GetImage(Symbol.Dismiss);
            pshClose.TabStop = false;
            pshClose.AllowFocus = false;
            pshClose.Click += (_, _) =>
            {
                _timer?.Stop();
                _timer?.Dispose();
                _timer = null;
              
                Close();
            };

            AFLabel lblContent = table.Add<AFLabel>(2, 1, colspan: 2);
            lblContent.Dock = DockStyle.Fill;
            lblContent.Padding = new(5, 5, 0, 0);
            lblContent.AutoSizeMode = LabelAutoSizeMode.None;
            lblContent.Appearance.TextOptions.VAlignment = VertAlignment.Top;
            lblContent.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
            lblContent.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
            lblContent.Appearance.Options.UseTextOptions = true;
            lblContent.AllowHtmlString = true;
            lblContent.Text = content;

            pshAction = table.Add<AFButton>(3, 1, colspan: 2);
            pshAction.Text = "Öffnen";
            pshAction.AutoSize = true;
            pshAction.Dock = DockStyle.Right;
            pshAction.Visible = action != null;
            pshAction.Padding = new(10, 5, 10, 5);
            pshClose.TabStop = false;
            pshClose.AllowFocus = false;
            pshAction.Click += (_, _) =>
            {
                _timer?.Stop();
                _timer?.Dispose();
                _timer = null;

                if (link != null) action?.Invoke(link);

                Close();
            };

            table.SetColumn(1, TablePanelEntityStyle.Relative, 1.0f);
            table.SetRow(2, TablePanelEntityStyle.Relative, 1.0f);
            
            table.EndLayout();
            Controls.Add(table);
            table.BringToFront();

            switch (type)
            {
                case eNotificationType.Error:
                    lblColorCode.BackColor = UI.TranslateToSkinColor(Color.Red);
                    break;
                case eNotificationType.Information:
                    lblColorCode.BackColor = UI.TranslateToSkinColor(Color.Blue);
                    break;
                case eNotificationType.Success:
                    lblColorCode.BackColor = UI.TranslateToSkinColor(Color.Green);
                    break;
                case eNotificationType.Warning:
                    lblColorCode.BackColor = UI.TranslateToSkinColor(Color.Yellow);
                    break;
            }

            
            if (action == null && timeoutSeconds == 0)
                timeoutSeconds = 10;

            if (timeoutSeconds > 0)
            {
                _timer = new()
                {
                    Enabled = false,
                    Interval = timeoutSeconds * 1000
                };

                _timer.Tick += (_, _) =>
                {
                    _timer?.Stop();
                    _timer?.Dispose();

                    // den Notificator benachrichtigen, dass dieser Toast via Timer geschlossen wurde...
                    notificator.addMissedToast(type, caption, content, link);

                    Close();
                };
            }
        }

        /// <summary>
        /// <see cref="System.Windows.Forms.Form.OnShown(EventArgs)"/>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            notificator.registerToast(this);
            _timer?.Start();
        }

        /// <summary>
        /// <see cref="System.Windows.Forms.Form.OnFormClosing(FormClosingEventArgs)"/>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            notificator.unregisterToast(this);
        }
    }

    /// <summary>
    /// Beschreibung einer verpassten ToastNotification
    /// </summary>
    public class ToastNotification : Base
    {
        /// <summary>
        /// Überschrift
        /// </summary>
        public string Caption { get; set; } = "";
    }
}