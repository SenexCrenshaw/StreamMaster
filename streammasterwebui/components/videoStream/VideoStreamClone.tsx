import VideoStreamPanel from '@components/videoStreamPanel/VideoStreamPanel';

import { Dialog } from 'primereact/dialog';
import { classNames } from 'primereact/utils';

import { memo } from 'react';

const VideoStreamClone = (props: VideoStreamCloneProperties) => {
  const { disabled } = props;

  const className = classNames('text-base text-color', props.className, {
    'p-disabled': disabled
  });

  const ReturnToParent = () => {
    props.onClose();
  };

  return (
    <Dialog
      className={className}
      header="Clone Channel"
      onHide={() => {
        ReturnToParent();
      }}
      style={{ width: '50vw' }}
      visible={props.visible}
    >
      <VideoStreamPanel
        onSave={() => {
          ReturnToParent();
        }}
        videoStream={props.VideoStream}
      />
    </Dialog>
  );
};

VideoStreamClone.displayName = 'Clone Channel';
// VideoStreamClone.defaultProps = {
//   className: null,
//   disabled: false,

//   visible: true,
// };
interface VideoStreamCloneProperties {
  readonly VideoStream: VideoStreamDto | undefined;
  readonly className?: string | null;
  readonly disabled?: boolean;
  readonly onClose: () => void;
  readonly visible?: boolean;
}

export default memo(VideoStreamClone);
