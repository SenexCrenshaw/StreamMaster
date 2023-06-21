// help_en.js
const help_en = {
  adminPassword:"Admin Password",
  adminUserName: "Admin Username",
  apiKey: "API Key",
  auth: "Authentication",
  authenticationMethod: "Authentication Method",
  backup:'Backup',
  cacheIcons: 'Cache Icons to the local disk to speed things up',
  cleanURLs: 'Clean URLs from Logs',
  development: 'Development',
  deviceID: 'HDHR Device ID and capability ID',
  ffmPegExecutable:'FFMPeg Executable. The name "ffmpeg(.exe)" will be search in the OS path as well',
  filesEPG: 'Files / EPG',
  general: 'General',
  keywordSearch: 'Keyword Search',
  maxConnectRetry:'How many times to retry receiving data from the source stream',
  maxConnectRetryTimeMS: 'Receiving Data Retry Timeout in MS',
  overWriteM3UChannels: 'Overwrite M3U Channels Numbers even if they are set',
  password: "Password",
  rememberme:'Remember Me',
  ringBufferSizeMB: 'Buffer Size (MB)',
  sdPassword: 'Sched Direct Password',
  sdUserName: 'Sched Direct Username',
  settings: 'Settings',
  signin: "Sign In",
  signInSuccessful:'Sign In Successful',
  signInUnSuccessful:'Sign In Unsuccessful',
  signout: "Sign Out",
  streaming: 'Streaming',
  streamingProxyType: 'Stream Buffer Type to use. None will just use the original M3U URLs, FFMPEG - run the stream through FFMPEG, Streram Master - run the stream through SM own proxy',
  user: "User",
};

export const getHelp = (key) => {
  return help_en[key];
}

export default help_en;

