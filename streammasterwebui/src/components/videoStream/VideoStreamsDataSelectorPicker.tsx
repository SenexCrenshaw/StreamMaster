import { memo } from "react";
import VideoStreamDataSelector from "./VideoStreamDataSelector";

const VideoStreamsDataSelectorPicker = () => {
  return (
    <div className='grid grid-nogutter flex flex-wrap justify-content-between h-full col-12 p-0'>
      <div className='col-6'>
        <VideoStreamDataSelector

          enableEditMode={false}
          id="VideoStreamsDataSelectorPicker"
          showBrief
        />
      </div>
      <div className='col-6'>
        {/* <StreamGroupVideoStreamDataSelector
                    channelGroups={selectedChannelGroups}
                    enableEditMode={false}
                    id={id}
                    showBrief
                  /> */}
      </div>
    </div>
  );
}

export default memo(VideoStreamsDataSelectorPicker);
