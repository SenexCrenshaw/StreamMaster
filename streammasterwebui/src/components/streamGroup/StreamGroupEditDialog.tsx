import { InputText } from "primereact/inputtext";
import { useState, useEffect, useCallback, useMemo, memo } from "react";
import {type StreamGroupDto, type UpdateStreamGroupRequest } from "../../store/iptvApi";
import { UpdateStreamGroup } from "../../store/signlar_functions";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";

import EditButton from "../buttons/EditButton";
import { useSelectedStreamGroup } from "../../app/slices/useSelectedStreamGroup";


type StreamGroupEditDialogProps = {
  readonly id: string;
  readonly onHide?: (value: StreamGroupDto | undefined) => void;
};

const StreamGroupEditDialog = (props: StreamGroupEditDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [name, setName] = useState<string>('');

  const { selectedStreamGroup } = useSelectedStreamGroup(props.id);

  useEffect(() => {
    if (selectedStreamGroup === undefined) {
      return;
    }

    if (selectedStreamGroup.name !== undefined) {
      setName(selectedStreamGroup.name);
    }


  }, [selectedStreamGroup]);


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

    if (!isSaveEnabled || !name || name === '' || selectedStreamGroup === undefined || selectedStreamGroup.id === undefined) {
      ReturnToParent();

      return;
    }

    if (!isSaveEnabled) {
      return;
    }

    const data = {} as UpdateStreamGroupRequest;
    data.name = name;
    data.streamGroupId = selectedStreamGroup.id;

    UpdateStreamGroup(data)
      .then(() => {
        setInfoMessage('Stream Group Edit Successfully');
      }).catch((e) => {
        setInfoMessage('Stream Group Edit Error: ' + e.message);
      });
  }, [ReturnToParent, isSaveEnabled, name, selectedStreamGroup]);

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



  // const onsetSelectedChannelGroups = useCallback((selectedData: ChannelGroupDto | ChannelGroupDto[]) => {
  //   if (Array.isArray(selectedData)) {
  //     setSelectedChannelGroups(selectedData);
  //   } else {
  //     setSelectedChannelGroups([selectedData]);
  //   }

  // }, []);


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

          {/* <Accordion className='mt-2'>
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
          </Accordion> */}

          <div className="flex col-12 mt-3 gap-2 justify-content-end">
            <EditButton label='Edit Stream Group' onClick={() => onUpdate()} tooltip='Edit Stream Group' />
          </div>

        </div>
      </InfoMessageOverLayDialog>

      <EditButton
        disabled={selectedStreamGroup === undefined || selectedStreamGroup.streamGroupNumber === undefined || selectedStreamGroup.streamGroupNumber === 0}
        iconFilled
        label='Edit Stream Group'
        onClick={() => setShowOverlay(true)}
        tooltip='Edit Stream Group' />

    </>
  );
}

StreamGroupEditDialog.displayName = 'StreamGroupEditDialog';
StreamGroupEditDialog.defaultProps = {
}

export default memo(StreamGroupEditDialog);
