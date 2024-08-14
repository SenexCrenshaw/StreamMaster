import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { GetVideoInfoRequest, VideoInfo } from '@lib/smAPI/smapiTypes';
import { GetVideoInfo } from '@lib/smAPI/Statistics/StatisticsCommands';
import useGetVideoInfos from '@lib/smAPI/Statistics/useGetVideoInfos';
import { JsonEditor } from 'json-edit-react';

import { ScrollPanel } from 'primereact/scrollpanel';
import { useCallback, useMemo, useState } from 'react';

type VideoInfoProps = {
  smStreamId: string;
};

export const VideoInfoDisplay: React.FC<VideoInfoProps> = ({ smStreamId }) => {
  const [videoInfo, setVideoInfo] = useState<VideoInfo | null>(null);
  const { data: videoInfos } = useGetVideoInfos();
  const getVideoInfo = useCallback(() => {
    // if (smStreamId === null || videoInfo?.Format !== undefined) return;

    const request = { SMStreamId: smStreamId } as GetVideoInfoRequest;

    GetVideoInfo(request)
      .then((videoInfo) => {
        if (videoInfo === null || videoInfo === undefined) return;

        setVideoInfo(videoInfo);
      })
      .catch((error) => {
        console.error(error);
      });
  }, [smStreamId]);

  const getContent = useMemo(() => {
    if (!videoInfo) return <div>Loading...</div>;

    let jsonObject = JSON.parse(videoInfo.JsonOutput);
    Logger.debug('VideoInfoDisplay:', jsonObject);
    return <JsonEditor data={jsonObject} restrictEdit restrictDelete restrictAdd theme="githubDark" />;
  }, [videoInfo]);

  const hasVideoInfo = useMemo(() => {
    if (videoInfos === undefined || videoInfos === null) return false;

    const found = videoInfos.some((info: VideoInfo) => info.StreamId === smStreamId);

    return found;
  }, [smStreamId, videoInfos]);

  return (
    <SMPopUp
      buttonClassName="icon-blue"
      buttonDisabled={!hasVideoInfo}
      onOpen={(e) => {
        if (e === true) {
          getVideoInfo();
        }
      }}
      info=""
      noBorderChildren
      contentWidthSize="4"
      placement="bottom-end"
      icon="pi-id-card"
      title={'Video Info : ' + videoInfo?.StreamName}
      tooltip="Stream Info"
      isLeft
    >
      <ScrollPanel style={{ height: '50vh', width: '100%' }}>{getContent}</ScrollPanel>
    </SMPopUp>
  );
};
