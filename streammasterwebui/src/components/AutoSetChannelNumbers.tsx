/* eslint-disable react/no-unused-prop-types */
import { Button } from "primereact/button";
import { type CheckboxChangeEvent } from "primereact/checkbox";
import { Checkbox } from "primereact/checkbox";
import { InputNumber } from "primereact/inputnumber";
import React from "react";
// import StreamMasterSetting from "../store/signlar/StreamMasterSetting";
import type * as StreamMasterApi from '../store/iptvApi';
import { SetVideoStreamChannelNumbers } from "../store/signlar_functions";
import { getTopToolOptions } from "../common/common";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";

const AutoSetChannelNumbers = (props: AutoSetChannelNumbersProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [block, setBlock] = React.useState<boolean>(false);

  const [overwriteNumbers, setOverwriteNumbers] = React.useState<boolean>(true);
  const [startNumber, setStartNumber] = React.useState<number>(1);
  // const setting = StreamMasterSetting();

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  };

  // React.useMemo(() => {
  //   if (setting.data.firstFreeNumber) {
  //     setStartNumber(setting.data.firstFreeNumber);
  //   }
  // }, [setting.data.firstFreeNumber]);

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

    const ret = [] as StreamMasterApi.ChannelNumberPair[];
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
          .then((returnData) => {
            ret.push(...returnData);
          }).catch(() => { })
      );

    }

    const p = Promise.all(promises);

    await p.then(() => {
      if (ret.length === 0) {
        setInfoMessage('Auto Set Channels No Changes');
      } else {
        setInfoMessage('Auto Set Channels Successful');
      }

      props.onChange?.(ret);
    }).catch((error) => {
      setInfoMessage('Auto Set Channels Error: ' + error.message);
    });


  }, [getNextNumber, overwriteNumbers, props, startNumber]);


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
            {`Auto set (${props.ids.length}) channel numbers ${overwriteNumbers ? 'and overwrite existing numbers ?' : '?'}`}
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
            <Button
              icon="pi pi-times "
              label="Cancel"
              onClick={() => ReturnToParent()}
              rounded
              severity="warning"
              size="small"
            />
            <Button
              icon="pi pi-check"
              label="Set & Save"
              onClick={async () => await onAutoChannelsSave()}
              rounded
              severity="success"
              size="small"
            />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <Button
        disabled={props.ids === null || props.ids.length === 0}
        icon="pi pi-sort-numeric-up-alt"
        onClick={() => setShowOverlay(true)}
        rounded
        size="small"
        tooltip={`Auto Set (${props.ids.length}) Channels`}
        tooltipOptions={getTopToolOptions}
      />

    </>
  )
}

AutoSetChannelNumbers.displayName = 'Auto Set Channel Numbers';
AutoSetChannelNumbers.defaultProps = {
};

export type AutoSetChannelNumbersProps = {
  ids: StreamMasterApi.ChannelNumberPair[];
  onChange?: ((value: StreamMasterApi.ChannelNumberPair[]) => void) | null;
};

export default React.memo(AutoSetChannelNumbers);
