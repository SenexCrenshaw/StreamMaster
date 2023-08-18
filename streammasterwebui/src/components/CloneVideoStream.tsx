import { Dialog } from 'primereact/dialog';
import { classNames } from 'primereact/utils';
import React from 'react';
import type * as StreamMasterApi from '../store/iptvApi';

import VideoStreamPanel from './VideoStreamPanel';

const CloneVideoStream = (props: CloneVideoStreamProps) => {

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

CloneVideoStream.displayName = 'Clone Channel';
CloneVideoStream.defaultProps = {
  className: null,
  disabled: false,

  visible: true,
};
type CloneVideoStreamProps = {
  VideoStream: StreamMasterApi.VideoStreamDto | undefined;
  className?: string | null;
  disabled?: boolean;
  onClose: (() => void);
  visible?: boolean;
};


export default React.memo(CloneVideoStream);
