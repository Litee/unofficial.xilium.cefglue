namespace Xilium.CefGlue
{
    using System;
    using Xilium.CefGlue.Interop;

    public abstract unsafe partial class CefExtensionHandler
    {
        private int get_extension_resource(cef_extension_handler_t* self, cef_extension_t* extension, cef_browser_t* browser, cef_string_t* file, cef_get_extension_resource_callback_t* callback)
        {
            throw new NotImplementedException();
        }

        private int can_access_browser(cef_extension_handler_t* self, cef_extension_t* extension, cef_browser_t* browser, int include_incognito, cef_browser_t* target_browser)
        {
            throw new NotImplementedException();
        }

        private cef_browser_t* get_active_browser(cef_extension_handler_t* self, cef_extension_t* extension, cef_browser_t* browser, int include_incognito)
        {
            throw new NotImplementedException();
        }

        private int on_before_browser(cef_extension_handler_t* self, cef_extension_t* extension, cef_browser_t* browser, cef_browser_t* active_browser, int index, cef_string_t* url, int active, cef_window_info_t* windowInfo, cef_client_t** client, cef_browser_settings_t* settings)
        {
            throw new NotImplementedException();
        }

        private int on_before_background_browser(cef_extension_handler_t* self, cef_extension_t* extension, cef_string_t* url, cef_client_t** client, cef_browser_settings_t* settings)
        {
            throw new NotImplementedException();
        }

        private void on_extension_unloaded(cef_extension_handler_t* self, cef_extension_t* extension)
        {
            throw new NotImplementedException();
        }

        private void on_extension_loaded(cef_extension_handler_t* self, cef_extension_t* extension)
        {
            throw new NotImplementedException();
        }

        private void on_extension_load_failed(cef_extension_handler_t* self, CefErrorCode result)
        {
            throw new NotImplementedException();
        }

    }
}
