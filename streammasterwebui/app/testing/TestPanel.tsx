import { VideoStreamDto } from '@lib/iptvApi';
import { GetVideoStreamInfoFromId } from '@lib/smAPI/VideoStreams/VideoStreamsGetAPI';
import { memo, useEffect } from 'react';

export interface StreamDataSelectorProperties {
  readonly data: VideoStreamDto;
  readonly enableEditMode?: boolean;
}

const TestPanel = () => {
  // const [videoStreamDto, setVideoStreamDto]=-
  useEffect(() => {
    GetVideoStreamInfoFromId('3c8ef639142e3ffc5dfe169c9dd70979').then((data) => {
      console.log(data);
    });
  }, []);

  return <div></div>;
};
export default memo(TestPanel);
