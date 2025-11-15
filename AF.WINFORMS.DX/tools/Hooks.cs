using System.Diagnostics;

namespace AF.WINFORMS.DX;

/// <summary>
/// Provides base implementation of methods for subscription and unsubscription to application and/or global mouse and keyboard hooks.
/// </summary>
public abstract class Hooker
{
	internal abstract int Subscribe(int hookId, HookCallback hookCallback);

	internal void Unsubscribe(int handle)
	{
		int result = UnhookWindowsHookEx(handle);

		if (result == 0) ThrowLastUnmanagedErrorAsException();
	}

	internal abstract bool IsGlobal { get; }

	/// <summary>
	/// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain. 
	/// A hook procedure can call this function either before or after processing the hook information. 
	/// </summary>
	/// <param name="idHook">Ignored.</param>
	/// <param name="nCode">[in] Specifies the hook code passed to the current hook procedure.</param>
	/// <param name="wParam">[in] Specifies the wParam value passed to the current hook procedure.</param>
	/// <param name="lParam">[in] Specifies the lParam value passed to the current hook procedure.</param>
	/// <returns>This value is returned by the next hook procedure in the chain.</returns>
	/// <remarks>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
	/// </remarks>
	[DllImport("user32.dll", CharSet = CharSet.Auto,
		CallingConvention = CallingConvention.StdCall)]
	internal static extern int CallNextHookEx(
		int idHook,
		int nCode,
		int wParam,
		IntPtr lParam);

	/// <summary>
	/// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain. 
	/// You would install a hook procedure to monitor the system for certain types of events. These events 
	/// are associated either with a specific thread or with all threads in the same desktop as the calling thread. 
	/// </summary>
	/// <param name="idHook">
	/// [in] Specifies the type of hook procedure to be installed. This parameter can be one of the following values.
	/// </param>
	/// <param name="lpfn">
	/// [in] Pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a 
	/// thread created by a different process, the lpfn parameter must point to a hook procedure in a dynamic-link 
	/// library (DLL). Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
	/// </param>
	/// <param name="hMod">
	/// [in] Handle to the DLL containing the hook procedure pointed to by the lpfn parameter. 
	/// The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by 
	/// the current process and if the hook procedure is within the code associated with the current process. 
	/// </param>
	/// <param name="dwThreadId">
	/// [in] Specifies the identifier of the thread with which the hook procedure is to be associated. 
	/// If this parameter is zero, the hook procedure is associated with all existing threads running in the 
	/// same desktop as the calling thread. 
	/// </param>
	/// <returns>
	/// If the function succeeds, the return value is the handle to the hook procedure.
	/// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
	/// </returns>
	/// <remarks>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
	/// </remarks>
	[DllImport("user32.dll", CharSet = CharSet.Auto,
		CallingConvention = CallingConvention.StdCall, SetLastError = true)]
	internal static extern int SetWindowsHookEx(
		int idHook,
		HookCallback lpfn,
		IntPtr hMod,
		int dwThreadId);

	/// <summary>
	/// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
	/// </summary>
	/// <param name="idHook">
	/// [in] Handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx. 
	/// </param>
	/// <returns>
	/// If the function succeeds, the return value is nonzero.
	/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
	/// </returns>
	/// <remarks>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
	/// </remarks>
	[DllImport("user32.dll", CharSet = CharSet.Auto,
		CallingConvention = CallingConvention.StdCall, SetLastError = true)]
	internal static extern int UnhookWindowsHookEx(int idHook);

	internal static void ThrowLastUnmanagedErrorAsException()
	{
		//Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
		int errorCode = Marshal.GetLastWin32Error();
		//Initializes and throws a new instance of the Win32Exception class with the specified error. 
		throw new Win32Exception(errorCode);
	}
}

/// <summary>
/// Provides methods for subscription and unsubscription to application mouse and keyboard hooks.
/// </summary>
public class AppHooker : Hooker
{
	/// <summary>
	/// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure. 
	/// </summary>
	internal const int WH_MOUSE = 7;

	/// <summary>
	/// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure. 
	/// </summary>
	internal const int WH_KEYBOARD = 2;

	internal override int Subscribe(int hookId, HookCallback hookCallback)
	{
		int hookHandle = SetWindowsHookEx(
			hookId,
			hookCallback,
			IntPtr.Zero,
			GetCurrentThreadId());

		if (hookHandle == 0) ThrowLastUnmanagedErrorAsException();

		return hookHandle;
	}

	internal override bool IsGlobal => false;

	/// <summary>
	/// Retrieves the unmanaged thread identifier of the calling thread.
	/// </summary>
	/// <returns></returns>
	[DllImport("kernel32")]
	private static extern int GetCurrentThreadId();
}

/// <summary>
/// Provides methods for subscription and unsubscription to global mouse and keyboard hooks.
/// </summary>
public class GlobalHooker : Hooker
{
	internal override int Subscribe(int hookId, HookCallback hookCallback)
	{
		var processModule = Process.GetCurrentProcess().MainModule ?? throw new NullReferenceException(@"Cant found MainModule of current process.");
		
		int hookHandle = SetWindowsHookEx(
			hookId,
			hookCallback,
			processModule.BaseAddress,
			0);

		if (hookHandle == 0)
			ThrowLastUnmanagedErrorAsException();

		return hookHandle;
	}

	internal override bool IsGlobal => true;

	/// <summary>
	/// Installs a hook procedure that monitors low-level mouse input events.
	/// </summary>
	internal const int WH_MOUSE_LL = 14;

	/// <summary>
	/// Installs a hook procedure that monitors low-level keyboard  input events.
	/// </summary>
	internal const int WH_KEYBOARD_LL = 13;
}

/// <summary>
/// The CallWndProc hook procedure is an application-defined or library-defined callback 
/// function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer 
/// to this callback function. CallWndProc is a placeholder for the application-defined 
/// or library-defined function name.
/// </summary>
/// <param name="nCode">
/// [in] Specifies whether the hook procedure must process the message. 
/// If nCode is HC_ACTION, the hook procedure must process the message. 
/// If nCode is less than zero, the hook procedure must pass the message to the 
/// CallNextHookEx function without further processing and must return the 
/// value returned by CallNextHookEx.
/// </param>
/// <param name="wParam">
/// [in] Specifies whether the message was sent by the current thread. 
/// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
/// </param>
/// <param name="lParam">
/// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
/// </param>
/// <returns>
/// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
/// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
/// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
/// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
/// procedure does not call CallNextHookEx, the return value should be zero. 
/// </returns>
/// <remarks>
/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
/// </remarks>
public delegate int HookCallback(int nCode, int wParam, IntPtr lParam);

