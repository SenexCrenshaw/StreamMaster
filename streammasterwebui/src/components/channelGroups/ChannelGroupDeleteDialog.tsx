import React, { useMemo } from "react";
import { type ChannelGroupsDeleteAllChannelGroupsFromParametersApiArg } from "../../store/iptvApi";
import { useChannelGroupsDeleteAllChannelGroupsFromParametersMutation, type ChannelGroupDto, type DeleteChannelGroupRequest } from "../../store/iptvApi";
import { useChannelGroupsDeleteChannelGroupMutation } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import DeleteButton from "../buttons/DeleteButton";
import { useChannelGroupToRemove } from "../../app/slices/useChannelGroupToRemove";
import { useQueryFilter } from "../../app/slices/useQueryFilter";
import { useSelectAll } from "../../app/slices/useSelectAll";

type ChannelGroupDeleteDialogProps = {
  iconFilled?: boolean | undefined;
  id: string;
  onDelete?: (results: number[] | undefined) => void;
  onHide?: () => void;
  values?: ChannelGroupDto[] | undefined;
};


const ChannelGroupDeleteDialog = ({
  iconFilled,
  id,
  onDelete,
  onHide,
  values,
}: ChannelGroupDeleteDialogProps) => {

  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<ChannelGroupDto[]>([] as ChannelGroupDto[]);
  const [infoMessage, setInfoMessage] = React.useState('');

  const { setChannelGroupToRemove } = useChannelGroupToRemove(id);

  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const [channelGroupsDeleteChannelGroupMutation] = useChannelGroupsDeleteChannelGroupMutation();
  const [channelGroupsDeleteAllChannelGroupsFromParametersMutation] = useChannelGroupsDeleteAllChannelGroupsFromParametersMutation();

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onHide?.();
  }, [onHide]);

  React.useMemo(() => {

    if (values !== null && values !== undefined) {
      setSelectedChannelGroups(values);
    }

  }, [values]);

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

    if ((!values || values?.length === 0)) {
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

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedChannelGroups]);

  const isFirstDisabled = useMemo(() => {
    if (!values || values?.length === 0) {
      return true;
    }

    return values[0].isReadOnly;

  }, [values]);

  const getTotalCount = useMemo(() => {
    let count = values?.length ?? 0;

    if (count === 1 && isFirstDisabled) {
      return 0;
    }

    return count;

  }, [isFirstDisabled, values?.length]);

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
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <DeleteButton disabled={getTotalCount === 0 && !selectAll} label="Delete Groups" onClick={async () => await deleteGroup()} tooltip="Delete User Created Groups" />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <DeleteButton disabled={getTotalCount === 0} iconFilled={iconFilled} onClick={() => setShowOverlay(true)} tooltip="Delete User Created Group" />

    </>
  );
}

ChannelGroupDeleteDialog.displayName = 'ChannelGroupDeleteDialog';
export default React.memo(ChannelGroupDeleteDialog);
