
type StreamMasterApiData = {
  baseHostURL: string,
  hubName: string,
  isDev: boolean,
};


declare global {
  // eslint-disable-next-line @typescript-eslint/consistent-type-definitions
  interface Window {
    StreamMaster: StreamMasterApiData;
  }
}
export { };