/// <summary>
/// Base class used to implement mouse or keybord hook listeners.
/// It provides base methods to subscribe and unsubscribe to hooks.
/// Common processing, error handling and cleanup logic.
/// </summary>
public abstract class BaseHookListener : IDisposable
{
	private Hooker m_Hooker;

	/// <summary>
	/// Base constructor of <see cref="BaseHookListener"/>
	/// </summary>
	/// <param name="hooker">Depending on this parameter the listener hooks either application or global keyboard events.</param>
	/// <remarks>
	/// Hooks are not active after instantiation. You need to use either <see cref="BaseHookListener.Enabled"/> property or call <see cref="BaseHookListener.Start"/> method.
	/// </remarks>
	protected BaseHookListener(Hooker hooker)
	{
		m_Hooker = hooker;
		Enabled = false;
	}

	/// <summary>
	/// Stores the handle to the Keyboard or Mouse hook procedure.
	/// </summary>
	protected int HookHandle { get; set; }

	/// <summary>
	/// Keeps the reference to prevent garbage collection of delegate. See: CallbackOnCollectedDelegate http://msdn.microsoft.com/en-us/library/43yky316(v=VS.100).aspx
	/// </summary>
	protected HookCallback? HookCallbackReferenceKeeper { get; set; }

	internal bool IsGlobal => m_Hooker.IsGlobal;

	/// <summary>
	/// Override this method to modify logic of firing events.
	/// </summary>
	protected abstract bool ProcessCallback(int wParam, IntPtr lParam);

	/// <summary>
	/// A callback function which will be called every time a keyboard or mouse activity detected.
	/// <see cref="HookCallback"/>
	/// </summary>
	protected int HookCallback(int nCode, int wParam, IntPtr lParam)
	{
		if (nCode == 0)
		{
			bool shouldProcess = ProcessCallback(wParam, lParam);

			if (!shouldProcess) return -1;
		}

		return CallNextHook(nCode, wParam, lParam);
	}

	private int CallNextHook(int nCode, int wParam, IntPtr lParam)
	{
		return Hooker.CallNextHookEx(HookHandle, nCode, wParam, lParam);
	}

	/// <summary>
	/// Subscribes to the hook and starts firing events.
	/// </summary>
	/// <exception cref="System.ComponentModel.Win32Exception"></exception>
	public void Start()
	{
		if (Enabled) throw new InvalidOperationException(@"Hook listener is already started. Call Stop() method first or use enabled property.");

		HookCallbackReferenceKeeper = HookCallback;

		try
		{
			HookHandle = m_Hooker.Subscribe(GetHookId(), HookCallbackReferenceKeeper);
		}
		catch(Exception)
		{
			HookCallbackReferenceKeeper = null;
			HookHandle = 0;
			throw;
		}
	}

	/// <summary>
	/// Unsubscribes from the hook and stops firing events.
	/// </summary>
	/// <exception cref="System.ComponentModel.Win32Exception"></exception>
	public void Stop()
	{
		try
		{
			m_Hooker.Unsubscribe(HookHandle);
		}
		finally
		{
			HookCallbackReferenceKeeper = null;
			HookHandle = 0;
		}
	}

	/// <summary>
	/// Enables you to switch from application hooks to global hooks and vice versa on the fly
	/// without unsubscribing from events. Component remains enabled or disabled state after this call as it was before.
	/// </summary>
	/// <param name="hooker">An AppHooker or GlobalHooker object.</param>
	public void Replace(Hooker hooker)
	{
		bool rememberEnabled = Enabled;
		Enabled = false;
		m_Hooker = hooker;
		Enabled = rememberEnabled;
	}

	/// <summary>
	/// Override to deliver correct id to be used for <see cref="Hooker.SetWindowsHookEx"/> call.
	/// </summary>
	/// <returns></returns>
	protected abstract int GetHookId();

	/// <summary>
	/// Gets or Sets the enabled status of the Hook.
	/// </summary>
	/// <value>
	/// True - The Hook is presently installed, activated, and will fire events.
	/// <para>
	/// False - The Hook is not part of the hook chain, and will not fire events.
	/// </para>
	/// </value>
	public bool Enabled
	{
		get => HookHandle != 0;
		set
		{
			if (value && !Enabled) Start();
			else if (!value && Enabled) Stop();
		}
	}

	/// <summary>
	/// Release delegates, unsubscribes from hooks.
	/// </summary>
	/// <filterpriority>2</filterpriority>
	public virtual void Dispose()
	{
		Stop();
	}

	/// <summary>
	/// Unsubscribes from global hooks skiping error handling.
	/// </summary>
	~BaseHookListener()
	{
		if (HookHandle != 0) Hooker.UnhookWindowsHookEx(HookHandle);
	}
}

/// <summary>
/// This class monitors all keyboard activities and provides appropriate events.
/// </summary>
public class KeyboardHookListener : BaseHookListener
{
	/// <summary>
	/// Initializes a new instance of <see cref="KeyboardHookListener"/>.
	/// </summary>
	/// <param name="hooker">Depending on this parameter the listener hooks either application or global keyboard events.</param>
	/// <remarks>Hooks are not active after instantiation. You need to use either <see cref="BaseHookListener.Enabled"/> property or call <see cref="BaseHookListener.Start"/> method.</remarks>
	public KeyboardHookListener(Hooker hooker)
		: base(hooker)
	{
	}

	/// <summary>
	/// This method processes the data from the hook and initiates event firing.
	/// </summary>
	/// <param name="wParam">The first Windows Messages parameter.</param>
	/// <param name="lParam">The second Windows Messages parameter.</param>
	/// <returns>
	/// True - The hook will be passed along to other applications.
	/// <para>
	/// False - The hook will not be given to other applications, effectively blocking input.
	/// </para>
	/// </returns>
	protected override bool ProcessCallback(int wParam, IntPtr lParam)
	{
		KeyEventArgsExt? e = KeyEventArgsExt.FromRawData(wParam, lParam, IsGlobal);

		if (e == null) return false;

		InvokeKeyDown(e);
		InvokeKeyPress(wParam, lParam);
		InvokeKeyUp(e);

		return !e.Handled;
	}

	/// <summary>
	/// Returns the correct hook id to be used for <see cref="Hooker.SetWindowsHookEx"/> call.
	/// </summary>
	/// <returns>WH_KEYBOARD (0x02) or WH_KEYBOARD_LL (0x13) constant.</returns>
	protected override int GetHookId()
	{
		return IsGlobal ? 
			GlobalHooker.WH_KEYBOARD_LL : 
			AppHooker.WH_KEYBOARD;
	}

	/// <summary>
	/// Occurs when a key is preseed. 
	/// </summary>
	public event KeyEventHandler? KeyDown;

	private void InvokeKeyDown(KeyEventArgsExt e)
	{
		if (e.Handled || !e.IsKeyDown) return;

		KeyDown?.Invoke(this, e);
	}

