import NumberEditor from '@components/inputs/NumberEditor';
import StringEditor from '@components/inputs/StringEditor';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { AddVideoProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { AddVideoProfileRequest } from '@lib/smAPI/smapiTypes';
import { useCallback, useRef, useState } from 'react';

const CreateVideoProfileDialog = () => {
  const smPopUpRef = useRef<SMPopUpRef>(null);
  const [addVideoProfileRequest, setAddVideoProfileRequest] = useState<AddVideoProfileRequest>({ Timeout: 100 } as AddVideoProfileRequest);

  const save = useCallback(() => {
    Logger.debug('CreateVideoProfileDialog', addVideoProfileRequest);

    AddVideoProfile(addVideoProfileRequest)
      .then((response) => {})
      .catch((error) => {
        Logger.error('CreateFileProfileDialog', { error });
      })
      .finally(() => {
        smPopUpRef.current?.hide();
      });
  }, [addVideoProfileRequest]);

  return (
    <SMPopUp
      contentWidthSize="6"
      buttonClassName="icon-green"
      iconFilled
      icon="pi-plus"
      title="Create Profile"
      ref={smPopUpRef}
      modal
      modalCentered
      onOkClick={() => {
        save();
      }}
      okButtonDisabled={!addVideoProfileRequest.Name || !addVideoProfileRequest.Command || !addVideoProfileRequest.Parameters}
      tooltip="Create Profile"
      zIndex={10}
    >
      <>
        <div className="sm-headerBg dialog-padding border-sides">
          <div className="w-12 ">
            <div className="flex gap-1">
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
              <div className="sm-w-1">
                <NumberEditor
                  label="Timeout"
                  darkBackGround
                  disableDebounce
                  min={100}
                  showButtons
                  onChange={(e) => {
                    if (e !== undefined) {
                      addVideoProfileRequest.Timeout = e;
                      setAddVideoProfileRequest({ ...addVideoProfileRequest });
                    }
                  }}
                  onSave={(e) => {
                    save();
                  }}
                  value={addVideoProfileRequest.Timeout}
                />
              </div>
            </div>
          </div>
        </div>
        <div className="layout-padding-bottom-lg sm-headerBg border-radius-bottom" />
      </>
    </SMPopUp>
  );
};

export default CreateVideoProfileDialog;
