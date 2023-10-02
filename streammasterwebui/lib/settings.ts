let baseHostURLRef = '';
let apiRootRef = '';
let apiKeyRef = '';
let isDebugRef = false;
let urlBaseRef = '';
let versionRef = '';

if (typeof window !== 'undefined' && window && window.StreamMaster) {
    console.log('Setting baseHostURLRef ',window.StreamMaster.baseHostURL);
    baseHostURLRef = window.StreamMaster.baseHostURL;
    apiRootRef = window.StreamMaster.apiRoot;
    apiKeyRef = window.StreamMaster.apiKey;
    isDebugRef = window.StreamMaster.isDebug;
    urlBaseRef = window.StreamMaster.urlBase;
    versionRef = window.StreamMaster.version;
}

export const baseHostURL = baseHostURLRef;
export const apiRoot = apiRootRef;
export const apiKey = apiKeyRef;
export const isDebug = isDebugRef;
export const urlBase = urlBaseRef;
export const version = versionRef;