	/// <summary>
	/// Occurs when a key is pressed.
	/// </summary>
	/// <remarks>
	/// Key events occur in the following order: 
	/// <list type="number">
	/// <item>KeyDown</item>
	/// <item>KeyPress</item>
	/// <item>KeyUp</item>
	/// </list>
	///The KeyPress event is not raised by noncharacter keys; however, the noncharacter keys do raise the KeyDown and KeyUp events. 
	///Use the KeyChar property to sample keystrokes at run time and to consume or modify a subset of common keystrokes. 
	///To handle keyboard events only in your application and not enable other applications to receive keyboard events, 
	///set the <see cref="KeyPressEventArgs.Handled"/> property in your form's KeyPress event-handling method to <b>true</b>. 
	/// </remarks>
	public event KeyPressEventHandler? KeyPress;

	private void InvokeKeyPress(int wParam, IntPtr lParam)
	{
		InvokeKeyPress(KeyPressEventArgsExt.FromRawData(wParam, lParam, IsGlobal));
	}

	private void InvokeKeyPress(KeyPressEventArgsExt? e)
	{
		if (e == null) return;

		if (e.Handled || e.IsNonChar) return;
		
		KeyPress?.Invoke(this, e);
	}

	/// <summary>
	/// Occurs when a key is released. 
	/// </summary>
	public event KeyEventHandler? KeyUp;

	private void InvokeKeyUp(KeyEventArgsExt e)
	{
		if (e.Handled || !e.IsKeyUp) return;

		KeyUp?.Invoke(this, e);
	}

	/// <summary>
	/// Release delegates, unsubscribes from hooks.
	/// </summary>
	/// <filterpriority>2</filterpriority>
	public override void Dispose()
	{
		KeyPress = null;
		KeyDown = null;
		KeyUp = null;

		base.Dispose();
	}
}

/// <summary>
/// Provides extended argument data for the <see cref='KeyboardHookListener.KeyDown'/> or <see cref='KeyboardHookListener.KeyUp'/> event.
/// </summary>
public class KeyEventArgsExt : KeyEventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="KeyEventArgsExt"/> class.
	/// </summary>
	/// <param name="keyData"></param>
	public KeyEventArgsExt(Keys keyData) : base(keyData)
	{
	}

	internal KeyEventArgsExt(Keys keyData, int timestamp, bool isKeyDown, bool isKeyUp)
		: this(keyData)
	{
		Timestamp = timestamp; 
		IsKeyDown = isKeyDown;
		IsKeyUp = isKeyUp;
	}

	/// <summary>
	/// Creates <see cref="KeyEventArgsExt"/> from Windows Message parameters.
	/// </summary>
	/// <param name="wParam">The first Windows Message parameter.</param>
	/// <param name="lParam">The second Windows Message parameter.</param>
	/// <param name="isGlobal">Specifies if the hook is local or global.</param>
	/// <returns>A new KeyEventArgsExt object.</returns>
	internal static KeyEventArgsExt? FromRawData(int wParam, IntPtr lParam, bool isGlobal)
	{
		return isGlobal ? 
			FromRawDataGlobal(wParam, lParam) : 
			FromRawDataApp(wParam, lParam);
	}

	/// <summary>
	/// Creates <see cref="KeyEventArgsExt"/> from Windows Message parameters, based upon
	/// a local application hook.
	/// </summary>
	/// <param name="wParam">The first Windows Message parameter.</param>
	/// <param name="lParam">The second Windows Message parameter.</param>
	/// <returns>A new KeyEventArgsExt object.</returns>
	private static KeyEventArgsExt FromRawDataApp(int wParam, IntPtr lParam)
	{
		//http://msdn.microsoft.com/en-us/library/ms644984(v=VS.85).aspx

		const ulong maskKeydown = 0x40000000; // for bit 30
		const ulong maskKeyup = 0x80000000; // for bit 31

		int timestamp = Environment.TickCount;

#if IS_X64
		// both of these are ugly hacks. Is there a better way to convert a 64bit IntPtr to uint?

		// flags = uint.Parse(lParam.ToString());
		ulong flags = Convert.ToUInt32(lParam.ToInt64());
#else
		ulong flags = (ulong)lParam;
#endif
			

		//bit 30 Specifies the previous key state. The value is 1 if the key is down before the message is sent; it is 0 if the key is up.
		bool wasKeyDown = (flags & maskKeydown) > 0;
		//bit 31 Specifies the transition state. The value is 0 if the key is being pressed and 1 if it is being released.
		bool isKeyReleased = (flags & maskKeyup) > 0;

		Keys keyData = (Keys)wParam;

		bool isKeyDown = !wasKeyDown && !isKeyReleased;
		bool isKeyUp = wasKeyDown && isKeyReleased;

		return new(keyData, timestamp, isKeyDown, isKeyUp);
	}

	/// <summary>
	/// Creates <see cref="KeyEventArgsExt"/> from Windows Message parameters, based upon
	/// a system-wide hook.
	/// </summary>
	/// <param name="wParam">The first Windows Message parameter.</param>
	/// <param name="lParam">The second Windows Message parameter.</param>
	/// <returns>A new KeyEventArgsExt object.</returns>
	private static KeyEventArgsExt? FromRawDataGlobal(int wParam, IntPtr lParam)
	{
		if (Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct)) is not KeyboardHookStruct keyboardHookStruct) return null;

		Keys keyData = (Keys)keyboardHookStruct.VirtualKeyCode;
		bool isKeyDown = wParam == Messages.WM_KEYDOWN || wParam == Messages.WM_SYSKEYDOWN;
		bool isKeyUp = wParam == Messages.WM_KEYUP || wParam == Messages.WM_SYSKEYUP;
			
		return new(keyData, keyboardHookStruct.Time, isKeyDown, isKeyUp);
	}

	/// <summary>
	/// The system tick count of when the event occured.
	/// </summary> 
	public int Timestamp { get; private set; }
		
	/// <summary>
	/// True if event singnals key down..
	/// </summary>
	public bool IsKeyDown { get; private set; }
		
	/// <summary>
	/// True if event singnals key up.
	/// </summary>
	public bool IsKeyUp { get; private set; }
}

