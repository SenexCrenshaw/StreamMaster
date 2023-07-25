
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import { Button } from "primereact/button";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { getTopToolOptions } from "../common/common";
import { InputText } from "primereact/inputtext";
import DataSelector from "../features/dataSelector/DataSelector";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";

const ChannelGroupEditDialog = (props: ChannelGroupEditDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [regex, setRegex] = React.useState<string | undefined>('');
  const [newGroupName, setNewGroupName] = React.useState('');

  const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamsQuery();

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    setRegex('');
    props.onClose?.();
  }, [props]);

  React.useEffect(() => {

    if (props.value) {
      setNewGroupName(props.value.name);
      if (props.value.regexMatch !== null)
        setRegex(props.value.regexMatch);
    }

  }, [props.value]);

  const changeGroupName = () => {
    setBlock(true);
    if (!newGroupName || !props.value) {
      ReturnToParent();
      return;
    }

    const tosend = {} as StreamMasterApi.UpdateChannelGroupRequest;

    tosend.groupName = props.value?.name;
    tosend.newGroupName = newGroupName;

    if (regex !== undefined && regex !== '') {
      tosend.regex = regex;
    }

    Hub.UpdateChannelGroup(tosend).then(() => {
      setInfoMessage('Channel Group Edit Successfully');
    }).catch((e) => {
      setInfoMessage('Channel Group Edit Error: ' + e.message);
    });

  }

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [

      { field: 'user_Tvg_name', header: 'Name' },
    ]
  }, []);

  const dataSource = React.useMemo((): StreamMasterApi.VideoStreamDto[] | undefined => {
    if (regex === undefined || regex === '' || !videoStreamsQuery.data)
      return (undefined);


    const filteredData = videoStreamsQuery.data.filter((item) => {
      if (item.isHidden)
        return false;

      const regexToTest = new RegExp(`.*${regex}.*`, 'i');
      return regexToTest.test(item.user_Tvg_name);
    });

    return filteredData;
  }, [regex, videoStreamsQuery.data]);



  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Edit Group"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3>Edit Group</h3>
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
                label="Ok"
                onClick={changeGroupName}
                rounded
                severity="success"
              />
            </div>
            <div hidden={regex === undefined || regex === ''}>
              <div className='m3uFilesEditor flex flex-column col-12 flex-shrink-0 '>
                <DataSelector
                  columns={sourceColumns}
                  dataSource={dataSource}
                  emptyMessage="No Streams"
                  globalSearchEnabled={false}
                  id='StreamingServerStatusPanel'
                  showHeaders={false}
                  style={{ height: 'calc(50vh - 40px)' }}
                />
              </div>
            </div>
          </div>
        </div >
      </InfoMessageOverLayDialog>

      <Button
        icon="pi pi-pencil"
        onClick={() => setShowOverlay(true)}
        rounded
        size="small"
        text
        tooltip="Edit Group"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

ChannelGroupEditDialog.displayName = 'ChannelGroupEditDialog';
ChannelGroupEditDialog.defaultProps = {
  value: null,

};

type ChannelGroupEditDialogProps = {
  onClose?: (() => void);
  value?: StreamMasterApi.ChannelGroupDto | undefined;
};

export default React.memo(ChannelGroupEditDialog);
