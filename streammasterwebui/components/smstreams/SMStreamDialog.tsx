import IconSelector from '@components/icons/IconSelector';
import NumberEditor from '@components/inputs/NumberEditor';
import SMChannelGroupDropDown from '@components/inputs/SMChannelGroupDropDown';
import StringEditor from '@components/inputs/StringEditor';
import { CreateSMChannelRequest, CreateSMStreamRequest, SMChannelDto, SMStreamDto } from '@lib/smAPI/smapiTypes';
import React, { forwardRef, useCallback, useEffect, useImperativeHandle, useMemo, useState } from 'react';

interface SMStreamDialogProperties {
  onSave: (request: CreateSMChannelRequest) => void;
  readonly onSaveEnabled?: (saveEnabled: boolean) => void;
  readonly smStreamDto?: SMStreamDto;
}
export interface SMStreamDialogRef {
  save: () => void;
}

const SMStreamDialog = forwardRef<SMStreamDialogRef, SMStreamDialogProperties>(({ onSave, onSaveEnabled, smStreamDto }, ref) => {
  const [request, setRequest] = useState<CreateSMStreamRequest>({} as CreateSMStreamRequest);
  const [orig, setOrig] = useState<SMStreamDto | null>(null);

  const doSave = useCallback(() => {
    onSave(request);
  }, [onSave, request]);

  useEffect(() => {
    if (smStreamDto) {
      if (orig === null) {
        setOrig(smStreamDto);
      }
      setRequest({
        ChannelNumber: smStreamDto.ChannelNumber,
        Group: smStreamDto.Group,
        Logo: smStreamDto.Logo,
        Name: smStreamDto.Name,
        Url: smStreamDto.Url
      });
    }
  }, [orig, smStreamDto]);

  const setName = useCallback(
    (value: string) => {
      if (request.Name !== value) {
        setRequest((prevRequest) => ({ ...prevRequest, Name: value }));
      }
    },
    [request.Name]
  );

  const setUrl = useCallback(
    (value: string) => {
      if (request.Url !== value) {
        setRequest((prevRequest) => ({ ...prevRequest, Url: value }));
      }
    },
    [request.Url]
  );

  const setLogo = useCallback(
    (value: string) => {
      if (request.Logo !== value) {
        setRequest((prevRequest) => ({ ...prevRequest, Logo: value }));
      }
    },
    [request.Logo]
  );

  const setGroup = useCallback(
    (value: string) => {
      if (request.Group !== value) {
        setRequest((prevRequest) => ({ ...prevRequest, Group: value }));
      }
    },
    [request.Group]
  );

  const setChannelNumber = useCallback(
    (value: number) => {
      if (request.ChannelNumber !== value) {
        setRequest((prevRequest) => ({ ...prevRequest, ChannelNumber: value }));
      }
    },
    [request.ChannelNumber]
  );

  const isSaveEnabled = useMemo(() => {
    if (orig === null) {
      if (request.Name === '' || request.Url === '') {
        return false;
      }
      return true;
    }

    if (smStreamDto === undefined) {
      return false;
    }

    if (request.Name === '' || request.Url === '') {
      return false;
    }
    if (orig === null) {
      return false;
    }

    if (request.Name !== orig.Name) {
      return true;
    }

    if (request.Url !== orig.Url) {
      return true;
    }

    if (request.Group !== orig.Group) {
      return true;
    }

    if (request.ChannelNumber !== orig.ChannelNumber) {
      return true;
    }

    return false;
  }, [orig, request, smStreamDto]);

  useEffect(() => {
    onSaveEnabled && onSaveEnabled(isSaveEnabled);
  }, [isSaveEnabled, onSaveEnabled]);

  useImperativeHandle(
    ref,
    () => ({
      save: () => {
        doSave();
      }
    }),
    [doSave]
  );

  return (
    <>
      <div className="sm-headerBg dialog-padding border-sides">
        <div className="flex w-12 gap-1 pl-2">
          <div className="flex flex-column sm-w-9 gap-1 justify-content-between">
            <div className="flex w-12 gap-1">
              <div className="sm-w-6">
                <StringEditor
                  autoFocus
                  label="Name"
                  placeholder="Name"
                  darkBackGround
                  disableDebounce
                  onChange={(e) => e && setName(e)}
                  onSave={(e) => {
                    e && setName(e);
                    if (isSaveEnabled) {
                      doSave();
                    }
                  }}
                  value={request.Name}
                />
              </div>
              <div className="sm-w-6">
                <SMChannelGroupDropDown
                  darkBackGround
                  label="Group"
                  onChange={(e) => {
                    e && setGroup(e);
                  }}
                  smChannel={{ Group: request.Group } as SMChannelDto}
                />
              </div>
            </div>
            <div className="flex w-12 gap-1">
              <div className="w-9 justify-content-start align-items-center">
                <StringEditor disableDebounce label="URL" darkBackGround onChange={(e) => e && setUrl(e)} value={request.Url} />
              </div>
              <div className="w-3 justify-content-start align-items-center">
                <NumberEditor
                  disableDebounce
                  label="Channel #"
                  showButtons
                  darkBackGround
                  onChange={(e) => setChannelNumber(e)}
                  onSave={(e) => setChannelNumber(e)}
                  value={request.ChannelNumber}
                />
              </div>
            </div>
          </div>

          <div className="sm-w-8rem">
            <IconSelector darkBackGround label="Logo" fixed large enableEditMode onChange={(e) => setLogo(e)} value={request.Logo} />
          </div>
        </div>
      </div>
      <div className="layout-padding-bottom-lg sm-headerBg border-radius-bottom" />
    </>
  );
});

SMStreamDialog.displayName = 'SMStreamDialog';

export default React.memo(SMStreamDialog);