///<summary>
/// Provides extended data for the <see cref='KeyboardHookListener.KeyPress'/> event.
///</summary>
public class KeyPressEventArgsExt : KeyPressEventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref='KeyPressEventArgsExt'/> class.
	/// </summary>
	/// <param name="keyChar">Character corresponding to the key pressed. 0 char if represens a system or functional non char key.</param>
	public KeyPressEventArgsExt(char keyChar)
		: base(keyChar)
	{
		IsNonChar = false;
		Timestamp = Environment.TickCount;
	}

	private static KeyPressEventArgsExt CreateNonChar()
	{
		KeyPressEventArgsExt e = new((char)0x0)
		{
			IsNonChar = true,
			Timestamp = Environment.TickCount
		};
		return e;
	}

	/// <summary>
	/// Creates <see cref="KeyPressEventArgsExt"/> from Windows Message parameters.
	/// </summary>
	/// <param name="wParam">The first Windows Message parameter.</param>
	/// <param name="lParam">The second Windows Message parameter.</param>
	/// <param name="isGlobal">Specifies if the hook is local or global.</param>
	/// <returns>A new KeyPressEventArgsExt object.</returns>
	internal static KeyPressEventArgsExt? FromRawData(int wParam, IntPtr lParam, bool isGlobal)
	{
		return isGlobal ?
			FromRawDataGlobal(wParam, lParam) :
			FromRawDataApp(wParam, lParam);
	}

	/// <summary>
	/// Creates <see cref="KeyPressEventArgsExt"/> from Windows Message parameters,
	/// based upon a local application hook.
	/// </summary>
	/// <param name="wParam">The first Windows Message parameter.</param>
	/// <param name="lParam">The second Windows Message parameter.</param>
	/// <returns>A new KeyPressEventArgsExt object.</returns>
	private static KeyPressEventArgsExt FromRawDataApp(int wParam, IntPtr lParam)
	{
		//http://msdn.microsoft.com/en-us/library/ms644984(v=VS.85).aspx

		const ulong maskKeydown = 0x40000000;         // for bit 30
		const ulong maskKeyup = 0x80000000;           // for bit 31
		const ulong maskScanCode = 0xff0000;          // for bit 23-16

#if IS_X64
		// both of these are ugly hacks. Is there a better way to convert a 64bit IntPtr to uint?

		// flags = uint.Parse(lParam.ToString());
		ulong flags = Convert.ToUInt32(lParam.ToInt64());
#else
		ulong flags = (ulong)lParam;
#endif

		//bit 30 Specifies the previous key state. The value is 1 if the key is down before the message is sent; it is 0 if the key is up.
		bool wasKeyDown = (flags & maskKeydown) > 0;
		//bit 31 Specifies the transition state. The value is 0 if the key is being pressed and 1 if it is being released.
		bool isKeyReleased = (flags & maskKeyup) > 0;

		if (!wasKeyDown && !isKeyReleased) return CreateNonChar();

		int virtualKeyCode = wParam;
		int scanCode = checked((int)(flags & maskScanCode));
		const int fuState = 0;

		bool isSuccessfull = Keyboard.TryGetCharFromKeyboardState(virtualKeyCode, scanCode, fuState, out char ch);
		
		return !isSuccessfull ? CreateNonChar() : new(ch);
	}

	/// <summary>
	/// Creates <see cref="KeyPressEventArgsExt"/> from Windows Message parameters,
	/// based upon a system-wide hook.
	/// </summary>
	/// <param name="wParam">The first Windows Message parameter.</param>
	/// <param name="lParam">The second Windows Message parameter.</param>
	/// <returns>A new KeyPressEventArgsExt object.</returns>
	internal static KeyPressEventArgsExt? FromRawDataGlobal(int wParam, IntPtr lParam)
	{
		if (wParam != Messages.WM_KEYDOWN) return CreateNonChar();

		if (Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct)) is not KeyboardHookStruct keyboardHookStruct) return null;

		int virtualKeyCode = keyboardHookStruct.VirtualKeyCode;
		int scanCode = keyboardHookStruct.ScanCode;
		int fuState = keyboardHookStruct.Flags;

		bool isSuccessfull = Keyboard.TryGetCharFromKeyboardState(virtualKeyCode, scanCode, fuState, out char ch);
		if (!isSuccessfull) return CreateNonChar();

		KeyPressEventArgsExt e = new(ch)
		{
			Timestamp = keyboardHookStruct.Time     // Update the timestamp to use the actual one from KeyboardHookStruct
		};

		return e;
	}

	/// <summary>
	/// True if represents a system or functional non char key.
	/// </summary>
	public bool IsNonChar { get; private set; }
		
	/// <summary>
	/// The system tick count of when the event occured.
	/// </summary> 
	public int Timestamp { get; private set; }
}

/// <summary>
/// Provides extended data for the MouseClickExt and MouseMoveExt events. 
/// </summary>
public class MouseEventExtArgs : MouseEventArgs
{
	/// <summary>
	/// Creates <see cref="MouseEventExtArgs"/> from Windows Message parameters.
	/// </summary>
	/// <param name="wParam">The first Windows Message parameter.</param>
	/// <param name="lParam">The second Windows Message parameter.</param>
	/// <param name="isGlobal">Specifies if the hook is local or global.</param>
	/// <returns>A new MouseEventExtArgs object.</returns>
	internal static MouseEventExtArgs? FromRawData(int wParam, IntPtr lParam, bool isGlobal)
	{
		return isGlobal ?
			FromRawDataGlobal(wParam, lParam) :
			FromRawDataApp(wParam, lParam);
	}

	/// <summary>
	/// Creates <see cref="MouseEventExtArgs"/> from Windows Message parameters, 
	/// based upon a local application hook.
	/// </summary>
	/// <param name="wParam">The first Windows Message parameter.</param>
	/// <param name="lParam">The second Windows Message parameter.</param>
	/// <returns>A new MouseEventExtArgs object.</returns>
	private static MouseEventExtArgs? FromRawDataApp(int wParam, IntPtr lParam)
	{
		return Marshal.PtrToStructure(lParam, typeof(AppMouseStruct)) is not AppMouseStruct marshalledMouseStruct 
			? null 
			: FromRawDataUniversal(wParam, marshalledMouseStruct.ToMouseStruct());
	}

	/// <summary>
	/// Creates <see cref="MouseEventExtArgs"/> from Windows Message parameters, 
	/// based upon a system-wide global hook.
	/// </summary>
	/// <param name="wParam">The first Windows Message parameter.</param>
	/// <param name="lParam">The second Windows Message parameter.</param>
	/// <returns>A new MouseEventExtArgs object.</returns>
	internal static MouseEventExtArgs? FromRawDataGlobal(int wParam, IntPtr lParam)
	{
		return (Marshal.PtrToStructure(lParam, typeof(MouseStruct)) is not MouseStruct marshalledMouseStruct)
			? null
			: FromRawDataUniversal(wParam, marshalledMouseStruct);
	}

