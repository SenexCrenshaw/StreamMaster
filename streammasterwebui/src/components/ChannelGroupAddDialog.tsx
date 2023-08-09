import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";
import { getTopToolOptions } from "../common/common";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import DataSelector from "../features/dataSelector/DataSelector";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";

const ChannelGroupAddDialog = (props: ChannelGroupAddDialogProps) => {

  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');

  const [newGroupName, setNewGroupName] = React.useState('');
  const [regex, setRegex] = React.useState<string>('');

  const [channelGroupsCreateChannelGroupMutation] = StreamMasterApi.useChannelGroupsCreateChannelGroupMutation();

  const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamsByNamePatternQuery(regex ?? '');

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    setRegex('');
    setNewGroupName('');
    props.onHide?.(newGroupName);
  }, [newGroupName, props]);

  const addGroup = React.useCallback(() => {
    setBlock(true);
    if (!newGroupName) {
      ReturnToParent();
      return;
    }

    const data = {} as StreamMasterApi.CreateChannelGroupRequest;
    data.groupName = newGroupName;

    if (regex !== undefined && regex !== '') {
      data.regex = regex;
    }

    channelGroupsCreateChannelGroupMutation(data).then(() => {
      setInfoMessage('Channel Group Added Successfully');
    }).catch((e) => {
      setInfoMessage('Channel Group Add Error: ' + e.message);
    });

  }, [ReturnToParent, channelGroupsCreateChannelGroupMutation, newGroupName, regex]);

  React.useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if (event.code === 'Enter' || event.code === 'NumpadEnter') {
        event.preventDefault();
        if (newGroupName !== "") {
          addGroup();
        }
      }

    };

    document.addEventListener('keydown', callback);
    return () => {
      document.removeEventListener('keydown', callback);
    };
  }, [addGroup, newGroupName]);

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [

      { field: 'user_Tvg_name', header: 'Name' },
    ]
  }, []);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header='Add Group'
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <InputText
              autoFocus
              className="withpadding p-inputtext-sm w-full"
              onChange={(e) => setNewGroupName(e.target.value)}
              placeholder="Group Name"
              value={newGroupName}
            />
            <InputText
              className="withpadding p-inputtext-sm w-full mt-2"
              onChange={(e) => setRegex(e.target.value)}
              placeholder="Group Regex"
              value={regex}
            />
            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <Button
                icon="pi pi-times "
                label="Cancel"
                onClick={(() => ReturnToParent())}
                rounded
                severity="warning"
              />
              <Button
                icon="pi pi-check"
                label="Add"
                onClick={addGroup}
                rounded
                severity="success"
              />
            </div>
            <div hidden={regex === undefined || regex === ''}>
              <div className='m3uFilesEditor flex flex-column col-12 flex-shrink-0 '>
                <DataSelector
                  columns={sourceColumns}
                  dataSource={videoStreamsQuery.data}
                  emptyMessage="No Streams"
                  globalSearchEnabled={false}
                  id='StreamingServerStatusPanel'
                  isLoading={videoStreamsQuery.isLoading}
                  showHeaders={false}
                  style={{ height: 'calc(50vh - 40px)' }}
                />
              </div>
            </div>
          </div>
        </div >
      </InfoMessageOverLayDialog >

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
        tooltip="Add Group"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
};


ChannelGroupAddDialog.displayName = 'Play List Editor';
ChannelGroupAddDialog.defaultProps = {

};

export type ChannelGroupAddDialogProps = {
  onHide?: (value: string) => void;
};

export default React.memo(ChannelGroupAddDialog);
