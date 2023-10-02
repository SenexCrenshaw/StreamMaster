let baseHostURLRef = 'http://127.0.0.1:7095';
let apiRootRef = "/api";
let apiKeyRef = "f835904d5a2343d8ac567c026d6c08b2";
let isDebugRef = true;
let urlBaseRef = "";
let versionRef = "DEV";

if (typeof window !== 'undefined') {
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