	/// <summary>
	/// Creates <see cref="MouseEventExtArgs"/> from relevant mouse data. 
	/// </summary>
	/// <param name="wParam">First Windows Message parameter.</param>
	/// <param name="mouseInfo">A MouseStruct containing information from which to contruct MouseEventExtArgs.</param>
	/// <returns>A new MouseEventExtArgs object.</returns>
	private static MouseEventExtArgs FromRawDataUniversal(int wParam, MouseStruct mouseInfo)
	{
		MouseButtons button = MouseButtons.None;
		short mouseDelta = 0;
		int clickCount = 0;

		bool isMouseKeyDown = false;
		bool isMouseKeyUp = false;


		switch (wParam)
		{
			case Messages.WM_LBUTTONDOWN:
				isMouseKeyDown = true;
				button = MouseButtons.Left;
				clickCount = 1;
				break;
			case Messages.WM_LBUTTONUP:
				isMouseKeyUp = true;
				button = MouseButtons.Left;
				clickCount = 1;
				break;
			case Messages.WM_LBUTTONDBLCLK:
				isMouseKeyDown = true;
				button = MouseButtons.Left;
				clickCount = 2;
				break;
			case Messages.WM_RBUTTONDOWN:
				isMouseKeyDown = true;
				button = MouseButtons.Right;
				clickCount = 1;
				break;
			case Messages.WM_RBUTTONUP:
				isMouseKeyUp = true;
				button = MouseButtons.Right;
				clickCount = 1;
				break;
			case Messages.WM_RBUTTONDBLCLK:
				isMouseKeyDown = true;
				button = MouseButtons.Right;
				clickCount = 2;
				break;
			case Messages.WM_MBUTTONDOWN:
				isMouseKeyDown = true;
				button = MouseButtons.Middle;
				clickCount = 1;
				break;
			case Messages.WM_MBUTTONUP:
				isMouseKeyUp = true;
				button = MouseButtons.Middle;
				clickCount = 1;
				break;
			case Messages.WM_MBUTTONDBLCLK:
				isMouseKeyDown = true;
				button = MouseButtons.Middle;
				clickCount = 2;
				break;
			case Messages.WM_MOUSEWHEEL:
				mouseDelta = mouseInfo.MouseData;
				break;
			case Messages.WM_XBUTTONDOWN:
				button = mouseInfo.MouseData == 1 ? MouseButtons.XButton1 :
																		MouseButtons.XButton2;
				isMouseKeyDown = true;
				clickCount = 1;
				break;

			case Messages.WM_XBUTTONUP:
				button = mouseInfo.MouseData == 1 ? MouseButtons.XButton1 :
																		MouseButtons.XButton2;
				isMouseKeyUp = true;
				clickCount = 1;
				break;

			case Messages.WM_XBUTTONDBLCLK:
				isMouseKeyDown = true;
				button = mouseInfo.MouseData == 1 ? MouseButtons.XButton1 :
																		MouseButtons.XButton2;
				clickCount = 2;
				break;
		}

		var e = new MouseEventExtArgs(
			button,
			clickCount,
			mouseInfo.Point,
			mouseDelta,
			mouseInfo.Timestamp,
			isMouseKeyDown,
			isMouseKeyUp);

		return e;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MouseEventExtArgs"/> class. 
	/// </summary>
	/// <param name="buttons">One of the MouseButtons values indicating which mouse button was pressed.</param>
	/// <param name="clicks">The number of times a mouse button was pressed.</param>
	/// <param name="point">The x and y -coordinate of a mouse click, in pixels.</param>
	/// <param name="delta">A signed count of the number of detents the wheel has rotated.</param>
	/// <param name="timestamp">The system tick count when the event occured.</param>
	/// <param name="isMouseKeyDown">True if event singnals mouse button down.</param>
	/// <param name="isMouseKeyUp">True if event singnals mouse button up.</param>
	internal MouseEventExtArgs(MouseButtons buttons, int clicks, HookPoint point, int delta, int timestamp,  bool isMouseKeyDown, bool isMouseKeyUp)
		: base(buttons, clicks, point.X, point.Y, delta)
	{
		IsMouseKeyDown = isMouseKeyDown;
		IsMouseKeyUp = isMouseKeyUp;
		Timestamp = timestamp;
	}

	internal MouseEventExtArgs ToDoubleClickEventArgs()
	{
		return new(Button, 2, Point, Delta, Timestamp, IsMouseKeyDown, IsMouseKeyUp); 
	}

	/// <summary>
	/// Set this property to <b>true</b> inside your event handler to prevent further processing of the event in other applications.
	/// </summary>
	public bool Handled { get; set; }

	/// <summary>
	/// True if event contains information about wheel scroll.
	/// </summary>
	public bool WheelScrolled => Delta != 0;

	/// <summary>
	/// True if event signals a click. False if it was only a move or wheel scroll.
	/// </summary>
	public bool Clicked => Clicks > 0;

	/// <summary>
	/// True if event singnals mouse button down.
	/// </summary>
	public bool IsMouseKeyDown
	{
		get;
		private set;
	}

	/// <summary>
	/// True if event singnals mouse button up.
	/// </summary>
	public bool IsMouseKeyUp
	{
		get;
		private set;
	}

	/// <summary>
	/// The system tick count of when the event occured.
	/// </summary>
	public int Timestamp
	{
		get;
		private set;
	}

	/// <summary>
	/// 
	/// </summary>
	internal HookPoint Point => new(X, Y);
}

/// <summary>
/// This class monitors all mouse activities and provides appropriate events.
/// </summary>
public class MouseHookListener : BaseHookListener
{

	private HookPoint m_PreviousPosition;
	private int m_PreviousClickedTime;
	private MouseButtons m_PreviousClicked;
	private MouseButtons m_DownButtonsWaitingForMouseUp;
	private MouseButtons m_SuppressButtonUpFlags;
	private readonly int m_SystemDoubleClickTime;

	/// <summary>
	/// Initializes a new instance of <see cref="MouseHookListener"/>.
	/// </summary>
	/// <param name="hooker">Depending on this parameter the listener hooks either application or global mouse events.</param>
	/// <remarks>
	/// Hooks are not active after installation. You need to use either <see cref="BaseHookListener.Enabled"/> property or call <see cref="BaseHookListener.Start"/> method.
	/// </remarks>
	public MouseHookListener(Hooker hooker)
		: base(hooker)
	{
		m_PreviousPosition = new(-1, -1);
		m_PreviousClickedTime = 0;
		m_DownButtonsWaitingForMouseUp = MouseButtons.None;
		m_SuppressButtonUpFlags = MouseButtons.None;
		m_PreviousClicked = MouseButtons.None;
		m_SystemDoubleClickTime = Mouse.GetDoubleClickTime();
	}

	//##################################################################
	#region ProcessCallback and related subroutines

