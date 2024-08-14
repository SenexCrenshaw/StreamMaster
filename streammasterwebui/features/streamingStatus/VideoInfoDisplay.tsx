import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { GetVideoInfoRequest, VideoInfo } from '@lib/smAPI/smapiTypes';
import { GetVideoInfo } from '@lib/smAPI/Statistics/StatisticsCommands';
import { JsonEditor } from 'json-edit-react';

import { ScrollPanel } from 'primereact/scrollpanel';
import { useCallback, useMemo, useState } from 'react';

type VideoInfoProps = {
  smStreamId: string;
};

const DisplayTable: React.FC<{ data: any; title?: string }> = ({ data, title }) => {
  return (
    <div>
      {title && <h4>{title}</h4>}
      <table style={{ width: '100%' }}>
        <tbody className="videoinfo-selector">
          {Object.entries(data).map(([key, value]) => (
            <tr key={key}>
              <td style={{ padding: '5px' }}>{key}</td>
              <td style={{ padding: '5px' }}>
                {value !== null && typeof value === 'object' ? <DisplayTable data={value} /> : value !== null && value !== undefined ? value.toString() : 'N/A'}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export const VideoInfoDisplay: React.FC<VideoInfoProps> = ({ smStreamId }) => {
  const [videoInfo, setVideoInfo] = useState<VideoInfo | null>(null);

  const getVideoInfo = useCallback(() => {
    // if (smStreamId === null || videoInfo?.Format !== undefined) return;

    const request = { SMStreamId: smStreamId } as GetVideoInfoRequest;

    GetVideoInfo(request)
      .then((data) => {
        if (data === null || data === undefined) return;

        setVideoInfo(data);
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

  return (
    <SMPopUp
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
