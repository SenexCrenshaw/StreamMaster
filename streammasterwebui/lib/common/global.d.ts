
export type StreamMasterApiData = {
  apiKey: string,
  apiRoot: string,
  baseHostURL: string,
  isDebug: boolean,
  urlBase: string,
  version: string,
};


declare global {
  
  interface Window {
    StreamMaster: StreamMasterApiData;
  }
}
export { };

