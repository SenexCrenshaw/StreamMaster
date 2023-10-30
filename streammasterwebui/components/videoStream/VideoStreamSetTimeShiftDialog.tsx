import { formatToFourDigits } from '@lib/common/common';
import { type UpdateVideoStreamRequest, type VideoStreamDto } from '@lib/iptvApi';
import { UpdateVideoStream } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import { memo, useEffect, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import ClockButton from '../buttons/ClockButton';
import NumberInput from '../inputs/NumberInput';

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

  useEffect(() => {
    if (value?.timeShift !== undefined && value.timeShift.length === 4) {
      const digit = value.timeShift.charAt(1);
      setTimshift(Number(digit));
    }
  }, [value]);

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
    toSend.timeShift = formatToFourDigits(timshift);

    await UpdateVideoStream(toSend)
      .then(() => {})
      .catch(() => {});
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
          <NumberInput
            label="Time Shift"
            onChange={(e) => {
              setTimshift(e);
            }}
            showClear
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
