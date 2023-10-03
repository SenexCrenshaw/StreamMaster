const StreamMaster = typeof window !== 'undefined' ? window.StreamMaster : undefined;

export const baseHostURL = StreamMaster?.baseHostURL ?? "http://127.0.0.1:7095";
export const apiRoot = StreamMaster?.apiRoot ?? "/api";
export const apiKey = StreamMaster?.apiKey ?? "";
export const isDebug = StreamMaster?.isDebug ?? true;
export const urlBase = StreamMaster?.urlBase ?? "";
export const version = StreamMaster?.version ?? "DEV";
