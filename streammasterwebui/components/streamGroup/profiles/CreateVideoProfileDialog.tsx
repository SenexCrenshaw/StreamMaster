import StringEditor from '@components/inputs/StringEditor';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { AddVideoProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { AddVideoProfileRequest } from '@lib/smAPI/smapiTypes';
import { useCallback, useMemo, useRef, useState } from 'react';

const CreateVideoProfileDialog = () => {
  const defaultValues = useMemo(
    () =>
      ({
        Command: 'ffmpeg',
        Parameters:
          '-hide_banner -loglevel error -user_agent {clientUserAgent} -i {streamUrl} -reconnect 1 -map 0:v -map 0:a? -map 0:s? -c copy -bsf:v h264_mp4toannexb -f mpegts pipe:1',
        Timeout: 100
      } as AddVideoProfileRequest),
    []
  );

  const smPopUpRef = useRef<SMPopUpRef>(null);
  const [addVideoProfileRequest, setAddVideoProfileRequest] = useState<AddVideoProfileRequest>(defaultValues);

  const ReturnToParent = useCallback(() => {
    smPopUpRef.current?.hide();
    setAddVideoProfileRequest(defaultValues);
  }, [defaultValues]);

  const save = useCallback(() => {
    // Logger.debug('CreateVideoProfileDialog', addVideoProfileRequest);

    AddVideoProfile(addVideoProfileRequest)
      .then((response) => {})
      .catch((error) => {
        Logger.error('CreateFileProfileDialog', { error });
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [ReturnToParent, addVideoProfileRequest]);

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
      okButtonDisabled={!addVideoProfileRequest.Name || !addVideoProfileRequest.Command || !addVideoProfileRequest.Parameters}
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
                  addVideoProfileRequest.Name = e;
                  setAddVideoProfileRequest({ ...addVideoProfileRequest });
                }
              }}
              onSave={(e) => {
                save();
              }}
              value={addVideoProfileRequest.Name}
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
                  addVideoProfileRequest.Command = e;
                  setAddVideoProfileRequest({ ...addVideoProfileRequest });
                }
              }}
              onSave={(e) => {
                save();
              }}
              value={addVideoProfileRequest.Command}
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
                  addVideoProfileRequest.Parameters = e;
                  setAddVideoProfileRequest({ ...addVideoProfileRequest });
                }
              }}
              onSave={(e) => {
                save();
              }}
              value={addVideoProfileRequest.Parameters}
            />
          </div>
        </div>

        <div className="layout-padding-bottom-lg sm-headerBg border-radius-bottom" />
      </>
    </SMPopUp>
  );
};

export default CreateVideoProfileDialog;
