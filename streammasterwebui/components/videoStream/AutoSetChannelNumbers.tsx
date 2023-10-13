import {
  type VideoStreamDto,
  type VideoStreamsSetVideoStreamChannelNumbersApiArg,
  type VideoStreamsSetVideoStreamChannelNumbersFromParametersApiArg,
} from '@lib/iptvApi';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedVideoStreams } from '@lib/redux/slices/useSelectedVideoStreams';
import { useSortInfo } from '@lib/redux/slices/useSortInfo';
import { SetVideoStreamChannelNumbers, SetVideoStreamChannelNumbersFromParameters } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import { Checkbox, type CheckboxChangeEvent } from 'primereact/checkbox';
import { InputNumber } from 'primereact/inputnumber';
import React, { useMemo } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import AutoSetButton from '../buttons/AutoSetButton';
import OKButton from '../buttons/OKButton';

type AutoSetChannelNumbersProps = {
  readonly id: string;
};

const AutoSetChannelNumbers = ({ id }: AutoSetChannelNumbersProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [block, setBlock] = React.useState<boolean>(false);

  const [overwriteNumbers, setOverwriteNumbers] = React.useState<boolean>(true);
  const [startNumber, setStartNumber] = React.useState<number>(1);

  // const [videoStreamsSetVideoStreamChannelNumbersFromParametersMutation] = useVideoStreamsSetVideoStreamChannelNumbersFromParametersMutation();
  // const [videoStreamsSetVideoStreamChannelNumbersMutation] = useVideoStreamsSetVideoStreamChannelNumbersMutation();

  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);
  const { sortInfo } = useSortInfo(id);
  const { selectedVideoStreams } = useSelectedVideoStreams(id);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  };

  const ids = useMemo((): string[] => {
    if (selectedVideoStreams !== undefined && selectedVideoStreams.length > 0) {
      const i = selectedVideoStreams?.map((a: VideoStreamDto) => a.id) ?? [];

      return i;
    }

    return [];
  }, [selectedVideoStreams]);

  const onAutoChannelsSave = React.useCallback(async () => {
    setBlock(true);

    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();

        return;
      }

      const toSendAll = {} as VideoStreamsSetVideoStreamChannelNumbersFromParametersApiArg;

      toSendAll.parameters = queryFilter;
      toSendAll.overWriteExisting = overwriteNumbers;
      toSendAll.startNumber = startNumber;

      SetVideoStreamChannelNumbersFromParameters(toSendAll)
        .then(() => {
          setInfoMessage('Set Stream Visibility Successfully');
        })
        .catch((error) => {
          setInfoMessage('Set Stream Visibility Error: ' + error.message);
        });

      return;
    }

    const data = {} as VideoStreamsSetVideoStreamChannelNumbersApiArg;

    data.overWriteExisting = overwriteNumbers;
    data.startNumber = startNumber;
    data.orderBy = sortInfo.orderBy;

    data.ids = [];

    const max = 500;

    let count = 0;

    const promises = [];

    while (count < ids.length) {
      if (count + max < ids.length) {
        data.ids = ids.slice(count, count + max);
      } else {
        data.ids = ids.slice(count, ids.length);
      }

      count += max;

      promises.push(
        SetVideoStreamChannelNumbers(data)
          .then(() => {})
          .catch(() => {}),
      );
    }

    const p = Promise.all(promises);

    await p
      .then(() => {
        setInfoMessage('Auto Set Channels Successful');
      })
      .catch((error) => {
        setInfoMessage('Auto Set Channels Error: ' + error.message);
      });
  }, [ids, overwriteNumbers, queryFilter, selectAll, sortInfo, startNumber]);

  const getTotalCount = useMemo(() => {
    return ids.length;
  }, [ids.length]);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Auto Set Channel Numbers"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={4}
        show={showOverlay}
      >
        <div className="border-1 surface-border flex grid flex-wrap justify-content-center p-0 m-0">
          <div className="flex flex-column mt-2 col-6">
            {`Auto set channel numbers ${overwriteNumbers ? 'and overwrite existing numbers ?' : '?'}`}
            <span className="scalein animation-duration-500 animation-iteration-2 text-bold text-red-500 font-italic mt-2">This will auto save</span>
          </div>
          <div className=" flex mt-2 col-6 align-items-center justify-content-start p-0 m-0">
            <span>
              <div className="flex col-12 justify-content-center align-items-center p-0 m-0  w-full ">
                <div className="flex col-2 justify-content-center align-items-center p-0 m-0">
                  <Checkbox checked={overwriteNumbers} id="overwriteNumbers" onChange={(e: CheckboxChangeEvent) => setOverwriteNumbers(e.checked ?? false)} />
                </div>
                <span className="flex col-10 text-xs">Overwrite Existing</span>
              </div>
              <div className="flex col-12 justify-content-center align-items-center p-0 m-0">
                <div className="flex col-6 justify-content-end align-items-center p-0 m-0">
                  <span className="text-xs pl-4">Ch. #</span>
                </div>
                <div className="flex col-6 pl-1 justify-content-start align-items-center p-0 m-0 w-full">
                  <InputNumber
                    className="withpadding"
                    id="startNumber"
                    max={999999}
                    min={0}
                    onChange={(e) => e.value && setStartNumber(e.value)}
                    showButtons
                    size={3}
                    value={startNumber}
                  />
                </div>
              </div>
            </span>
          </div>
          <div className="flex col-12 gap-2 mt-4 justify-content-center ">
            <OKButton onClick={async () => await onAutoChannelsSave()} />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <AutoSetButton disabled={getTotalCount === 0 && !selectAll} onClick={() => setShowOverlay(true)} tooltip="Auto Set Channels" />
    </>
  );
};

AutoSetChannelNumbers.displayName = 'Auto Set Channel Numbers';
export default React.memo(AutoSetChannelNumbers);