	/// <summary>
	/// This method processes the data from the hook and initiates event firing.
	/// </summary>
	/// <param name="wParam">The first Windows Messages parameter.</param>
	/// <param name="lParam">The second Windows Messages parameter.</param>
	/// <returns>
	/// True - The hook will be passed along to other applications.
	/// <para>
	/// False - The hook will not be given to other applications, effectively blocking input.
	/// </para>
	/// </returns>
	protected override bool ProcessCallback(int wParam, IntPtr lParam)
	{
		MouseEventExtArgs? e = MouseEventExtArgs.FromRawData(wParam, lParam, IsGlobal);

		if (e == null) return true;

		if (e.IsMouseKeyDown)
			ProcessMouseDown(ref e);

		if (e.Clicks == 1 && e.IsMouseKeyUp && !e.Handled)
			ProcessMouseClick(ref e);

		if (e.Clicks == 2 && !e.Handled)
			MouseDoubleClick?.Invoke(this, e);

		if (e.IsMouseKeyUp) ProcessMouseUp(ref e);

		if (e.WheelScrolled)
			MouseWheel?.Invoke(this, e);

		if (HasMoved(e.Point)) ProcessMouseMove(ref e);

		return !e.Handled;
	}

	private void ProcessMouseDown(ref MouseEventExtArgs e)
	{
		if (IsGlobal)
			ProcessPossibleDoubleClick(ref e);
		else
		{
			// These are only used for global. No need for them in AppHooks
			m_DownButtonsWaitingForMouseUp = MouseButtons.None;
			m_PreviousClicked = MouseButtons.None;
			m_PreviousClickedTime = 0;
		} 

		MouseDown?.Invoke(this, e);
		MouseDownExt?.Invoke(this, e);

		if (e.Handled)
		{
			SetSupressButtonUpFlag(e.Button);
			e.Handled = true;
		}
	}

	private void ProcessPossibleDoubleClick(ref MouseEventExtArgs e)
	{
		if (IsDoubleClick(e.Button, e.Timestamp))
		{
			e = e.ToDoubleClickEventArgs();
			m_DownButtonsWaitingForMouseUp = MouseButtons.None;
			m_PreviousClicked = MouseButtons.None;
			m_PreviousClickedTime = 0;
		}
		else
		{
			m_DownButtonsWaitingForMouseUp |= e.Button;
			m_PreviousClickedTime = e.Timestamp;
		}
	}

	private void ProcessMouseClick(ref MouseEventExtArgs e)
	{
		if ((m_DownButtonsWaitingForMouseUp & e.Button) != MouseButtons.None)
		{
			m_PreviousClicked = e.Button;
			m_DownButtonsWaitingForMouseUp = MouseButtons.None;

			MouseClick?.Invoke(this, e);
			MouseClickExt?.Invoke(this, e);
		}
	}

	private void ProcessMouseUp(ref MouseEventExtArgs e)
	{
		if (!HasSupressButtonUpFlag(e.Button)) 
			MouseUp?.Invoke(this, e);
		else
		{
			RemoveSupressButtonUpFlag(e.Button);
			e.Handled = true;
		}
	}

	private void ProcessMouseMove(ref MouseEventExtArgs e)
	{
		m_PreviousPosition = e.Point;

		MouseMove?.Invoke(this, e);
		MouseMoveExt?.Invoke(this, e);
	}

	#endregion

	private void RemoveSupressButtonUpFlag(MouseButtons button)
	{
		m_SuppressButtonUpFlags ^= button;
	}

	private bool HasSupressButtonUpFlag(MouseButtons button)
	{
		return (m_SuppressButtonUpFlags & button) != 0;
	}

	private void SetSupressButtonUpFlag(MouseButtons button)
	{
		m_SuppressButtonUpFlags |= button;
	}

	/// <summary>
	/// Returns the correct hook id to be used for <see cref="Hooker.SetWindowsHookEx"/> call.
	/// </summary>
	/// <returns>WH_MOUSE (0x07) or WH_MOUSE_LL (0x14) constant.</returns>
	protected override int GetHookId()
	{
		return IsGlobal ?
			GlobalHooker.WH_MOUSE_LL :
			AppHooker.WH_MOUSE;
	}

	private bool HasMoved(HookPoint actualPoint)
	{
		return m_PreviousPosition != actualPoint;
	}

	private bool IsDoubleClick(MouseButtons button, int timestamp)
	{
		return
			button == m_PreviousClicked &&
			timestamp - m_PreviousClickedTime <= m_SystemDoubleClickTime; // Mouse.GetDoubleClickTime();
	}

	/// <summary>
	/// Occurs when the mouse pointer is moved.
	/// </summary>
	public event MouseEventHandler? MouseMove;

	/// <summary>
	/// Occurs when the mouse pointer is moved.
	/// </summary>
	/// <remarks>
	/// This event provides extended arguments of type <see cref = "MouseEventArgs" /> enabling you to 
	/// supress further processing of mouse movement in other applications.
	/// </remarks>
	public event EventHandler<MouseEventExtArgs>? MouseMoveExt;

	/// <summary>
	/// Occurs when a click was performed by the mouse.
	/// </summary>
	public event MouseEventHandler? MouseClick;

	/// <summary>
	/// Occurs when a click was performed by the mouse.
	/// </summary>
	/// <remarks>
	/// This event provides extended arguments of type <see cref = "MouseEventArgs" /> enabling you to 
	/// supress further processing of mouse click in other applications.
	/// </remarks>
	[Obsolete(@"To suppress mouse clicks use MouseDownExt event instead.")]
	public event EventHandler<MouseEventExtArgs>? MouseClickExt;

	/// <summary>
	/// Occurs when the mouse a mouse button is pressed.
	/// </summary>
	public event MouseEventHandler? MouseDown;

	/// <summary>
	/// Occurs when the mouse a mouse button is pressed.
	/// </summary>
	/// <remarks>
	/// This event provides extended arguments of type <see cref = "MouseEventArgs" /> enabling you to 
	/// supress further processing of mouse click in other applications.
	/// </remarks>
	public event EventHandler<MouseEventExtArgs>? MouseDownExt;

	/// <summary>
	/// Occurs when a mouse button is released.
	/// </summary>
	public event MouseEventHandler? MouseUp;

	/// <summary>
	/// Occurs when the mouse wheel moves.
	/// </summary>
	public event MouseEventHandler? MouseWheel;

	/// <summary>
	/// Occurs when a mouse button is double-clicked.
	/// </summary>
	public event MouseEventHandler? MouseDoubleClick;

