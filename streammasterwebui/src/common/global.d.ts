
type StreamMasterApiData = {
  apiKey: string,
  apiRoot: string,
  baseHostURL: string,
  hubName: string,
  isDev: boolean,
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
