import MinusButton from '@components/buttons/MinusButton';

import { memo, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import OKButton from '../buttons/OKButton';

interface ProfileDeleteDialogProperties {
  readonly iconFilled?: boolean;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean;
  readonly data: FfmpegProfileDto;
}

const ProfileDeleteDialog = ({ iconFilled, onClose, skipOverLayer, data }: ProfileDeleteDialogProperties) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');

  const [block, setBlock] = useState<boolean>(false);

  const [settingsRemoveFfmpegProfileMutation] = useProfilesRemoveFfmpegProfileMutation();

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  };

  const deleteProfile = async () => {
    setBlock(true);

    if (!data) {
      ReturnToParent();
      return;
    }
    const tosend: ProfilesRemoveFfmpegProfileApiArg = {};
    tosend.name = data.name;

    await settingsRemoveFfmpegProfileMutation(tosend)
      .then(() => {
        setInfoMessage('Delete Stream Successful');
      })
      .catch((error) => {
        setInfoMessage(`Delete Stream Error: ${error.message}`);
      });
  };

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header={'Delete ' + data.name + ' ?'}
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={2}
        show={showOverlay}
      >
        <div className="m-0 p-0 w-full">
          <div className="card flex mt-3 flex-wrap gap-1 justify-content-center ">
            <OKButton onClick={async () => await deleteProfile()} />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <MinusButton disabled={false} iconFilled={iconFilled} onClick={() => setShowOverlay(true)} tooltip="Delete Profile" />
    </>
  );
};

ProfileDeleteDialog.displayName = 'ProfileDeleteDialog';

export default memo(ProfileDeleteDialog);
