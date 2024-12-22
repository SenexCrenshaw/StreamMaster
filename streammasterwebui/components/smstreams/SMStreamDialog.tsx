import EPGSelector from '@components/epg/EPGSelector';
import IconSelector from '@components/icons/IconSelector';
import NumberEditor from '@components/inputs/NumberEditor';
import SMChannelGroupDropDown from '@components/inputs/SMChannelGroupDropDown';
import StringEditor from '@components/inputs/StringEditor';
import CommandProfileDropDown from '@components/profiles/CommandProfileDropDown';
import { CreateSMChannelRequest, CreateSMStreamRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';
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
  const [request, setRequest] = useState<CreateSMStreamRequest>({
    CommandProfileName: 'Default',
    Logo: '/images/streammaster_logo.png'
  } as CreateSMStreamRequest);
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
        EPGID: 'Dummy',
        Group: smStreamDto.Group,
        Logo: smStreamDto.Logo,
        Name: smStreamDto.Name,
        Url: smStreamDto.Url,
        CommandProfileName: smStreamDto.CommandProfileName
      });
    }
  }, [orig, smStreamDto]);

  const updateStateAndRequest = useCallback(
    (updatedFields: Partial<SMStreamDto>) => {
      if (smStreamDto !== undefined) {
        const updatedDto = { ...smStreamDto, ...updatedFields };
        const updatedRequest = { ...request, Id: updatedDto.Id, ...updatedFields };

        setRequest(updatedRequest);
        return;
      }

      const updatedRequest = { ...request, ...updatedFields };

      setRequest(updatedRequest);
    },
    [request, smStreamDto]
  );

  const isSaveEnabled = useMemo(() => {
    if (orig === null) {
      if (!request.Name || request.Name === '' || !request?.Url || request.Url === '') {
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

    if (request.EPGID !== orig.EPGID) {
      return true;
    }

    if (request.Url !== orig.Url) {
      return true;
    }

    if (request.Logo !== orig.Logo) {
      return true;
    }

    if (request.CommandProfileName !== orig.CommandProfileName) {
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
      <div className="sm-border-square-top sm-headerBg pt-2">
        <div className="flex w-12 gap-1 pl-2 ">
          <div className="flex flex-column w-9 gap-1 pr-3 ">
            <div className="flex w-12 gap-1">
              <div className="w-6">
                <StringEditor
                  autoFocus
                  label="Name"
                  placeholder="Name"
                  darkBackGround
                  disableDebounce
                  onChange={(e) => {
                    e !== undefined && updateStateAndRequest({ Name: e });
                  }}
                  // onSave={(e) => {
                  //   e !== undefined && updateStateAndRequest({ Name: e });
                  // }}
                  value={request.Name}
                />
              </div>
              <div className="w-6">
                <EPGSelector value={request.EPGID} buttonDarkBackground label="EPG" onChange={(e) => e !== undefined && updateStateAndRequest({ EPGID: e })} />
              </div>
            </div>
            <div className="flex w-12 gap-1">
              <div className="sm-w-3">
                <SMChannelGroupDropDown
                  darkBackGround
                  label="Group"
                  onChange={(e) => {
                    e !== undefined && updateStateAndRequest({ Group: e });
                  }}
                  value={request.Group}
                />
              </div>

              <div className="w-9">
                <StringEditor
                  disableDebounce
                  label="URL"
                  darkBackGround
                  onChange={(e) => e !== undefined && updateStateAndRequest({ Url: e })}
                  value={request.Url}
                />
              </div>
            </div>
            <div className="sm-w-3">
              <CommandProfileDropDown
                buttonDarkBackground
                value={request.CommandProfileName ?? ''}
                onChange={(e) => e !== undefined && updateStateAndRequest({ CommandProfileName: e.ProfileName })}
              />
            </div>
          </div>
          <div className="w-3 flex flex-column justify-content-start align-items-center ">
            <div className="w-9">
              <NumberEditor
                disableDebounce
                label="Channel #"
                showButtons
                darkBackGround
                onChange={(e) => e !== undefined && updateStateAndRequest({ ChannelNumber: e })}
                onSave={(e) => e !== undefined && updateStateAndRequest({ ChannelNumber: e })}
                value={request.ChannelNumber}
              />
            </div>

            <IconSelector
              darkBackGround
              label="Logo"
              large
              enableEditMode
              onChange={(e) => e !== undefined && updateStateAndRequest({ Logo: e })}
              value={request.Logo}
            />
          </div>
        </div>
        <div className="layout-padding-bottom-lg sm-headerBg" />
      </div>
      <div className="layout-padding-bottom-lg sm-bgColor" />
    </>
  );
});

SMStreamDialog.displayName = 'SMStreamDialog';

export default React.memo(SMStreamDialog);
