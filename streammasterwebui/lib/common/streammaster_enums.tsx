export enum AuthenticationType {
  None = 0,
  // = 1,
  Forms = 2
}

export enum SMFileTypes {
  M3U = 0,
  EPG = 1,
  HDHR = 2,
  Channel = 3,
  M3UStream = 4,
  Icon = 5,
  Image = 6,
  TvLogo = 7,
  ProgrammeIcon = 8,
  ChannelIcon = 9
}

export enum StreamingProxyTypes {
  SystemDefault = 0,
  None = 1,
  StreamMaster = 2,
  FFMpeg = 3
}

export enum VideoStreamHandlers {
  SystemDefault = 0,
  None = 1,
  Loop = 2
}

export enum M3UFileStreamUrlPrefix {
  SystemDefault = 0,
  TS = 1,
  M3U8 = 2
}
