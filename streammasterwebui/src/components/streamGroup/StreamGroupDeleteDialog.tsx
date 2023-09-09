import { memo, useCallback, useState } from "react";
import { useSelectedStreamGroup } from "../../app/slices/useSelectedStreamGroup";
import { useStreamGroupsDeleteStreamGroupMutation, type DeleteStreamGroupRequest } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import DeleteButton from "../buttons/DeleteButton";

type StreamGroupDeleteDialogProps = {
  readonly id: string;
  readonly onHide?: () => void;
};

const StreamGroupDeleteDialog = ({ id, onHide }: StreamGroupDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');

  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup(id);
  const [streamGroupsDeleteStreamGroupMutations] = useStreamGroupsDeleteStreamGroupMutation();

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onHide?.();
  }, [onHide]);


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
    setSelectedStreamGroup(undefined);
  }, [ReturnToParent, selectedStreamGroup, setSelectedStreamGroup, streamGroupsDeleteStreamGroupMutations]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header='Delete Stream Group?'
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
      <DeleteButton disabled={selectedStreamGroup === undefined || selectedStreamGroup.id === undefined || selectedStreamGroup.id === 0} onClick={() => setShowOverlay(true)} tooltip='Delete Stream Group' />

    </>
  );
}

StreamGroupDeleteDialog.displayName = 'StreamGroupDeleteDialog';

export default memo(StreamGroupDeleteDialog);

