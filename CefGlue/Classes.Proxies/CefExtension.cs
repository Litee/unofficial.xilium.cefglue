namespace Xilium.CefGlue
{
    using Xilium.CefGlue.Interop;

    public sealed unsafe partial class CefExtension
    {

        /// <summary>
        /// Returns the unique extension identifier. This is calculated based on the
        /// extension public key, if available, or on the extension path. See
        /// https://developer.chrome.com/extensions/manifest/key for details.
        /// </summary>
        public string GetIdentifier()
        {
            var n_result = cef_extension_t.get_identifier(_self);
            return cef_string_userfree.ToString(n_result);
        }

        /// <summary>
        /// Returns the absolute path to the extension directory on disk. This value
        /// will be prefixed with PK_DIR_RESOURCES if a relative path was passed to
        /// CefRequestContext::LoadExtension.
        /// </summary>
        public string GetPath()
        {
            var n_result = cef_extension_t.get_path(_self);
            return cef_string_userfree.ToString(n_result);
        }

        /// <summary>
        /// Returns the extension manifest contents as a CefDictionaryValue object. See
        /// https://developer.chrome.com/extensions/manifest for details.
        /// </summary>
        public CefDictionaryValue GetManifest()
        {
            var n_result = cef_extension_t.get_manifest(_self);
            return CefDictionaryValue.FromNative(n_result);
        }

        /// <summary>
        /// Returns true if this object is the same extension as |that| object.
        /// Extensions are considered the same if identifier, path and loader context
        /// match.
        /// </summary>
        public bool IsSame(CefExtension that)
        {
            if (that == null) return false;
            return cef_extension_t.is_same(_self, that.ToNative()) != 0;
        }

        /// <summary>
        /// Returns the handler for this extension. Will return NULL for internal
        /// extensions or if no handler was passed to CefRequestContext::LoadExtension.
        /// </summary>
        public CefExtensionHandler GetHandler()
        {
            var n_result = cef_extension_t.get_handler(_self);
            return CefExtensionHandler.FromNativeOrNull(n_result);
        }

        /// <summary>
        /// Returns the request context that loaded this extension. Will return NULL
        /// for internal extensions or if the extension has been unloaded. See the
        /// CefRequestContext::LoadExtension documentation for more information about
        /// loader contexts. Must be called on the browser process UI thread.
        /// </summary>
        public CefRequestContext GetLoaderContext()
        {
            var n_result = cef_extension_t.get_loader_context(_self);
            return CefRequestContext.FromNativeOrNull(n_result);
        }

        /// <summary>
        /// Returns true if this extension is currently loaded. Must be called on the
        /// browser process UI thread.
        /// </summary>
        public bool IsLoaded()
        {
            return cef_extension_t.is_loaded(_self) != 0;
        }

        /// <summary>
        /// Unload this extension if it is not an internal extension and is currently
        /// loaded. Will result in a call to CefExtensionHandler::OnExtensionUnloaded
        /// on success.
        /// </summary>
        public void Unload()
        {
            cef_extension_t.unload(_self);
        }
    }
}
