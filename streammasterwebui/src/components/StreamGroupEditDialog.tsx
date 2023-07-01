/* eslint-disable @typescript-eslint/no-unused-vars */
import React from "react";
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { InputText } from "primereact/inputtext";
import { InputNumber } from "primereact/inputnumber";
import * as StreamMasterApi from '../store/iptvApi';
import { UpdateStreamGroup } from "../store/signlar_functions";
import { Accordion, AccordionTab } from "primereact/accordion";
import PlayListDataSelector from "./PlayListDataSelector";

const StreamGroupEditDialog = (props: StreamGroupEditDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [name, setName] = React.useState<string>('');
  const [streamGroupNumber, setStreamGroupNumber] = React.useState<number>();
  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[]);
  const streamGroupsQuery = StreamMasterApi.useStreamGroupsGetStreamGroupsQuery();

  React.useMemo(() => {
    if (props.value === undefined) {
      return;
    }

    if (props.value.name !== undefined) {
      setName(props.value.name);
    }

    if (props.value.streamGroupNumber !== undefined) {
      setStreamGroupNumber(props.value.streamGroupNumber);
    }

    if (props.value.channelGroups !== undefined) {
      setSelectedChannelGroups(props.value.channelGroups);
    }

  }, [props.value]);

  const getNextStreamGroupNumber = React.useCallback((): number => {
    if (!streamGroupsQuery?.data) {
      return 0;
    }

    if (streamGroupsQuery.data.length === 0) {
      return 1;
    }

    const numbers = streamGroupsQuery.data.map((x) => x.streamGroupNumber);

    const [min, max] = [1, Math.max(...numbers)];

    if (max < min) {
      return min;
    }

    const out = Array.from(Array(max - min), (v, i) => i + min).filter(
      (i) => !numbers.includes(i),
    );

    if (out.length > 0) {
      return out[0];
    }

    return max + 1;
  }, [streamGroupsQuery.data]);

  React.useEffect(() => {
    if (streamGroupNumber === undefined || streamGroupNumber === 0) {
      setStreamGroupNumber(getNextStreamGroupNumber());
    }
  }, [getNextStreamGroupNumber, streamGroupNumber]);

  const ReturnToParent = React.useCallback((retData?: StreamMasterApi.StreamGroupDto) => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onHide?.(retData);
  }, [props]);

  const doesStreamGroupNumberExist = (
    sgNumber: number | undefined,
  ): boolean => {
    if (!sgNumber || !streamGroupsQuery || !streamGroupsQuery.data) return false;

    if (streamGroupsQuery.data.map((x) => x.streamGroupNumber).includes(sgNumber)) {
      return true;
    }

    return false;
  };

  const onChannelStreamGroupNumberChange = (e: number) => {
    if (e && e > 0 && e < 1000000) {
      setStreamGroupNumber(e);
    }
  };

  const isSaveEnabled = React.useMemo((): boolean => {

    if (name && name !== '') {
      return true;
    }

    if (streamGroupNumber !== undefined && streamGroupNumber !== 0) {
      return true;
    }

    return false;

  }, [name, streamGroupNumber]);


  const onUpdate = React.useCallback(() => {

    setBlock(true);
    if (!isSaveEnabled || !name || streamGroupNumber === 0 || name === '' || props.value === undefined || props.value.id === undefined) {
      ReturnToParent();
      return;
    }


    if (!isSaveEnabled || !streamGroupNumber || streamGroupNumber === 0) {
      return;
    }

    const data = {} as StreamMasterApi.UpdateStreamGroupRequest;

    data.name = name;

    data.streamGroupId = props.value.id;
    data.streamGroupNumber = streamGroupNumber;

    data.channelGroupNames = selectedChannelGroups.map((x) => x.name);

    UpdateStreamGroup(data)
      .then((result) => {
        if (result) {
          setInfoMessage('Stream Group Edit Successfully');
          ReturnToParent(result);
        } else {
          setInfoMessage('Stream Group Edit No Changes');
        }
      }).catch((e) => {
        setInfoMessage('Stream Group Edit Error: ' + e.message);
      });
  }, [ReturnToParent, isSaveEnabled, name, props.value, selectedChannelGroups, streamGroupNumber]);

  React.useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if (event.code === 'Enter' || event.code === 'NumpadEnter') {
        event.preventDefault();
        if (name !== "") {
          onUpdate();
        }
      }

    };

    document.addEventListener('keydown', callback);
    return () => {
      document.removeEventListener('keydown', callback);
    };
  }, [onUpdate, name]);

  const onsetSelectedChannelGroups = React.useCallback((selectedData: StreamMasterApi.ChannelGroupDto | StreamMasterApi.ChannelGroupDto[]) => {
    if (Array.isArray(selectedData)) {
      setSelectedChannelGroups(selectedData);
    } else {
      setSelectedChannelGroups([selectedData]);
    }

  }, []);


  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header='Edit Stream Group'
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={6}
        show={showOverlay}
      >

        <div className="justify-content-between align-items-center ">
          <div className="flex">
            <span className="p-float-label col-6">
              <InputText
                autoFocus
                className={name === '' ? 'withpadding p-invalid' : 'withpadding'}
                id="Name"
                onChange={(e) => setName(e.target.value)}
                type="text"
                value={name}
              />
              <label
                className="text-500"
                htmlFor="Name"
              >Name</label>
            </span>

            <div className="flex col-6 justify-content-end align-items-center p-0 m-0">
              {/* Ch #*/}
              <div className="flex flex-wrap col-12 p-0 m-0 text-xs">
                <div className="flex col-12 justify-content-end align-items-center p-0 m-0">
                  <span className="text-xs"                >
                    {doesStreamGroupNumberExist(streamGroupNumber) ? 'Stream Group Number Exists!' : 'Stream Group Number'}
                  </span>
                </div>
                <div className='flex col-12 justify-content-end align-items-center p-0 m-0'>
                  <InputNumber
                    className='withpadding p-0 m-0'
                    id="channelNumber"
                    max={999999}
                    min={0}
                    onChange={(e) => { onChannelStreamGroupNumberChange(e.value ?? 0) }}
                    showButtons
                    size={3}

                    value={streamGroupNumber}
                  />
                </div>
              </div>
            </div>

          </div>

          <Accordion className='mt-2'>
            <AccordionTab header="Groups">
              <div className='col-12 m-0 p-0 pr-1' >
                <PlayListDataSelector
                  hideControls
                  id='streamggroupeditdialog'
                  maxHeight={400}
                  name="Groups"
                  onSelectionChange={(e) => onsetSelectedChannelGroups(e as StreamMasterApi.ChannelGroupDto[])}
                  selectChannelGroups={selectedChannelGroups}
                />
              </div>
            </AccordionTab>
          </Accordion>

          <div className="flex col-12 mt-3 gap-2 justify-content-end">
            <Button
              icon="pi pi-times "
              label="Cancel"
              onClick={(() => ReturnToParent())}
              rounded
              severity="warning"
            />
            <Button
              disabled={!isSaveEnabled}
              icon="pi pi-check"
              label="Edit"
              onClick={onUpdate}
              rounded
              severity="success"
            />
          </div>

        </div>
      </InfoMessageOverLayDialog>
      <Button
        disabled={props.value === undefined || props.value.streamGroupNumber === undefined || props.value.streamGroupNumber === 0}
        icon="pi pi-pencil"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="warning"
        size="small"
        style={{
          ...{
            maxHeight: "2rem",
            maxWidth: "2rem"
          }
        }}
        tooltip="Edit Stream Group"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

StreamGroupEditDialog.displayName = 'StreamGroupEditDialog';
StreamGroupEditDialog.defaultProps = {
}

type StreamGroupEditDialogProps = {
  onHide?: (value: StreamMasterApi.StreamGroupDto | undefined) => void;
  value: StreamMasterApi.StreamGroupDto | undefined;
};

export default React.memo(StreamGroupEditDialog);
