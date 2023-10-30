import { useStreamGroupsDeleteStreamGroupMutation, type DeleteStreamGroupRequest } from '@lib/iptvApi';
import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import { memo, useCallback, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import XButton from '../buttons/XButton';

interface StreamGroupDeleteDialogProperties {
  readonly id: string;
  readonly onHide?: () => void;
}

const StreamGroupDeleteDialog = ({ id, onHide }: StreamGroupDeleteDialogProperties) => {
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

    await streamGroupsDeleteStreamGroupMutations(data)
      .then(() => {
        setSelectedStreamGroup();
        setInfoMessage('Stream Group Deleted Successfully');
      })
      .catch((error) => {
        setSelectedStreamGroup();
        setInfoMessage(`Stream Group Delete Error: ${error.message}`);
      });
  }, [ReturnToParent, selectedStreamGroup, setSelectedStreamGroup, streamGroupsDeleteStreamGroupMutations]);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Delete Stream Group?"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className="flex justify-content-center w-full">
          <XButton label="Delete Stream Group" onClick={async () => await deleteStreamGroup()} tooltip="Delete Stream Group" />
        </div>
      </InfoMessageOverLayDialog>
      <XButton
        disabled={selectedStreamGroup === undefined || selectedStreamGroup.id === undefined || selectedStreamGroup.id < 2}
        onClick={() => setShowOverlay(true)}
        tooltip="Delete Stream Group"
      />
    </>
  );
};

StreamGroupDeleteDialog.displayName = 'StreamGroupDeleteDialog';

export default memo(StreamGroupDeleteDialog);