	/// <summary>
	/// Release delegates, unsubscribes from hooks.
	/// </summary>
	/// <filterpriority>2</filterpriority>
	public override void Dispose()
	{
		MouseClick = null;
		MouseClickExt = null;
		MouseDown = null;
		MouseDownExt = null;
		MouseMove = null;
		MouseMoveExt = null;
		MouseUp = null;
		MouseWheel = null;
		MouseDoubleClick = null;
		base.Dispose();
	}
}

#region internal classes

internal static class Keyboard
{
	//values from Winuser.h in Microsoft SDK.
	public const byte VK_SHIFT = 0x10;
	public const byte VK_CAPITAL = 0x14;
	public const byte VK_NUMLOCK = 0x90;

	internal static bool TryGetCharFromKeyboardState(int virtualKeyCode, int scanCode, int fuState, out char ch)
	{
		bool isDownShift = (GetKeyState(VK_SHIFT) & 0x80) == 0x80;
		bool isDownCapslock = GetKeyState(VK_CAPITAL) != 0;

		byte[] keyState = new byte[256];
		GetKeyboardState(keyState);
		byte[] inBuffer = new byte[2];

		bool isSuccesfullyConverted = ToAscii(virtualKeyCode,
												scanCode,
												keyState,
												inBuffer,
												fuState) == 1;
		if (!isSuccesfullyConverted)
		{
			ch = (char)0;
			return false;
		}

		ch = (char)inBuffer[0];
		if ((isDownCapslock ^ isDownShift) && char.IsLetter(ch)) ch = char.ToUpper(ch);
		return true;
	}


	/// <summary>
	/// The ToAscii function translates the specified virtual-key code and keyboard 
	/// state to the corresponding character or characters. The function translates the code 
	/// using the input language and physical keyboard layout identified by the keyboard layout handle.
	/// </summary>
	/// <param name="uVirtKey">
	/// [in] Specifies the virtual-key code to be translated. 
	/// </param>
	/// <param name="uScanCode">
	/// [in] Specifies the hardware scan code of the key to be translated. 
	/// The high-order bit of this value is set if the key is up (not pressed). 
	/// </param>
	/// <param name="lpbKeyState">
	/// [in] Pointer to a 256-byte array that contains the current keyboard state. 
	/// Each element (byte) in the array contains the state of one key. 
	/// If the high-order bit of a byte is set, the key is down (pressed). 
	/// The low bit, if set, indicates that the key is toggled on. In this function, 
	/// only the toggle bit of the CAPS LOCK key is relevant. The toggle state 
	/// of the NUM LOCK and SCROLL LOCK keys is ignored.
	/// </param>
	/// <param name="lpwTransKey">
	/// [out] Pointer to the buffer that receives the translated character or characters. 
	/// </param>
	/// <param name="fuState">
	/// [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise. 
	/// </param>
	/// <returns>
	/// If the specified key is a dead key, the return value is negative. Otherwise, it is one of the following values. 
	/// Value Meaning 
	/// 0 The specified virtual key has no translation for the current state of the keyboard. 
	/// 1 One character was copied to the buffer. 
	/// 2 Two characters were copied to the buffer. This usually happens when a dead-key character 
	/// (accent or diacritic) stored in the keyboard layout cannot be composed with the specified 
	/// virtual key to form a single character. 
	/// </returns>
	/// <remarks>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
	/// </remarks>
	[DllImport("user32")]
	public static extern int ToAscii(
		int uVirtKey,
		int uScanCode,
		byte[] lpbKeyState,
		byte[] lpwTransKey,
		int fuState);

	/// <summary>
	/// The GetKeyboardState function copies the status of the 256 virtual keys to the 
	/// specified buffer. 
	/// </summary>
	/// <param name="pbKeyState">
	/// [in] Pointer to a 256-byte array that contains keyboard key states. 
	/// </param>
	/// <returns>
	/// If the function succeeds, the return value is nonzero.
	/// If the function fails, the return value is zero. To get extended error information, call GetLastError. 
	/// </returns>
	/// <remarks>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
	/// </remarks>
	[DllImport("user32")]
	public static extern int GetKeyboardState(byte[] pbKeyState);

	/// <summary>
	/// The GetKeyState function retrieves the status of the specified virtual key. The status specifies whether the key is up, down, or toggled 
	/// (on, off—alternating each time the key is pressed). 
	/// </summary>
	/// <param name="vKey">
	/// [in] Specifies a virtual key. If the desired virtual key is a letter or digit (A through Z, a through z, or 0 through 9), nVirtKey must be set to the ASCII value of that character. For other keys, it must be a virtual-key code. 
	/// </param>
	/// <returns>
	/// The return value specifies the status of the specified virtual key, as follows: 
	///If the high-order bit is 1, the key is down; otherwise, it is up.
	///If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key, is toggled if it is turned on. The key is off and untoggled if the low-order bit is 0. A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled, and off when the key is untoggled.
	/// </returns>
	/// <remarks>http://msdn.microsoft.com/en-us/library/ms646301.aspx</remarks>
	[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
	public static extern short GetKeyState(int vKey);
}

/// <summary>
/// The KeyboardHookStruct structure contains information about a low-level keyboard input event. 
/// </summary>
/// <remarks>
/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal struct KeyboardHookStruct
{
	/// <summary>
	/// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
	/// </summary>
	public int VirtualKeyCode;
	/// <summary>
	/// Specifies a hardware scan code for the key. 
	/// </summary>
	public int ScanCode;
	/// <summary>
	/// Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
	/// </summary>
	public int Flags;
	/// <summary>
	/// Specifies the Time stamp for this message.
	/// </summary>
	public int Time;
	/// <summary>
	/// Specifies extra information associated with the message. 
	/// </summary>
	public int ExtraInfo;
}

internal static class Messages
{
	//values from Winuser.h in Microsoft SDK.

	/// <summary>
	/// The WM_MOUSEMOVE message is posted to a window when the cursor moves. 
	/// </summary>
	public const int WM_MOUSEMOVE = 0x200;

	/// <summary>
	/// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button 
	/// </summary>
	public const int WM_LBUTTONDOWN = 0x201;

	/// <summary>
	/// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
	/// </summary>
	public const int WM_RBUTTONDOWN = 0x204;

	/// <summary>
	/// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button 
	/// </summary>
	public const int WM_MBUTTONDOWN = 0x207;

	/// <summary>
	/// The WM_LBUTTONUP message is posted when the user releases the left mouse button 
	/// </summary>
	public const int WM_LBUTTONUP = 0x202;

	/// <summary>
	/// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
	/// </summary>
	public const int WM_RBUTTONUP = 0x205;

	/// <summary>
	/// The WM_MBUTTONUP message is posted when the user releases the middle mouse button 
	/// </summary>
	public const int WM_MBUTTONUP = 0x208;

	/// <summary>
	/// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button 
	/// </summary>
	public const int WM_LBUTTONDBLCLK = 0x203;

	/// <summary>
	/// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button 
	/// </summary>
	public const int WM_RBUTTONDBLCLK = 0x206;

