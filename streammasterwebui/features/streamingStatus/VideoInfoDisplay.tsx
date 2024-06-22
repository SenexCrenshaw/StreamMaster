import { SMCard } from '@components/sm/SMCard';
import SMPopUp from '@components/sm/SMPopUp';
import { GetVideoInfoFromId } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { GetVideoInfoFromIdRequest, VideoInfo } from '@lib/smAPI/smapiTypes';
import { ScrollPanel } from 'primereact/scrollpanel';
import { useCallback, useState } from 'react';

type VideoInfoProps = {
  channelId: number;
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

export const VideoInfoDisplay: React.FC<VideoInfoProps> = ({ channelId }) => {
  const [videoInfo, setVideoInfo] = useState<VideoInfo | null>(null);

  const getVideoInfo = useCallback(() => {
    if (channelId === 0 || videoInfo?.Format !== undefined) return;

    const request = { SMChannelId: channelId } as GetVideoInfoFromIdRequest;

    GetVideoInfoFromId(request)
      .then((data) => {
        if (data === null || data === undefined) return;

        setVideoInfo(data);
      })
      .catch((error) => {
        console.error(error);
      });
  }, [channelId, videoInfo?.Format]);

  const getContent = useCallback(() => {
    if (!videoInfo) return <div>Loading...</div>;
    return (
      <div>
        {videoInfo.Format && (
          <SMCard title="Format" info="">
            <DisplayTable data={videoInfo.Format} />
          </SMCard>
        )}

        {videoInfo.Streams && videoInfo.Streams.length > 0 && (
          <>
            <div className="layout-padding-bottom-lg" />
            <SMCard title="Video Streams" info="" noBorderChildren>
              {videoInfo.Streams.map((stream, index) => (
                <div key={index}>
                  <div className="layout-padding-bottom" />
                  <SMCard title={'Stream ' + (index + 1)} info="">
                    <DisplayTable data={stream} />
                  </SMCard>
                </div>
              ))}
            </SMCard>
          </>
        )}
      </div>
    );
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
      <ScrollPanel style={{ height: '50vh', width: '100%' }}>{getContent()}</ScrollPanel>
    </SMPopUp>
  );
};
