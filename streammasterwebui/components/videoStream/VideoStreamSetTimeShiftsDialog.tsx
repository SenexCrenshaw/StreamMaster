import { formatToFourDigits } from '@lib/common/common';
import { VideoStreamDto, VideoStreamsSetVideoStreamTimeShiftFromParametersApiArg, VideoStreamsSetVideoStreamTimeShiftsApiArg } from '@lib/iptvApi';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedVideoStreams } from '@lib/redux/slices/useSelectedVideoStreams';
import { SetVideoStreamTimeShiftFromParameters, SetVideoStreamTimeShifts } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import { memo, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import ClockButton from '../buttons/ClockButton';
import NumberInput from '../inputs/NumberInput';

type VideoStreamSetTimeShiftsDialogProps = {
  readonly id: string;
};

const VideoStreamSetTimeShiftsDialog = ({ id }: VideoStreamSetTimeShiftsDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [block, setBlock] = useState<boolean>(false);

  const [timshift, setTimshift] = useState<number>(0);

  const { selectedVideoStreams } = useSelectedVideoStreams(id);
  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  };

  const onSetTS = async () => {
    setBlock(true);

    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();

        return;
      }

      const toSendAll = {} as VideoStreamsSetVideoStreamTimeShiftFromParametersApiArg;

      toSendAll.parameters = queryFilter;
      toSendAll.timeShift = formatToFourDigits(timshift);

      SetVideoStreamTimeShiftFromParameters(toSendAll)
        .then(() => {
          setInfoMessage('Set Streams Successfully');
        })
        .catch((error) => {
          setInfoMessage('Set Streams Error: ' + error.message);
        });

      return;
    }

    const ids = [...new Set(selectedVideoStreams.map((item: VideoStreamDto) => item.id))] as string[];

    const toSend = {} as VideoStreamsSetVideoStreamTimeShiftsApiArg;

    toSend.ids = ids;

    const max = 500;

    let count = 0;

    const promises = [];

    while (count < ids.length) {
      if (count + max < ids.length) {
        toSend.ids = ids.slice(count, count + max);
      } else {
        toSend.ids = ids.slice(count, ids.length);
      }

      count += max;
      promises.push(
        SetVideoStreamTimeShifts(toSend)
          .then(() => {})
          .catch(() => {}),
      );
    }

    const p = Promise.all(promises);

    await p
      .then(() => {
        setInfoMessage('Successful');
      })
      .catch((error) => {
        setInfoMessage('Error: ' + error.message);
      });
  };

  const getTotalCount = () => {
    return selectedVideoStreams.length;
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
      <ClockButton disabled={getTotalCount() === 0 && !selectAll} onClick={async () => setShowOverlay(true)} tooltip="Set Time Shift" />
    </>
  );
};

VideoStreamSetTimeShiftsDialog.displayName = 'VideoStreamSetTimeShiftsDialog';

export default memo(VideoStreamSetTimeShiftsDialog);
