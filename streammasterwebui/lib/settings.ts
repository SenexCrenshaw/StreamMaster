const windowDefined = typeof window !== "undefined";

export const isDev = process.env.NODE_ENV=== "development";
export const baseHostURL = windowDefined &&!isDev ? (window.location.protocol + "//" + window.location.host) : "http://127.0.0.1:7095";
export const apiRoot = windowDefined && window.StreamMaster ? (window.StreamMaster.apiRoot ?? "/api") : "/api";
export const apiKey = windowDefined && window.StreamMaster ? (window.StreamMaster.apiKey ?? "") : "";
export const isDebug =  windowDefined && window.StreamMaster ? (window.StreamMaster.isDebug ?? false) : false;
export const urlBase =  windowDefined && window.StreamMaster ? (window.StreamMaster.urlBase ?? "") : "";
export const version = windowDefined && window.StreamMaster ? (window.StreamMaster.version ?? "DEV") : "DEV";
