import { memo, useCallback } from 'react';
import { DeleteStreamGroupRequest, StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { DeleteStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { SMPopUp } from '@components/sm/SMPopUp';

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
      <SMPopUp title="Delete Stream Group" OK={() => deleteStreamGroup()} icon="pi-times" severity="danger">
        <div>
          "{streamGroup.Name}"
          <br />
          Are you sure?
        </div>
      </SMPopUp>
    </div>
  );
};

StreamGroupDeleteDialog.displayName = 'StreamGroupDeleteDialog';

export default memo(StreamGroupDeleteDialog);
