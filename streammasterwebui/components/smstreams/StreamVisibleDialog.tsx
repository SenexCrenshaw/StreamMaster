import { ToggleSMStreamVisibleById } from '@lib/smAPI/SMStreams/SMStreamsCommands';
import { SMStreamDto, ToggleSMStreamVisibleByIdRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback } from 'react';
import VisibleButton from '../buttons/VisibleButton';

interface StreamVisibleDialogProperties {
  readonly iconFilled?: boolean;
  readonly onClose?: () => void;
  readonly value: SMStreamDto;
}

const StreamVisibleDialog = ({ iconFilled, onClose, value }: StreamVisibleDialogProperties) => {
  const ReturnToParent = useCallback(() => {
    onClose?.();
  }, [onClose]);

  const onVisiblesClick = useCallback(async () => {
    if (value === undefined) {
      ReturnToParent();
      return;
    }

    await ToggleSMStreamVisibleById({ Id: value.Id as string } as ToggleSMStreamVisibleByIdRequest)
      .then(() => {})
      .catch((error) => {
        console.error('Error toggling visibility', error);
        throw error;
      });
  }, [ReturnToParent, value]);

  return <VisibleButton iconFilled={iconFilled} label="Toggle Visibility" onClick={async () => await onVisiblesClick()} tooltip="Toggle Visibility" isLeft />;
};

StreamVisibleDialog.displayName = 'StreamVisibleDialog';

export default memo(StreamVisibleDialog);
