import { VideoStreamDto } from '@lib/iptvApi';
import { memo } from 'react';

interface VideoStreamGroupTitleProperties {
  readonly onClose?: () => void;
  readonly value?: VideoStreamDto | undefined;
}

const VideoStreamGroupTitle = ({ value, onClose }: VideoStreamGroupTitleProperties) => {
  return <div>{value?.user_Tvg_name} </div>;
};

export default memo(VideoStreamGroupTitle);
