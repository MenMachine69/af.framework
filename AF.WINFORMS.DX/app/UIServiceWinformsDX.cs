#nullable enable
// ReSharper disable InconsistentNaming

namespace AF.WINFORMS.DX;

/// <summary>
/// UI Service Layer für AF Anwendungen basierend auf AF.WINFORMS.DX
/// </summary>
public class UIServiceWinFormsDX : IUIServiceWinFormsDX
{

}

/// <summary>
/// Interface eines allgemeinen UI Services
/// </summary>
public interface IUIService
{

}

/// <summary>
/// Interface eines UI Services für WinForms-Anwendungen
/// </summary>
public interface IUIServiceWinForms : IUIService
{

}

/// <summary>
/// Interace eines UI Services für WinFormsDX Anwendungen
/// </summary>
public interface IUIServiceWinFormsDX : IUIServiceWinForms
{

}


