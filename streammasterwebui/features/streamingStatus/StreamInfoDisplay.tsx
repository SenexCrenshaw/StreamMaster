import { SMCard } from '@components/sm/SMCard';
import SMPopUp from '@components/sm/SMPopUp';
import { SMStreamInfo } from '@lib/smAPI/smapiTypes';

import { ScrollPanel } from 'primereact/scrollpanel';
import { useMemo } from 'react';

type StreamInfoDisplayProps = {
  streamInfo: SMStreamInfo;
};

export const StreamInfoDisplay: React.FC<StreamInfoDisplayProps> = ({ streamInfo }) => {
  const hasInfo = useMemo(() => {
    if (streamInfo === undefined || streamInfo === null) return false;

    return true;
  }, [streamInfo]);

  const getContent = useMemo(() => {
    if (!streamInfo) return <div>Loading...</div>;

    return (
      <SMCard darkBackGround>
        <div className="flex flex-column">
          <div className="flex sm-start-stuff">
            <div className="text-container sm-w-3">Command Profile:</div>
            <div className="text-container">{streamInfo.CommandProfile.ProfileName}</div>
          </div>
          <div className="flex sm-start-stuff">
            <div className="text-container sm-w-3">Command:</div>
            <div className="text-container">{streamInfo.CommandProfile.Command}</div>
          </div>
          <div className="flex sm-start-stuff">
            <div className="text-container sm-w-3">Parameters:</div>
            <div className="text-container">{streamInfo.CommandProfile.Parameters}</div>
          </div>
        </div>
      </SMCard>
    );
  }, [streamInfo]);

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      buttonDisabled={!hasInfo}
      onOpen={(e) => {
        // if (e === true) {
        //   getVideoInfo();
        // }
      }}
      info=""
      noBorderChildren
      contentWidthSize="4"
      placement="bottom-end"
      icon="pi-info-circle"
      title={'Stream Info : ' + streamInfo?.Name}
      tooltip="Stream Info"
      isLeft
    >
      <ScrollPanel style={{ height: '10vh', width: '100%' }}>{getContent}</ScrollPanel>
    </SMPopUp>
  );
};
