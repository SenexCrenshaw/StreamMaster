import { Accordion, AccordionTab } from "primereact/accordion";
import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";
import { useState, useEffect, useCallback, useMemo, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type ChannelGroupDto, type StreamGroupDto, type UpdateStreamGroupRequest } from "../../store/iptvApi";
import { UpdateStreamGroup } from "../../store/signlar_functions";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import PlayListDataSelector from "../../features/playListEditor/PlayListDataSelector";


const StreamGroupEditDialog = (props: StreamGroupEditDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [name, setName] = useState<string>('');
  const [selectedChannelGroups, setSelectedChannelGroups] = useState<ChannelGroupDto[]>([] as ChannelGroupDto[]);

  useEffect(() => {
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


  const ReturnToParent = useCallback((retData?: StreamGroupDto) => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onHide?.(retData);
  }, [props]);

  const isSaveEnabled = useMemo((): boolean => {

    if (name && name !== '') {
      return true;
    }

    return false;

  }, [name]);


  const onUpdate = useCallback(() => {

    setBlock(true);

    if (!isSaveEnabled || !name || name === '' || props.value === undefined || props.value.id === undefined) {
      ReturnToParent();

      return;
    }

    if (!isSaveEnabled) {
      return;
    }

    const data = {} as UpdateStreamGroupRequest;

    data.name = name;

    data.streamGroupId = props.value.id;

    if (selectedChannelGroups.length > 0) {
      data.channelGroupNames = selectedChannelGroups.map((x) => x.name);
    } else {
      data.channelGroupNames = [];
    }

    UpdateStreamGroup(data)
      .then(() => {

        setInfoMessage('Stream Group Edit Successfully');

      }).catch((e) => {
        setInfoMessage('Stream Group Edit Error: ' + e.message);
      });
  }, [ReturnToParent, isSaveEnabled, name, props.value, selectedChannelGroups]);

  useEffect(() => {
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



  const onsetSelectedChannelGroups = useCallback((selectedData: ChannelGroupDto | ChannelGroupDto[]) => {
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
                  onSelectionChange={(e) => onsetSelectedChannelGroups(e as ChannelGroupDto[])}

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
  onHide?: (value: StreamGroupDto | undefined) => void;
  value: StreamGroupDto | undefined;
};

export default memo(StreamGroupEditDialog);
