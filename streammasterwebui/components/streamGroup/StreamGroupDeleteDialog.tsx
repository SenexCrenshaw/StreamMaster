import SMPopUp from '@components/sm/SMPopUp';
import { DeleteStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { DeleteStreamGroupRequest, StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { memo, useCallback } from 'react';

interface StreamGroupDeleteDialogProperties {
  readonly streamGroup: StreamGroupDto;
  readonly onHide?: () => void;
}

const StreamGroupDeleteDialog = ({ streamGroup, onHide }: StreamGroupDeleteDialogProperties) => {
  const ReturnToParent = useCallback(() => {
    onHide?.();
  }, [onHide]);

  const deleteStreamGroup = useCallback(async () => {
    if (streamGroup === undefined) {
      ReturnToParent();

      return;
    }

    const request = {} as DeleteStreamGroupRequest;

    request.Id = streamGroup.Id;

    await DeleteStreamGroup(request)
      .then(() => {})
      .catch((error) => {
        console.error('Error Deleting SG', error);
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [ReturnToParent, streamGroup]);

  return (
    <div className="flex justify-content-center w-full">
      <SMPopUp title="Delete Stream Group" onOkClick={() => deleteStreamGroup()} icon="pi-times">
        <div className="sm-center-stuff">
          <div className="text-container">{streamGroup.Name}</div>
        </div>
      </SMPopUp>
    </div>
  );
};

StreamGroupDeleteDialog.displayName = 'StreamGroupDeleteDialog';

export default memo(StreamGroupDeleteDialog);
