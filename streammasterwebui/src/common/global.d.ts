
type StreamMasterApiData = {
  apiKey: string,
  apiRoot: string,
  baseHostURL: string,
  isDebug: boolean,
  urlBase: string,
  version: string,
};


declare global {
  // eslint-disable-next-line @typescript-eslint/consistent-type-definitions
  interface Window {
    StreamMaster: StreamMasterApiData;
  }
}
export { };
