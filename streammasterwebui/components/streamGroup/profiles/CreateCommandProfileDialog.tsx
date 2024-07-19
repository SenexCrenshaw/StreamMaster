import StringEditor from '@components/inputs/StringEditor';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { AddCommandProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { AddCommandProfileRequest } from '@lib/smAPI/smapiTypes';
import { useCallback, useMemo, useRef, useState } from 'react';

const CreateCommandProfileDialog = () => {
  const defaultValues = useMemo(
    () =>
      ({
        Command: 'ffmpeg',
        Parameters:
          '-hide_banner -loglevel error -user_agent {clientUserAgent} -i {streamUrl} -reconnect 1 -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts pipe:1',
        ProfileName: 'Default',
        Timeout: 100
      } as AddCommandProfileRequest),
    []
  );

  const smPopUpRef = useRef<SMPopUpRef>(null);
  const [AddCommandProfileRequest, setAddCommandProfileRequest] = useState<AddCommandProfileRequest>(defaultValues);

  const ReturnToParent = useCallback(() => {
    smPopUpRef.current?.hide();
    setAddCommandProfileRequest(defaultValues);
  }, [defaultValues]);

  const save = useCallback(() => {
    // Logger.debug('CreateCommandProfileDialog', AddCommandProfileRequest);

    AddCommandProfile(AddCommandProfileRequest)
      .then((response) => {})
      .catch((error) => {
        Logger.error('CreateFileProfileDialog', { error });
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [ReturnToParent, AddCommandProfileRequest]);

  return (
    <SMPopUp
      buttonClassName="icon-green"
      contentWidthSize="6"
      icon="pi-plus-circle"
      modal
      onOkClick={() => {
        save();
      }}
      onCloseClick={() => {
        ReturnToParent();
      }}
      okButtonDisabled={!AddCommandProfileRequest.ProfileName || !AddCommandProfileRequest.Command || !AddCommandProfileRequest.Parameters}
      placement="top-end"
      ref={smPopUpRef}
      title="Create Profile"
      tooltip="Create Profile"
      zIndex={12}
    >
      <>
        <div className="sm-center-stuff gap-1">
          <div className="layout-padding-bottom-lg" />
          <div className="sm-w-2">
            <StringEditor
              autoFocus
              label="Name"
              placeholder="Name"
              darkBackGround
              disableDebounce
              onChange={(e) => {
                if (e !== undefined) {
                  AddCommandProfileRequest.ProfileName = e;
                  setAddCommandProfileRequest({ ...AddCommandProfileRequest });
                }
              }}
              onSave={(e) => {
                save();
              }}
              value={AddCommandProfileRequest.ProfileName}
            />
          </div>
          <div className="sm-w-2">
            <StringEditor
              label="Command"
              placeholder="Command"
              darkBackGround
              disableDebounce
              onChange={(e) => {
                if (e !== undefined) {
                  AddCommandProfileRequest.Command = e;
                  setAddCommandProfileRequest({ ...AddCommandProfileRequest });
                }
              }}
              onSave={(e) => {
                save();
              }}
              value={AddCommandProfileRequest.Command}
            />
          </div>
          <div className="sm-w-6">
            <StringEditor
              isLarge
              label="Parameters"
              placeholder="Parameters"
              darkBackGround
              disableDebounce
              onChange={(e) => {
                if (e !== undefined) {
                  AddCommandProfileRequest.Parameters = e;
                  setAddCommandProfileRequest({ ...AddCommandProfileRequest });
                }
              }}
              onSave={(e) => {
                save();
              }}
              value={AddCommandProfileRequest.Parameters}
            />
          </div>
        </div>

        <div className="layout-padding-bottom-lg sm-headerBg border-radius-bottom" />
      </>
    </SMPopUp>
  );
};

export default CreateCommandProfileDialog;
