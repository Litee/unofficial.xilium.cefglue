using System;
using Gtk;
using Xilium.CefGlue;
using CefGlue.Demo.GtkSharp;

public partial class MainWindow : Gtk.Window
{
    public MainWindow (): base (Gtk.WindowType.Toplevel)
    {
        Build ();

        for (var i = 0; i < _tabs.NPages; i++)
            _tabs.RemovePage(0);

        NewTab(this, EventArgs.Empty);
    }

    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
    {
        a.RetVal = true;
    }

    private void NewTab(object sender, EventArgs e)
    {
        var label = new Label("New Tab");
        var content = new WebBrowserWidget();
        _tabs.AppendPage(content, label);
        content.Visible = true;
        _tabs.CurrentPage = _tabs.PageNum(content);
    }
}
