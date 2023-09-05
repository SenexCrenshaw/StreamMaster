import { useState, useCallback, useMemo, memo } from "react";
import { isEmptyObject } from "../../common/common";
import { type StreamGroupDto, type DeleteStreamGroupRequest, useStreamGroupsDeleteStreamGroupMutation } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import DeleteButton from "../buttons/DeleteButton";

type StreamGroupDeleteDialogProps = {
  readonly iconFilled?: boolean | undefined;
  readonly onHide?: () => void;
  readonly value?: StreamGroupDto | undefined;
};

const StreamGroupDeleteDialog = ({ iconFilled, onHide, value }: StreamGroupDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [selectedStreamGroup, setSelectedStreamGroup] = useState<StreamGroupDto>({} as StreamGroupDto);
  const [infoMessage, setInfoMessage] = useState('');

  const [streamGroupsDeleteStreamGroupMutations] = useStreamGroupsDeleteStreamGroupMutation();

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onHide?.();
  }, [onHide]);

  useMemo(() => {

    if (value !== null && value !== undefined && !isEmptyObject(value)) {
      setSelectedStreamGroup(value);
    }

  }, [value]);

  const deleteStreamGroup = useCallback(async () => {
    setBlock(true);

    if (selectedStreamGroup === undefined) {
      ReturnToParent();

      return;
    }

    const data = {} as DeleteStreamGroupRequest;

    data.id = selectedStreamGroup.id;

    await streamGroupsDeleteStreamGroupMutations(data).then(() => {

      setInfoMessage('Stream Group Deleted Successfully');
    }).catch((error) => {
      setInfoMessage('Stream Group Delete Error: ' + error.message);
    });

  }, [ReturnToParent, selectedStreamGroup, streamGroupsDeleteStreamGroupMutations]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={`Delete "${selectedStreamGroup.name}" ?`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3 />
            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">

              <DeleteButton label='Delete Stream Group' onClick={async () => await deleteStreamGroup()} tooltip='Delete Stream Group' />

            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <DeleteButton iconFilled={iconFilled} onClick={() => setShowOverlay(true)} tooltip='Delete Stream Group' />

    </>
  );
}

StreamGroupDeleteDialog.displayName = 'StreamGroupDeleteDialog';
StreamGroupDeleteDialog.defaultProps = {
  iconFilled: true,
};

export default memo(StreamGroupDeleteDialog);

