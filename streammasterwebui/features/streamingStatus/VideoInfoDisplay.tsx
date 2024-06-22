import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { GetVideoInfoFromId } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { GetVideoInfoFromIdRequest, VideoInfo } from '@lib/smAPI/smapiTypes';
import { useEffect, useState } from 'react';

type VideoInfoProps = {
  channelId: number;
};

const DisplayTable: React.FC<{ data: any; title?: string }> = ({ data, title }) => {
  return (
    <div>
      {title && <h4>{title}</h4>}
      <table style={{ borderCollapse: 'collapse', width: '100%' }}>
        <tbody>
          {Object.entries(data).map(([key, value]) => (
            <tr key={key}>
              <td style={{ border: '1px solid black', padding: '5px' }}>{key}</td>
              <td style={{ border: '1px solid black', padding: '5px' }}>
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

  useEffect(() => {
    if (channelId === 0) return;
    const request = { SMChannelId: channelId } as GetVideoInfoFromIdRequest;
    Logger.debug('VideoInfoDisplay', request);
    GetVideoInfoFromId(request)
      .then((data) => {
        if (data === null || data === undefined) return;

        setVideoInfo(data);
      })
      .catch((error) => {
        console.error(error);
      });
  }, [channelId]);

  if (!videoInfo) return null;

  return (
    <SMPopUp title="Video Info">
      <div>
        {videoInfo.Format && (
          <div>
            <h2>Format</h2>
            <DisplayTable data={videoInfo.Format} />
          </div>
        )}

        {videoInfo.Streams && videoInfo.Streams.length > 0 && (
          <div>
            <h2>Video Streams</h2>
            {videoInfo.Streams.map((stream, index) => (
              <div key={index}>
                <h3 className="orange-color">Stream {index + 1}</h3>
                <DisplayTable data={stream} />
              </div>
            ))}
          </div>
        )}
      </div>
    </SMPopUp>
  );
};