	/// <summary>
	/// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button 
	/// </summary>
	public const int WM_MBUTTONDBLCLK = 0x209;

	/// <summary>
	/// The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel. 
	/// </summary>
	public const int WM_MOUSEWHEEL = 0x020A;

	/// <summary>
	/// The WM_XBUTTONDOWN message is posted when the user presses the first or second X mouse
	/// button. 
	/// </summary>
	public const int WM_XBUTTONDOWN = 0x20B;

	/// <summary>
	/// The WM_XBUTTONUP message is posted when the user releases the first or second X  mouse
	/// button.
	/// </summary>
	public const int WM_XBUTTONUP = 0x20C;

	/// <summary>
	/// The WM_XBUTTONDBLCLK message is posted when the user double-clicks the first or second
	/// X mouse button.
	/// </summary>
	/// <remarks>Only windows that have the CS_DBLCLKS style can receive WM_XBUTTONDBLCLK messages.</remarks>
	public const int WM_XBUTTONDBLCLK = 0x20D;

	/// <summary>
	/// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem 
	/// key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
	/// </summary>
	public const int WM_KEYDOWN = 0x100;

	/// <summary>
	/// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem 
	/// key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, 
	/// or a keyboard key that is pressed when a window has the keyboard focus.
	/// </summary>
	public const int WM_KEYUP = 0x101;

	/// <summary>
	/// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user 
	/// presses the F10 key (which activates the menu bar) or holds down the ALT key and then 
	/// presses another key. It also occurs when no window currently has the keyboard focus; 
	/// in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that 
	/// receives the message can distinguish between these two contexts by checking the context 
	/// code in the lParam parameter. 
	/// </summary>
	public const int WM_SYSKEYDOWN = 0x104;

	/// <summary>
	/// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user 
	/// releases a key that was pressed while the ALT key was held down. It also occurs when no 
	/// window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent 
	/// to the active window. The window that receives the message can distinguish between 
	/// these two contexts by checking the context code in the lParam parameter. 
	/// </summary>
	public const int WM_SYSKEYUP = 0x105;
}


internal class Mouse
{
	/// <summary>
	/// The GetDoubleClickTime function retrieves the current double-click time for the mouse. A double-click is a series of two clicks of the 
	/// mouse button, the second occurring within a specified time after the first. The double-click time is the maximum number of 
	/// milliseconds that may occur between the first and second click of a double-click. 
	/// </summary>
	/// <returns>
	/// The return value specifies the current double-click time, in milliseconds. 
	/// </returns>
	/// <remarks>
	/// http://msdn.microsoft.com/en-us/library/ms646258(VS.85).aspx
	/// </remarks>
	[DllImport("user32")]
	internal static extern int GetDoubleClickTime();
}

/// <summary>
/// The <see cref="MouseStruct"/> structure contains information about a mouse input event.
/// </summary>
/// <remarks>
/// See full documentation at http://globalmousekeyhook.codeplex.com/wikipage?title=MouseStruct
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
internal struct MouseStruct
{
	/// <summary>
	/// Specifies a Point structure that contains the X- and Y-coordinates of the cursor, in screen coordinates. 
	/// </summary>
	[FieldOffset(0x00)]
	public HookPoint Point;

	/// <summary>
	/// Specifies information associated with the message.
	/// </summary>
	/// <remarks>
	/// The possible values are:
	/// <list type="bullet">
	/// <item>
	/// <description>0 - No Information</description>
	/// </item>
	/// <item>
	/// <description>1 - X-Button1 Click</description>
	/// </item>
	/// <item>
	/// <description>2 - X-Button2 Click</description>
	/// </item>
	/// <item>
	/// <description>120 - Mouse Scroll Away from User</description>
	/// </item>
	/// <item>
	/// <description>-120 - Mouse Scroll Toward User</description>
	/// </item>
	/// </list>
	/// </remarks>
	[FieldOffset(0x0A)]
	public short MouseData;

	/// <summary>
	/// Returns a Timestamp associated with the input, in System Ticks.
	/// </summary>
	[FieldOffset(0x10)]
	public int Timestamp;
}

/// <summary>
/// The AppMouseStruct structure contains information about a application-level mouse input event.
/// </summary>
/// <remarks>
/// See full documentation at http://globalmousekeyhook.codeplex.com/wikipage?title=MouseStruct
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
internal struct AppMouseStruct
{

	/// <summary>
	/// Specifies a Point structure that contains the X- and Y-coordinates of the cursor, in screen coordinates. 
	/// </summary>
	[FieldOffset(0x00)]
	public HookPoint Point;

	/// <summary>
	/// Specifies information associated with the message.
	/// </summary>
	/// <remarks>
	/// The possible values are:
	/// <list type="bullet">
	/// <item>
	/// <description>0 - No Information</description>
	/// </item>
	/// <item>
	/// <description>1 - X-Button1 Click</description>
	/// </item>
	/// <item>
	/// <description>2 - X-Button2 Click</description>
	/// </item>
	/// <item>
	/// <description>120 - Mouse Scroll Away from User</description>
	/// </item>
	/// <item>
	/// <description>-120 - Mouse Scroll Toward User</description>
	/// </item>
	/// </list>
	/// </remarks>
#if IS_X64
	[FieldOffset(0x22)]
#else
	[FieldOffset(0x16)]
#endif
	public short MouseData;

	/// <summary>
	/// Converts the current <see cref="AppMouseStruct"/> into a <see cref="MouseStruct"/>.
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// The AppMouseStruct does not have a timestamp, thus one is generated at the time of this call.
	/// </remarks>
	public MouseStruct ToMouseStruct()
	{
		MouseStruct tmp = new()
		{
			Point = Point,
			MouseData = MouseData,
			Timestamp = Environment.TickCount
		};
		return tmp;
	}
}

/// <summary>
/// The Point structure defines the X- and Y- coordinates of a point. 
/// </summary>
/// <remarks>
/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdi/rectangl_0tiq.asp
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal struct HookPoint {
	/// <summary>
	/// Specifies the X-coordinate of the point. 
	/// </summary>
	public int X;
	/// <summary>
	/// Specifies the Y-coordinate of the point. 
	/// </summary>
	public int Y;

	public HookPoint(int x, int y)
	{
		X = x;
		Y = y;
	}

	public static bool operator ==(HookPoint a, HookPoint b)
	{
		return a.X == b.X && a.Y == b.Y;
	}

	public static bool operator !=(HookPoint a, HookPoint b)
	{
		return !(a == b);
	}

	public bool Equals(HookPoint other)
	{
		return other.X == X && other.Y == Y;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (obj.GetType() != typeof (HookPoint)) return false;
		return Equals((HookPoint) obj);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			return (X*397) ^ Y;
		}
	}
}
#endregion