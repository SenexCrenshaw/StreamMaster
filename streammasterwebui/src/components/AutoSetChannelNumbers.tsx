
import { type CheckboxChangeEvent } from "primereact/checkbox";
import { Checkbox } from "primereact/checkbox";
import { InputNumber } from "primereact/inputnumber";
import React, { useMemo } from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import { SetVideoStreamChannelNumbers } from "../store/signlar_functions";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import OKButton from "./buttons/OKButton";
import AutoSetButton from "./buttons/AutoSetButton";

const AutoSetChannelNumbers = (props: AutoSetChannelNumbersProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [block, setBlock] = React.useState<boolean>(false);

  const [overwriteNumbers, setOverwriteNumbers] = React.useState<boolean>(true);
  const [startNumber, setStartNumber] = React.useState<number>(1);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  };


  const getNextNumber = React.useCallback((sn: number, nums: number[]): number => {
    if (nums.length === 0) return sn;

    const max = Math.max(...nums);

    if (sn === max) return sn + 1;

    while (nums.includes(sn)) {
      ++sn;
    }

    return sn;
  }, []);

  const onAutoChannelsSave = React.useCallback(async () => {
    setBlock(true);
    let sn = overwriteNumbers ? startNumber - 1 : startNumber;

    const nums = [...new Set(props.ids.map((item: StreamMasterApi.ChannelNumberPair) => item.channelNumber))] as number[];

    const newChannels = props.ids.map((cp: StreamMasterApi.ChannelNumberPair) => {
      if (!overwriteNumbers && cp.channelNumber !== 0) {
        return {
          channelNumber: cp.channelNumber,
          id: cp.id,
        } as StreamMasterApi.ChannelNumberPair;
      }

      if (!overwriteNumbers) {
        sn = getNextNumber(sn, nums);
      }
      else {
        sn++;
      }

      nums.push(sn);

      return {
        channelNumber: sn,
        id: cp.id,
      } as StreamMasterApi.ChannelNumberPair;

    });

    const data = {} as StreamMasterApi.SetVideoStreamChannelNumbersRequest;
    data.channelNumberPairs = [];

    const max = 500;

    let count = 0;


    const promises = [];

    while (count < newChannels.length) {
      if (count + max < newChannels.length) {
        data.channelNumberPairs = newChannels.slice(count, count + max);
      } else {
        data.channelNumberPairs = newChannels.slice(count, newChannels.length);
      }

      count += max;

      promises.push(
        SetVideoStreamChannelNumbers(data)
          .then(() => {

          }).catch(() => { })
      );

    }

    const p = Promise.all(promises);

    await p.then(() => {

      setInfoMessage('Auto Set Channels Successful');

    }).catch((error) => {
      setInfoMessage('Auto Set Channels Error: ' + error.message);
    });


  }, [getNextNumber, overwriteNumbers, props, startNumber]);

  const getTotalCount = useMemo(() => {
    if (props.overrideTotalRecords !== undefined) {
      return props.overrideTotalRecords;
    }

    return props.ids.length;

  }, [props.overrideTotalRecords, props.ids.length]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header="Auto Set Channel Numbers"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={4}
        show={showOverlay}
      >
        <div className="border-1 surface-border flex grid flex-wrap justify-content-center p-0 m-0">
          <div className='flex flex-column mt-2 col-6'>
            {`Auto set (${getTotalCount}) channel numbers ${overwriteNumbers ? 'and overwrite existing numbers ?' : '?'}`}
            <span className="scalein animation-duration-500 animation-iteration-2 text-bold text-red-500 font-italic mt-2">
              This will auto save
            </span>
          </div>
          <div className=" flex mt-2 col-6 align-items-center justify-content-start p-0 m-0">
            <span >
              <div className="flex col-12 justify-content-center align-items-center p-0 m-0  w-full ">
                <div className='flex col-2 justify-content-center align-items-center p-0 m-0'>
                  <Checkbox
                    checked={overwriteNumbers}
                    id="overwriteNumbers"
                    onChange={(e: CheckboxChangeEvent) =>
                      setOverwriteNumbers(e.checked ?? false)
                    }
                  />
                </div>
                <span className="flex col-10 text-xs">
                  Overwrite Existing
                </span>
              </div>
              <div className="flex col-12 justify-content-center align-items-center p-0 m-0">
                <div className="flex col-6 justify-content-end align-items-center p-0 m-0">
                  <span className="text-xs pl-4" >
                    Ch. #
                  </span>
                </div>
                <div className='flex col-6 pl-1 justify-content-start align-items-center p-0 m-0 w-full'>
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

      <AutoSetButton disabled={getTotalCount === 0} onClick={() => setShowOverlay(true)} tooltip={`Auto Set (${getTotalCount}) Channels`} />

    </>
  )
}

AutoSetChannelNumbers.displayName = 'Auto Set Channel Numbers';
AutoSetChannelNumbers.defaultProps = {
};

export type AutoSetChannelNumbersProps = {
  ids: StreamMasterApi.ChannelNumberPair[];
  overrideTotalRecords?: number | undefined;
};

export default React.memo(AutoSetChannelNumbers);
