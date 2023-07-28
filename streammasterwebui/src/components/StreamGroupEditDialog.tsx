import React from "react";
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { InputText } from "primereact/inputtext";
import type * as StreamMasterApi from '../store/iptvApi';
import { UpdateStreamGroup } from "../store/signlar_functions";
import { Accordion, AccordionTab } from "primereact/accordion";
import PlayListDataSelector from "./PlayListDataSelector";

const StreamGroupEditDialog = (props: StreamGroupEditDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [name, setName] = React.useState<string>('');
  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[]);

  React.useMemo(() => {
    if (props.value === undefined) {
      return;
    }

    if (props.value.name !== undefined) {
      setName(props.value.name);
    }

    if (props.value.channelGroups !== undefined) {
      setSelectedChannelGroups(props.value.channelGroups);
    }

  }, [props.value]);


  const ReturnToParent = React.useCallback((retData?: StreamMasterApi.StreamGroupDto) => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onHide?.(retData);
  }, [props]);

  const isSaveEnabled = React.useMemo((): boolean => {

    if (name && name !== '') {
      return true;
    }

    return false;

  }, [name]);


  const onUpdate = React.useCallback(() => {

    setBlock(true);
    if (!isSaveEnabled || !name || name === '' || props.value === undefined || props.value.id === undefined) {
      ReturnToParent();
      return;
    }

    if (!isSaveEnabled) {
      return;
    }

    const data = {} as StreamMasterApi.UpdateStreamGroupRequest;

    data.name = name;

    data.streamGroupId = props.value.id;

    if (selectedChannelGroups.length > 0) {
      data.channelGroupNames = selectedChannelGroups.map((x) => x.name);
    }

    UpdateStreamGroup(data)
      .then(() => {

        setInfoMessage('Stream Group Edit Successfully');

      }).catch((e) => {
        setInfoMessage('Stream Group Edit Error: ' + e.message);
      });
  }, [ReturnToParent, isSaveEnabled, name, props.value, selectedChannelGroups]);

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
        closable
        header='Edit Stream Group'
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={4}
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

                />
              </div>
            </AccordionTab>
          </Accordion>

          <div className="flex col-12 mt-3 gap-2 justify-content-end">
            <Button
              disabled={!isSaveEnabled}
              icon="pi pi-check"
              label="Save"
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
