import { Button } from "primereact/button";
import { useState, useCallback, useMemo, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type StreamGroupDto, type DeleteStreamGroupRequest, useStreamGroupsDeleteStreamGroupMutation } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import { useStreamGroupToRemove } from "../../app/slices/useStreamGroupToRemove";


type StreamGroupDeleteDialogProps = {
  iconFilled?: boolean | undefined;
  id: string;
  onHide?: () => void;
  value?: StreamGroupDto | undefined;
};


const StreamGroupDeleteDialog = (props: StreamGroupDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [selectedStreamGroup, setSelectedStreamGroup] = useState<StreamGroupDto>({} as StreamGroupDto);
  const [infoMessage, setInfoMessage] = useState('');

  const { setStreamGroupToRemove } = useStreamGroupToRemove(props.id);

  const [streamGroupsDeleteStreamGroupMutations] = useStreamGroupsDeleteStreamGroupMutation();

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onHide?.();
  }, [props]);

  useMemo(() => {

    if (props.value !== null && props.value !== undefined) {
      setSelectedStreamGroup(props.value);
    }

  }, [props.value]);

  const deleteStreamGroup = useCallback(async () => {
    setBlock(true);

    if (selectedStreamGroup === undefined) {
      ReturnToParent();

      return;
    }

    const data = {} as DeleteStreamGroupRequest;

    data.id = selectedStreamGroup.id;

    await streamGroupsDeleteStreamGroupMutations(data).then(() => {
      setStreamGroupToRemove(selectedStreamGroup.id);
    }).catch((error) => {
      setInfoMessage('Stream Group Delete Error: ' + error.message);
    });

  }, [ReturnToParent, selectedStreamGroup, setStreamGroupToRemove, streamGroupsDeleteStreamGroupMutations]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header={`Delete "${selectedStreamGroup.name}" ?`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3 />
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
                label="Delete"
                onClick={async () => await deleteStreamGroup()}
                rounded
                severity="success"
              />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <Button
        icon="pi pi-minus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="danger"
        size="small"
        text={props.iconFilled !== true}
        tooltip="Delete Stream Group"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

StreamGroupDeleteDialog.displayName = 'StreamGroupDeleteDialog';
StreamGroupDeleteDialog.defaultProps = {
  iconFilled: true,
  value: null,
};

export default memo(StreamGroupDeleteDialog);

