import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import { Button } from "primereact/button";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { GetMessage, getTopToolOptions } from "../common/common";
import { InputText } from "primereact/inputtext";
// import StringEditorBodyTemplate from "./StringEditorBodyTemplate";


// import { DataView } from 'primereact/dataview';



const ChannelGroupEditDialog = (props: ChannelGroupEditDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  // const [regex, setRegex] = React.useState<string | undefined>('');
  const [newGroupName, setNewGroupName] = React.useState('');

  const [channelGroupsUpdateChannelGroupMutation] = StreamMasterApi.useChannelGroupsUpdateChannelGroupMutation();
  // const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamNamesByNamePatternQuery(regex ?? '');

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.(newGroupName);
  }, [props, newGroupName]);


  React.useEffect(() => {

    if (props.value) {
      setNewGroupName(props.value.name);
      // if (props.value.regexMatch !== null)
      //   setRegex(props.value.regexMatch);
    }

  }, [props.value]);

  const changeGroupName = React.useCallback(() => {
    setBlock(true);
    if (!newGroupName || !props.value) {
      ReturnToParent();
      return;
    }

    const toSend = {} as StreamMasterApi.UpdateChannelGroupRequest;

    toSend.channelGroupName = props.value?.name;
    toSend.newGroupName = newGroupName;

    // if (regex !== undefined && regex !== '') {
    //   toSend.regex = regex;
    // }

    channelGroupsUpdateChannelGroupMutation(toSend).then(() => {
      setInfoMessage('Channel Group Edit Successfully');
    }).catch((e) => {
      setInfoMessage('Channel Group Edit Error: ' + e.message);
    });
    setNewGroupName('');
  }, [ReturnToParent, channelGroupsUpdateChannelGroupMutation, newGroupName, props.value]);

  // const itemTemplate = (data: string) => {
  //   return (
  //     <div className="flex flex-column flex-row align-items-start">
  //       {data}
  //     </div>
  //   );
  // };

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={GetMessage("edit group")}
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3>{GetMessage("edit group")}</h3>
            <InputText
              autoFocus
              className="withpadding p-inputtext-sm w-full mb-1"
              onChange={(e) => setNewGroupName(e.target.value)}
              placeholder="Group Name"
              value={newGroupName}
            />

            {/* <StringEditorBodyTemplate
              includeBorder
              onChange={(e) => {
                setRegex(e)
              }}
              placeholder={GetMessage("channel group regex")}
              value={regex}
            /> */}

            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <Button
                icon="pi pi-check"
                label={GetMessage('ok')}
                onClick={changeGroupName}
                rounded
                severity="success"
              />
            </div>
            {/* <div hidden={regex === undefined || regex === ''}>
              <div className='m3uFilesEditor flex flex-column col-12 flex-shrink-0 '>
                <DataView
                  header={GetMessage("matches")}
                  itemTemplate={itemTemplate}
                  loading={videoStreamsQuery.isLoading || videoStreamsQuery.isFetching}
                  paginator
                  rows={25}
                  value={videoStreamsQuery.data} />
              </div>
            </div> */}
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
  onClose?: ((newName: string) => void);
  value?: StreamMasterApi.ChannelGroupDto | undefined;
};

export default React.memo(ChannelGroupEditDialog);
