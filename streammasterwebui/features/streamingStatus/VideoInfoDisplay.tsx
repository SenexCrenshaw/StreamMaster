import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { GetVideoInfoRequest, VideoInfo, VideoInfoDto } from '@lib/smAPI/smapiTypes';
import { GetVideoInfo } from '@lib/smAPI/Statistics/StatisticsCommands';
import useGetVideoInfos from '@lib/smAPI/Statistics/useGetVideoInfos';
import { JsonEditor } from 'json-edit-react';

import { ScrollPanel } from 'primereact/scrollpanel';
import { useCallback, useMemo, useState } from 'react';

type VideoInfoProps = {
  name: string;
  videoInfo?: string;
};

export const VideoInfoDisplay: React.FC<VideoInfoProps> = ({ name, videoInfo }) => {
  const getContent = useMemo(() => {
    if (!videoInfo) return <div>Loading...</div>;
    let jsonObject = JSON.parse(videoInfo);
    return <JsonEditor data={jsonObject} restrictEdit restrictDelete restrictAdd theme="githubDark" />;
  }, [videoInfo]);

  return (
    <SMPopUp
      buttonClassName="icon-blue"
      buttonDisabled={videoInfo === null}
      info=""
      noBorderChildren
      contentWidthSize="4"
      placement="bottom-end"
      icon="pi-video"
      title={'Video Info : ' + name}
      tooltip="Video Info"
      isLeft
    >
      <ScrollPanel style={{ height: '50vh', width: '100%' }}>{getContent}</ScrollPanel>
    </SMPopUp>
  );
};
