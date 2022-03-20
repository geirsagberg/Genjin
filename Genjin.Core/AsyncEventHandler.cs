namespace Genjin.Core;

internal delegate Task AsyncEventHandler<in TEventArgs>(object? sender, TEventArgs e);
