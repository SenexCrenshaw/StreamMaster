import React from "react";
import { Button } from "primereact/button";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import { InputText } from "primereact/inputtext";
import * as StreamMasterApi from '../../store/iptvApi';
import { AddStreamGroup } from "../../store/signlar_functions";
import PlayListDataSelector from "../dataSelectors/PlayListDataSelector";
import { Accordion, AccordionTab } from 'primereact/accordion';
import { getTopToolOptions } from "../../common/common";

const StreamGroupAddDialog = (props: StreamGroupAddDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [name, setName] = React.useState<string>('');
  const [streamGroupNumber, setStreamGroupNumber] = React.useState<number>();
  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[]);

  const streamGroupsQuery = StreamMasterApi.useStreamGroupsGetStreamGroupsQuery({} as StreamMasterApi.StreamGroupsGetStreamGroupsApiArg);

  const getNextStreamGroupNumber = React.useCallback((): number => {
    if (!streamGroupsQuery?.data?.data) {
      return 0;
    }

    if (streamGroupsQuery.data.data.length === 0) {
      return 1;
    }

    const numbers = streamGroupsQuery.data.data.map((x) => x.streamGroupNumber);

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

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setName('');
    setStreamGroupNumber(0);
    setBlock(false);
    props.onHide?.();
  }, [props]);


  const isSaveEnabled = React.useMemo((): boolean => {

    if (name && name !== '') {
      return true;
    }

    if (streamGroupNumber !== undefined && streamGroupNumber !== 0) {
      return true;
    }

    return false;

  }, [name, streamGroupNumber]);


  const onAdd = React.useCallback(() => {

    setBlock(true);
    if (!isSaveEnabled || !name || streamGroupNumber === 0 || name === '') {
      ReturnToParent();
      return;
    }


    if (!isSaveEnabled || !streamGroupNumber || streamGroupNumber === 0) {
      return;
    }

    const data = {} as StreamMasterApi.AddStreamGroupRequest;

    data.name = name;
    data.streamGroupNumber = streamGroupNumber;

    if (selectedChannelGroups.length > 0) {
      data.channelGroupNames = selectedChannelGroups.map((x) => x.name);
    }

    AddStreamGroup(data)
      .then(() => {

        setInfoMessage('Stream Group Added Successfully');

      }).catch((e) => {
        setInfoMessage('Stream Group Add Error: ' + e.message);
      });
  }, [ReturnToParent, isSaveEnabled, name, selectedChannelGroups, streamGroupNumber]);

  React.useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if (event.code === 'Enter' || event.code === 'NumpadEnter') {
        event.preventDefault();
        if (name !== "") {
          onAdd();
        }
      }

    };

    document.addEventListener('keydown', callback);
    return () => {
      document.removeEventListener('keydown', callback);
    };
  }, [onAdd, name]);

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
        header='Add Stream Group'
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


          </div>

          <Accordion className='mt-2'>
            <AccordionTab header="Groups">
              <div className='col-12 m-0 p-0 pr-1' >
                <PlayListDataSelector
                  id='streamggroupadddialog'
                  maxHeight={400}
                  onSelectionChange={(e) => onsetSelectedChannelGroups(e as StreamMasterApi.ChannelGroupDto[])}
                />
              </div>
            </AccordionTab>

          </Accordion>

          <div className="flex col-12 mt-3 gap-2 justify-content-end">
            <Button
              disabled={!isSaveEnabled}
              icon="pi pi-check"
              label="Add"
              onClick={onAdd}
              rounded
              severity="success"
            />
          </div>

        </div>
      </InfoMessageOverLayDialog>

      <Button
        icon="pi pi-plus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="success"
        size="small"
        style={{
          ...{
            maxHeight: "2rem",
            maxWidth: "2rem"
          }
        }}
        tooltip="Add Stream Group"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

StreamGroupAddDialog.displayName = 'StreamGroupAddDialog';
StreamGroupAddDialog.defaultProps = {
}

type StreamGroupAddDialogProps = {
  onHide?: () => void;
};

export default React.memo(StreamGroupAddDialog);
