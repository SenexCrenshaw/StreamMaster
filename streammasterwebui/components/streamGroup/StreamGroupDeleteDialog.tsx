import SMPopUp from '@components/sm/SMPopUp';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { DeleteStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { DeleteStreamGroupRequest, StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { memo, useCallback } from 'react';

interface StreamGroupDeleteDialogProperties {
  readonly streamGroup: StreamGroupDto;
  readonly zIndex?: number;
  readonly onHide?: () => void;
}

const StreamGroupDeleteDialog = ({ streamGroup, onHide, zIndex }: StreamGroupDeleteDialogProperties) => {
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const ReturnToParent = useCallback(() => {
    onHide?.();
  }, [onHide]);

  const deleteStreamGroup = useCallback(async () => {
    if (streamGroup === undefined) {
      ReturnToParent();

      return;
    }

    const request = {} as DeleteStreamGroupRequest;

    request.StreamGroupId = streamGroup.Id;

    await DeleteStreamGroup(request)
      .then(() => {})
      .catch((error) => {
        console.error('Error Deleting SG', error);
      })
      .finally(() => {
        request.StreamGroupId = streamGroup.Id;
        if (selectedStreamGroup?.Id === streamGroup.Id) {
          setSelectedStreamGroup(undefined);
        }
        ReturnToParent();
      });
  }, [ReturnToParent, selectedStreamGroup, setSelectedStreamGroup, streamGroup]);

  return (
    <div className="flex justify-content-center w-full">
      <SMPopUp
        placement="bottom-end"
        zIndex={zIndex}
        buttonClassName="icon-red"
        modal
        info=""
        title="Delete Stream Group"
        onOkClick={() => deleteStreamGroup()}
        icon="pi-times"
      >
        <div className="sm-center-stuff">
          <div className="text-container">{streamGroup.Name}</div>
        </div>
      </SMPopUp>
    </div>
  );
};

StreamGroupDeleteDialog.displayName = 'StreamGroupDeleteDialog';

export default memo(StreamGroupDeleteDialog);
