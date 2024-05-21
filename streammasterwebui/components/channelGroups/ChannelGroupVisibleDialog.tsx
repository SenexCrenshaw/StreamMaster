import React, { useMemo } from 'react';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import VisibleButton from '../buttons/VisibleButton';
import { ChannelGroupDto, UpdateChannelGroupRequest, UpdateChannelGroupsRequest } from '@lib/smAPI/smapiTypes';
import { UpdateChannelGroup, UpdateChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import OKButton from '@components/buttons/OKButton';

interface ChannelGroupVisibleDialogProperties {
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean | undefined;
  readonly value?: ChannelGroupDto | undefined;
}

const ChannelGroupVisibleDialog = ({ id, onClose, skipOverLayer = false, value }: ChannelGroupVisibleDialogProperties) => {
  const { selectedItems } = useSelectedItems<ChannelGroupDto>(id);
  const { selectAll } = useSelectAll(id);
  const dialogRef = React.useRef<SMDialogRef>(null);

  const ReturnToParent = React.useCallback(() => {
    onClose?.();
  }, [onClose]);

  const onVisibleClick = React.useCallback(async () => {
    if (!value && selectedItems.length === 0) {
      ReturnToParent();
      return;
    }

    if (value) {
      const toSend = {} as UpdateChannelGroupRequest;
      toSend.ChannelGroupId = value.Id;
      toSend.ToggleVisibility = true;
      UpdateChannelGroup(toSend)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        })
        .finally(() => {
          dialogRef.current?.close();
        });
    } else if (selectedItems) {
      const toSend = {} as UpdateChannelGroupsRequest;
      toSend.requests = selectedItems.map(
        (item) =>
          ({
            ChannelGroupId: item.Id,
            ToggleVisibility: true
          } as UpdateChannelGroupRequest)
      );
      UpdateChannelGroups(toSend)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        })
        .finally(() => {
          dialogRef.current?.close();
        });
    }
  }, [ReturnToParent, selectedItems, value]);

  const message = useMemo(() => {
    if (value) {
      return `Toggle Visibility for '${value.Name}'?`;
    }

    if (selectAll) {
      return `Toggle Visibility for Groups?`;
    }

    return `Toggle Visibility for Groups (${selectedItems.length})?`;
  }, [selectAll, selectedItems.length, value]);

  if (value) {
    return <VisibleButton iconFilled={false} onClick={async () => await onVisibleClick()} />;
  }

  return (
    <SMDialog
      ref={dialogRef}
      title="TOGGLE VISIBILITY"
      iconFilled={value === undefined}
      onHide={() => ReturnToParent()}
      buttonClassName="icon-blue"
      icon="pi-eye-slash"
      widthSize={2}
      info="General"
      tooltip="Toggle Visiblity"
      header={<OKButton onClick={async () => await onVisibleClick()} tooltip="Toggle Visiblity" />}
    >
      <div className="flex justify-content-center w-full mb-2">{message}</div>
    </SMDialog>
  );
};

ChannelGroupVisibleDialog.displayName = 'ChannelGroupVisibleDialog';

export default React.memo(ChannelGroupVisibleDialog);
