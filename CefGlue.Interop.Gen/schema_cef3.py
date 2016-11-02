#
# TODO: 
# Copyright (C) CefGlue Project
#

from schema import *

classdef = {
    'CefUserData': { 'role': ROLE_HANDLER, 'reversible': True },

    'CefApp': { 'role': ROLE_HANDLER, 'name': 'CefApp' },
    'CefBrowser': { 'role': ROLE_PROXY },
    'CefBrowserHost': { 'role': ROLE_PROXY },
    'CefCallback': { 'role': ROLE_PROXY },
    'CefClient': { 'role': ROLE_HANDLER, 'reversible': True },
    'CefDisplayHandler': { 'role': ROLE_HANDLER },
    'CefFrame': { 'role': ROLE_PROXY },
    'CefGeolocationCallback': { 'role': ROLE_PROXY },
    'CefGeolocationHandler': { 'role': ROLE_HANDLER },
    'CefLifeSpanHandler': { 'role': ROLE_HANDLER },
    'CefLoadHandler': { 'role': ROLE_HANDLER },
    'CefProcessMessage': { 'role': ROLE_PROXY },
    'CefProxyHandler': { 'role': ROLE_HANDLER },
    'CefRenderProcessHandler': { 'role': ROLE_HANDLER },
    'CefRequest': { 'role': ROLE_PROXY },
    'CefPostData': { 'role': ROLE_PROXY },
    'CefPostDataElement': { 'role': ROLE_PROXY },
    'CefAuthCallback': { 'role': ROLE_PROXY },
    'CefRequestHandler': { 'role': ROLE_HANDLER },
    'CefResourceBundleHandler': { 'role': ROLE_HANDLER },
    'CefResourceHandler': { 'role': ROLE_HANDLER, 'autodispose': True },
    'CefResponse': { 'role': ROLE_PROXY },
    'CefSchemeHandlerFactory': { 'role': ROLE_HANDLER },
    'CefReadHandler': { 'role': ROLE_HANDLER },
    'CefStreamReader': { 'role': ROLE_PROXY },
    'CefWriteHandler': { 'role': ROLE_HANDLER },
    'CefStreamWriter': { 'role': ROLE_PROXY },
    'CefStringVisitor': { 'role': ROLE_HANDLER },
    'CefTask': { 'role': ROLE_HANDLER },
    'CefV8Context': { 'role': ROLE_PROXY },
    'CefV8Handler': { 'role': ROLE_HANDLER, 'reversible': True },
    'CefV8Accessor': { 'role': ROLE_HANDLER },
    'CefV8Exception': { 'role': ROLE_PROXY },
    'CefV8Value': { 'role': ROLE_PROXY },
    'CefV8StackTrace': { 'role': ROLE_PROXY },
    'CefV8StackFrame': { 'role': ROLE_PROXY },
    'CefBinaryValue': { 'role': ROLE_PROXY },
    'CefDictionaryValue': { 'role': ROLE_PROXY },
    'CefListValue': { 'role': ROLE_PROXY },
    'CefXmlReader': { 'role': ROLE_PROXY },
    'CefZipReader': { 'role': ROLE_PROXY },
    'CefContextMenuHandler': { 'role': ROLE_HANDLER },
    'CefContextMenuParams': { 'role': ROLE_PROXY },
    'CefCommandLine': { 'role': ROLE_PROXY },
    'CefCookieManager': { 'role': ROLE_PROXY },
    'CefCookieVisitor': { 'role': ROLE_HANDLER, 'autodispose': True },
    'CefDOMVisitor': { 'role': ROLE_HANDLER, 'name': 'CefDomVisitor' },
    'CefDOMDocument': { 'role': ROLE_PROXY, 'name': 'CefDomDocument' },
    'CefDOMNode': { 'role': ROLE_PROXY, 'name': 'CefDomNode' },
    'CefDOMEvent': { 'role': ROLE_PROXY, 'name': 'CefDomEvent' },
    'CefDOMEventListener': { 'role': ROLE_HANDLER, 'name': 'CefDomEventListener' },
    'CefJSDialogCallback': { 'role': ROLE_PROXY },
    'CefJSDialogHandler': { 'role': ROLE_HANDLER },
    'CefMenuModel': { 'role': ROLE_PROXY },
    'CefSchemeRegistrar': { 'role': ROLE_PROXY },
    'CefWebPluginInfo': { 'role': ROLE_PROXY },
    'CefWebPluginInfoVisitor': { 'role': ROLE_HANDLER },
    'CefFocusHandler': { 'role': ROLE_HANDLER },
    'CefKeyboardHandler': { 'role': ROLE_HANDLER },
    'CefBrowserProcessHandler': { 'role': ROLE_HANDLER },
    'CefURLRequest': { 'role': ROLE_PROXY, 'name': 'CefUrlRequest' },
    'CefURLRequestClient': { 'role': ROLE_HANDLER, 'name': 'CefUrlRequestClient', 'reversible': True },
    'CefBeforeDownloadCallback': { 'role': ROLE_PROXY },
    'CefDownloadItemCallback': { 'role': ROLE_PROXY },
    'CefDownloadHandler': { 'role': ROLE_HANDLER },
    'CefDownloadItem': { 'role': ROLE_PROXY },
    'CefQuotaCallback': { 'role': ROLE_PROXY },
    'CefWebPluginUnstableCallback': { 'role': ROLE_HANDLER },
    # R880
    'CefRunFileDialogCallback': { 'role': ROLE_HANDLER },
    'CefFileDialogCallback': { 'role': ROLE_PROXY },
    'CefDialogHandler': { 'role': ROLE_HANDLER },
    'CefGetGeolocationCallback': { 'role': ROLE_HANDLER },
    'CefTraceClient': { 'role': ROLE_HANDLER },
    # R929
    'CefRenderHandler': { 'role': ROLE_HANDLER },
    # R980
    'CefTaskRunner': { 'role': ROLE_PROXY },
    # R1102
    'CefCompletionHandler': { 'role': ROLE_HANDLER },
    # R1177
    'CefAllowCertificateErrorCallback': { 'role': ROLE_PROXY },
    # R
    'CefDragData': { 'role': ROLE_PROXY },
    'CefDragHandler': { 'role': ROLE_HANDLER },
    # 1650.1503
    'CefRequestContext': { 'role': ROLE_PROXY },
    'CefRequestContextHandler': { 'role': ROLE_HANDLER, 'reversible': True },
    # 1750
    'CefEndTracingCallback': { 'role': ROLE_HANDLER },
    # 1916
    'CefCompletionCallback': { 'role': ROLE_HANDLER },
    # 2062
    'CefPrintDialogCallback': { 'role': ROLE_PROXY },
    'CefPrintJobCallback': { 'role': ROLE_PROXY },
    'CefPrintHandler': { 'role': ROLE_HANDLER },
    'CefPrintSettings': { 'role': ROLE_PROXY },
    # r1949
    'CefNavigationEntryVisitor': { 'role': ROLE_HANDLER },
    'CefNavigationEntry': { 'role': ROLE_PROXY },
    # 2272 branch
    # r2035
    'CefFindHandler': { 'role': ROLE_HANDLER },
    'CefSSLCertPrincipal': { 'role': ROLE_PROXY, 'name': 'CefSslCertPrincipal' },
    'CefSSLInfo': { 'role': ROLE_PROXY, 'name': 'CefSslInfo' },
    # 2357
    'CefSetCookieCallback': { 'role': ROLE_HANDLER, 'autodispose': True },
    'CefDeleteCookiesCallback': { 'role': ROLE_HANDLER, 'autodispose': True },
    'CefRequestCallback': { 'role': ROLE_PROXY },
    'CefValue': { 'role': ROLE_PROXY },
    # 2424
    'CefPdfPrintCallback': { 'role': ROLE_HANDLER },
    # 2454
    'CefRunContextMenuCallback': {'role': ROLE_PROXY },
    'CefResourceBundle': {'role': ROLE_PROXY },
    # 2526
    'CefResponseFilter': { 'role': ROLE_HANDLER, 'autodispose': True },
    # 2623
    'CefResolveCallback': { 'role': ROLE_HANDLER },
    # 51 (without views)
	'CefDownloadImageCallback': { 'role': ROLE_HANDLER },
	'CefImage': { 'role': ROLE_PROXY },
	'CefMenuModelDelegate': { 'role': ROLE_HANDLER },
	# 53
	'CefRegisterCdmCallback': { 'role': ROLE_HANDLER },
    }
