import React from 'react';
import * as StreamMasterApi from '../iptvApi';

const StreamMasterSetting = (): StreamMasterSettingResponse => {
  const settingsQuery = StreamMasterApi.useSettingsGetSettingQuery();
  const [isLoading, setIsLoading] = React.useState<boolean>(true);
  const [data, setData] = React.useState<StreamMasterApi.SettingDto>({} as StreamMasterApi.SettingDto);
  const [streamMasterIcon, setStreamMasterIcon] = React.useState<string>('');
  const [defaultIcon, setDefaultIcon] = React.useState<string>('');
  const [defaultIconName, setDefaultIconName] = React.useState<string>('');
  const [defaultIconDto, setDefaultIconDto] = React.useState<StreamMasterApi.IconFileDto>({} as StreamMasterApi.IconFileDto);

  // React.useEffect(() => {
  //   setIsLoading(settingsQuery.isLoading);
  // }, [settingsQuery.isLoading]);

  React.useEffect(() => {

    if (
      settingsQuery.isLoading ||
      !settingsQuery.data ||
      settingsQuery.data === undefined
    ) {
      return;
    }

    setIsLoading(true);

    if (
      settingsQuery.data.defaultIcon &&
      settingsQuery.data.streamMasterIcon &&
      settingsQuery.data.baseHostURL
    ) {
      setStreamMasterIcon(settingsQuery.data.baseHostURL + settingsQuery.data.streamMasterIcon);
      setDefaultIcon(settingsQuery.data.baseHostURL + settingsQuery.data.defaultIcon);
      setDefaultIconName(settingsQuery.data.defaultIcon);

      if (settingsQuery.data.defaultIconDto) {
        setDefaultIconDto(settingsQuery.data.defaultIconDto);
      }
    }

    setData(settingsQuery.data);
    setIsLoading(false);

  }, [settingsQuery.data, settingsQuery.isLoading]);

  return {
    data,
    defaultIcon,
    defaultIconDto,
    defaultIconName,
    isLoading,
    streamMasterIcon,
  };
};

export type StreamMasterSettingResponse = {
  data: StreamMasterApi.SettingDto;
  defaultIcon: string;
  defaultIconDto: StreamMasterApi.IconFileDto;
  defaultIconName: string;
  isLoading: boolean;
  streamMasterIcon: string;
};


export default StreamMasterSetting;
