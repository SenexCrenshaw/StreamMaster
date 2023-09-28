let baseHostURLRef = 'http://127.0.0.1:7095';
let apiRootRef = "/api";
let apiKeyRef = "f835904d5a2343d8ac567c026d6c08b2";
let isDebugRef = true;
let urlBaseRef = "";
let versionRef = "DEV";

if (typeof window !== 'undefined') {
    baseHostURLRef = window?.StreamMaster?.baseHostURL ?? baseHostURLRef;
    apiRootRef = window?.StreamMaster?.apiRoot ?? apiRootRef;
    apiKeyRef = window?.StreamMaster?.apiKey ?? apiKeyRef;
    isDebugRef = window?.StreamMaster?.isDebug ?? isDebugRef;
    urlBaseRef = window?.StreamMaster?.urlBase ?? urlBaseRef;
    versionRef = window?.StreamMaster?.version ?? versionRef;
}

export const baseHostURL = baseHostURLRef;
export const apiRoot = apiRootRef;
export const apiKey = apiKeyRef;
export const isDebug = isDebugRef;
export const urlBase = urlBaseRef;
export const version = versionRef;
