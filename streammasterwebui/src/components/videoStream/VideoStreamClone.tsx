
import { Dialog } from 'primereact/dialog';
import VideoStreamPanel from '../../features/videoStreamPanel/VideoStreamPanel';
import { classNames } from 'primereact/utils';
import { type VideoStreamDto } from '../../store/iptvApi';
import { memo } from 'react';

const VideoStreamClone = (props: VideoStreamCloneProps) => {

  const disabled = props.disabled;

  const className = classNames('text-base text-color', props.className, {
    'p-disabled': disabled,
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

        onSave={() => { ReturnToParent() }}
        videoStream={props.VideoStream}

      />
    </Dialog>

  );
};

VideoStreamClone.displayName = 'Clone Channel';
VideoStreamClone.defaultProps = {
  className: null,
  disabled: false,

  visible: true,
};
type VideoStreamCloneProps = {
  VideoStream: VideoStreamDto | undefined;
  className?: string | null;
  disabled?: boolean;
  onClose: (() => void);
  visible?: boolean;
};


export default memo(VideoStreamClone);
