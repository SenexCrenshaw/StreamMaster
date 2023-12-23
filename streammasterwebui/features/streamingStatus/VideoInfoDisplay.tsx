import { VideoInfo } from '@lib/iptvApi';

type VideoInfoProps = {
  videoInfo: VideoInfo;
};

const DisplayTable: React.FC<{ data: any; title?: string }> = ({ data, title }) => {
  return (
    <div>
      {title && <h4>{title}</h4>}
      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
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

export const VideoInfoDisplay: React.FC<VideoInfoProps> = ({ videoInfo }) => {
  return (
    <div>
      {videoInfo.format && (
        <div>
          <h2>Format</h2>
          <DisplayTable data={videoInfo.format} />
        </div>
      )}

      {videoInfo.streams && videoInfo.streams.length > 0 && (
        <div>
          <h2>Video Streams</h2>
          {videoInfo.streams.map((stream, index) => (
            <div key={index}>
              <h3 className="orange-color">Stream {index + 1}</h3>
              <DisplayTable data={stream} />
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
