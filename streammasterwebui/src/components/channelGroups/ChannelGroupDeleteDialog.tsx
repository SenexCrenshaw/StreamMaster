import React, { useMemo } from "react";
import { useChannelGroupToRemove } from "../../app/slices/useChannelGroupToRemove";
import { useQueryFilter } from "../../app/slices/useQueryFilter";
import { useSelectAll } from "../../app/slices/useSelectAll";
import { useSelectedChannelGroups } from "../../app/slices/useSelectedChannelGroups";
import { useChannelGroupsDeleteAllChannelGroupsFromParametersMutation, useChannelGroupsDeleteChannelGroupMutation, type ChannelGroupDto, type ChannelGroupsDeleteAllChannelGroupsFromParametersApiArg, type DeleteChannelGroupRequest } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import DeleteButton from "../buttons/DeleteButton";

type ChannelGroupDeleteDialogProps = {
  readonly cgid: string;
  readonly iconFilled?: boolean | undefined;
  readonly id: string;
  readonly onDelete?: (results: number[] | undefined) => void;
  readonly onHide?: () => void;
  readonly value?: ChannelGroupDto | undefined;
};

const ChannelGroupDeleteDialog = ({
  iconFilled,
  id,
  cgid,
  onDelete,
  onHide,
  value,
}: ChannelGroupDeleteDialogProps) => {

  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);

  const [infoMessage, setInfoMessage] = React.useState('');

  const { setChannelGroupToRemove } = useChannelGroupToRemove(id);

  const [channelGroupDto, setChannelGroupDto] = React.useState<ChannelGroupDto>();

  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);
  const { selectedChannelGroups } = useSelectedChannelGroups(cgid);

  const [channelGroupsDeleteChannelGroupMutation] = useChannelGroupsDeleteChannelGroupMutation();
  const [channelGroupsDeleteAllChannelGroupsFromParametersMutation] = useChannelGroupsDeleteAllChannelGroupsFromParametersMutation();

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onHide?.();
  }, [onHide]);

  React.useMemo(() => {
    if (value !== null) {
      setChannelGroupDto(value);
    }
  }, [value]);

  const deleteGroup = React.useCallback(async () => {
    setBlock(true);

    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();

        return;
      }

      const toSendAll = {} as ChannelGroupsDeleteAllChannelGroupsFromParametersApiArg;

      toSendAll.parameters = queryFilter;

      await channelGroupsDeleteAllChannelGroupsFromParametersMutation(toSendAll)
        .then(() => {
          setInfoMessage('Deleted Successfully');
        }
        ).catch((error) => {
          setInfoMessage('Delete Error: ' + error.message);
        });

      return;
    }

    if (!channelGroupDto) {
      ReturnToParent();

      return;
    }

    if (selectedChannelGroups.length === 0) {
      ReturnToParent();

      return;
    }

    const promises = [];
    const groupIds = [] as number[];

    for (const group of selectedChannelGroups.filter((a) => a.id !== undefined && !a.isReadOnly)) {

      if (group.id === undefined) {
        continue;
      }

      const data = {} as DeleteChannelGroupRequest;

      data.channelGroupId = group.id;
      groupIds.push(group.id);
      promises.push(
        channelGroupsDeleteChannelGroupMutation(data)
          .then(() => {
            if (group.id !== undefined) {
              setChannelGroupToRemove(group.id);
            }
          }).catch(() => { })
      );
    }

    const p = Promise.all(promises);

    await p.then(() => {
      setInfoMessage('Channel Group Delete Successful');
      onDelete?.(groupIds);
    }).catch((error) => {
      setInfoMessage('Channel Group Delete Error: ' + error.message);
      onDelete?.(undefined);
    });


  }, [ReturnToParent, channelGroupsDeleteAllChannelGroupsFromParametersMutation, channelGroupsDeleteChannelGroupMutation, onDelete, queryFilter, selectAll, selectedChannelGroups, setChannelGroupToRemove, channelGroupDto]);

  const isFirstDisabled = useMemo(() => {
    if (channelGroupDto) {
      return channelGroupDto.isReadOnly;
    }

    if (!selectedChannelGroups || selectedChannelGroups?.length === 0) {
      return true;
    }

    return selectedChannelGroups[0].isReadOnly;

  }, [channelGroupDto, selectedChannelGroups]);

  const getTotalCount = useMemo(() => {
    let count = selectedChannelGroups?.length ?? 0;

    if (count === 1 && isFirstDisabled) {
      return 0;
    }

    return count;

  }, [isFirstDisabled, selectedChannelGroups?.length]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={`Delete "${selectedChannelGroups.filter((a) => !a.isReadOnly).length < 2 ? selectedChannelGroups.filter((a) => !a.isReadOnly)[0] ? selectedChannelGroups.filter((a) => !a.isReadOnly)[0].name + '" Group ?' : ' Group ?' : selectedChannelGroups.filter((a) => !a.isReadOnly).length + ' Groups ?'}`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        show={showOverlay}
      >
        <div className="flex justify-content-center w-full">
          <DeleteButton disabled={getTotalCount === 0 && !selectAll} label="Delete Groups" onClick={async () => await deleteGroup()} tooltip="Delete User Created Groups" />
        </div>
      </InfoMessageOverLayDialog>

      <DeleteButton disabled={getTotalCount === 0} iconFilled={iconFilled} onClick={() => setShowOverlay(true)} tooltip="Delete User Created Group" />

    </>
  );
}

ChannelGroupDeleteDialog.displayName = 'ChannelGroupDeleteDialog';
export default React.memo(ChannelGroupDeleteDialog);
