import NumberEditor from '@components/inputs/NumberEditor';
import { memo, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import ClockButton from '../buttons/ClockButton';

interface VideoStreamSetTimeShiftDialogProperties {
  readonly iconFilled?: boolean | undefined;
  readonly onClose?: () => void;
  readonly value?: VideoStreamDto | undefined;
}

const VideoStreamSetTimeShiftDialog = ({ iconFilled, onClose, value }: VideoStreamSetTimeShiftDialogProperties) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [block, setBlock] = useState<boolean>(false);

  const [timshift, setTimshift] = useState<number>(0);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    if (onClose) {
      onClose();
    }
  };

  const onSetTS = async () => {
    if (value === undefined || value.id === undefined) {
      ReturnToParent();

      return;
    }
    ReturnToParent();
    const toSend = {} as UpdateVideoStreamRequest;

    toSend.id = value.id;
    toSend.timeShift = timshift;

    // await UpdateVideoStream(toSend)
    //   .then(() => {})
    //   .catch(() => {});
  };

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Set Time Shift"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={4}
        show={showOverlay}
      >
        <div className="flex justify-content-center w-full align-items-center h-full">
          <NumberEditor
            label="Time Shift"
            onChange={(e) => {
              setTimshift(e);
            }}
            value={timshift}
          />
          <ClockButton label="Set Time Shift" onClick={async () => await onSetTS()} />
        </div>
      </InfoMessageOverLayDialog>
      <ClockButton disabled={!value} iconFilled={iconFilled} onClick={async () => setShowOverlay(true)} tooltip="Set Time Shift" />
    </>
  );
};

VideoStreamSetTimeShiftDialog.displayName = 'VideoStreamSetTimeShiftDialog';

export default memo(VideoStreamSetTimeShiftDialog);
