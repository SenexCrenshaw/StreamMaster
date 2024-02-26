import AddButton from '@components/buttons/AddButton';
import SaveButton from '@components/buttons/SaveButton';
import InputWrapper from '@components/videoStreamPanel/InputWrapper';
import { getTopToolOptions } from '@lib/common/common';
import { ProfilesAddFfmpegProfileApiArg, useProfilesAddFfmpegProfileMutation } from '@lib/iptvApi';
import { Checkbox, CheckboxChangeEvent } from 'primereact/checkbox';
import { InputText } from 'primereact/inputtext';
import { memo, useMemo, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';

interface ProfileAddDialogProperties {
  // readonly iconFilled?: boolean;
  readonly onClose?: () => void;
  // readonly skipOverLayer?: boolean;
  // readonly data: FfmpegProfileDto;
}

const ProfileAddDialog = ({ onClose }: ProfileAddDialogProperties) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');

  const [name, setName] = useState<string | undefined>(undefined);
  const [parameters, setParameters] = useState<string | undefined>(undefined);
  // const [timeout, setTimeout] = useState<number | undefined>(undefined);
  const [isM3U8, setIsM3U8] = useState<boolean>(false);

  const [block, setBlock] = useState<boolean>(false);

  const [settingsAddFfmpegProfileMutation] = useProfilesAddFfmpegProfileMutation();

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    setName(undefined);
    setParameters(undefined);
    // setTimeout(undefined);
    setIsM3U8(false);
    onClose?.();
  };

  const addVideoStream = async () => {
    setBlock(true);

    if (!isSaveEnabled) {
      ReturnToParent();
      return;
    }
    const tosend: ProfilesAddFfmpegProfileApiArg = {};
    tosend.name = name;
    tosend.parameters = parameters;
    tosend.isM3U8 = isM3U8;
    tosend.timeOut = 30;

    await settingsAddFfmpegProfileMutation(tosend)
      .then(() => {
        setInfoMessage('Delete Stream Successful');
      })
      .catch((error) => {
        setInfoMessage(`Delete Stream Error: ${error.message}`);
      });
  };

  const isSaveEnabled = useMemo((): boolean => {
    var ret =
      name !== undefined && name.length > 0 && parameters !== undefined && parameters.length > 0 && isM3U8 !== undefined && parameters.includes('{streamUrl}');
    return ret;
  }, [isM3U8, name, parameters]);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={name ? 'Add ' + name : 'Add'}
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={6}
        show={showOverlay}
      >
        <div className="m-0 p-0 w-full">
          <div className="card flex mt-3 flex-wrap gap-2 justify-content-center ">
            <div className="flex col-12 justify-content-between">
              <InputWrapper
                columnSize={3}
                label="Name"
                renderInput={() => (
                  <InputText
                    autoFocus
                    className="w-full bordered-text mr-2"
                    onChange={(e) => setName(e.target.value)}
                    placeholder="Name"
                    type="text"
                    value={name}
                  />
                )}
              />

              <InputWrapper
                columnSize={7}
                label="Parameters: Use {streamUrl} for url placement"
                renderInput={() => (
                  <InputText
                    className="w-full bordered-text mr-2"
                    onChange={(e) => setParameters(e.target.value)}
                    placeholder="Parameters"
                    type="text"
                    value={parameters}
                  />
                )}
              />

              <InputWrapper
                columnSize={2}
                label="Is M3U8 Output"
                renderInput={() => (
                  <Checkbox
                    checked={isM3U8 ?? false}
                    onChange={async (e: CheckboxChangeEvent) => {
                      setIsM3U8(e.checked ?? false);
                    }}
                    tooltip="Is M3U8 Output"
                    tooltipOptions={getTopToolOptions}
                  />
                )}
              />
            </div>
          </div>
          <div className="card flex mr-3 flex-wrap gap-2 justify-content-end ">
            <SaveButton iconFilled={false} disabled={!isSaveEnabled} onClick={async () => await addVideoStream()} />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <AddButton iconFilled={false} onClick={() => setShowOverlay(true)} tooltip="Add Profile" />
    </>
  );
};

ProfileAddDialog.displayName = 'ProfileAddDialog';

export default memo(ProfileAddDialog);
