import EPGSelector from '@components/epg/EPGSelector';
import IconSelector from '@components/icons/IconSelector';
import NumberEditor from '@components/inputs/NumberEditor';
import SMChannelGroupDropDown from '@components/inputs/SMChannelGroupDropDown';
import StringEditor from '@components/inputs/StringEditor';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import useGetStationChannelNames from '@lib/smAPI/SchedulesDirect/useGetStationChannelNames';
import { SMChannelDto, SMStreamDto, StationChannelName, UpdateSMChannelRequest } from '@lib/smAPI/smapiTypes';
import React, { forwardRef, Suspense, useCallback, useEffect, useImperativeHandle, useMemo, useState } from 'react';
import StreamingProxyTypeSelector from './CommandProfileNameSelector';

const SMChannelSMStreamDialog = React.lazy(() => import('./SMChannelSMStreamDialog'));
const SMChannelSMStreamFromDataKeyDialog = React.lazy(() => import('./SMChannelSMStreamFromDataKeyDialog'));
interface SMChannelDialogProperties {
  readonly dataKey: string;
  readonly onSave: (request: UpdateSMChannelRequest) => void;
  readonly onSaveEnabled?: (saveEnabled: boolean) => void;
  readonly smChannel?: SMChannelDto;
}

export interface SMChannelDialogRef {
  save: () => void;
  reset: () => void;
}

const SMChannelDialog = forwardRef<SMChannelDialogRef, SMChannelDialogProperties>(({ dataKey, onSave, onSaveEnabled, smChannel }, ref) => {
  const { selectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const [tempSMChannel, setTempSMChannel] = useState<SMChannelDto | undefined>(undefined);
  const query = useGetStationChannelNames();
  const [request, setRequest] = useState<UpdateSMChannelRequest>({} as UpdateSMChannelRequest);
  const [stationChannelName, setStationChannelName] = useState<StationChannelName | undefined>(undefined);

  useEffect(() => {
    if (smChannel && smChannel.Id !== request.Id) {
      setTempSMChannel(smChannel);
      setRequest({ ...smChannel });
    }
  }, [smChannel, request]);

  const isSaveEnabled = useMemo(() => {
    if (!smChannel) {
      return Boolean(request.Name && request.Name !== '');
    }
    return (
      request.Name !== smChannel.Name ||
      request.Logo !== smChannel.Logo ||
      request.Group !== smChannel.Group ||
      request.CommandProfileName !== smChannel.CommandProfileName ||
      // request.ClientUserAgent !== smChannel.cl ||
      request.ChannelNumber !== smChannel.ChannelNumber ||
      request.EPGId !== smChannel.EPGId
    );
  }, [request.ChannelNumber, request.EPGId, request.Group, request.Logo, request.Name, request.CommandProfileName, smChannel]);

  const doSave = useCallback(() => {
    if (!isSaveEnabled) {
      return;
    }

    if (selectedItems.length > 0) {
      request.SMStreamsIds = selectedItems.map((e) => e.Id);
    }
    onSave(request);
  }, [isSaveEnabled, onSave, request, selectedItems]);

  const setName = useCallback(
    (value: string) => {
      if (request.Name !== value) {
        setRequest((prevRequest) => ({ ...prevRequest, Name: value }));
      }
    },
    [request.Name]
  );

  const setClientUserAgent = useCallback(
    (value: string) => {
      if (request.Name !== value) {
        setRequest((prevRequest) => ({ ...prevRequest, ClientUserAgent: value }));
      }
    },
    [request.Name]
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

  const setEPGId = useCallback(
    (value: string) => {
      if (request.EPGId !== value) {
        const newTempSMChannel = tempSMChannel ? { ...tempSMChannel, EPGId: value } : ({ EPGId: value } as SMChannelDto);
        setTempSMChannel(newTempSMChannel);

        const station = query.data?.find((e) => e.Id === value);
        if (station) {
          setStationChannelName(station);
        }

        setRequest((prevRequest) => ({ ...prevRequest, EPGId: value }));
      }
    },
    [query.data, request.EPGId, tempSMChannel]
  );

  const setCommandProfileName = useCallback(
    (value: string) => {
      if (request.CommandProfileName !== value) {
        const newTempSMChannel = tempSMChannel ? { ...tempSMChannel, CommandProfileName: value } : ({ CommandProfileName: value } as SMChannelDto);
        setTempSMChannel(newTempSMChannel);

        setRequest((prevRequest) => ({ ...prevRequest, CommandProfileName: value }));
      }
    },
    [request.CommandProfileName, tempSMChannel]
  );

  useEffect(() => {
    onSaveEnabled && onSaveEnabled(isSaveEnabled);
  }, [isSaveEnabled, onSaveEnabled]);

  useImperativeHandle(
    ref,
    () => ({
      reset: () => {
        if (smChannel) {
          setTempSMChannel(smChannel);
          setRequest({ ...smChannel });
        }
      },
      save: () => {
        doSave();
      }
    }),
    [doSave, smChannel]
  );

  return (
    <>
      <div className="sm-border-square-top sm-headerBg pt-2">
        <div className="flex w-12 gap-1 pl-2 ">
          <div className="flex flex-column w-9 gap-1 pr-3 ">
            <div className="flex w-12 gap-1">
              <div className="w-6 justify-content-start align-items-center">
                <StringEditor
                  autoFocus
                  label="Name"
                  placeholder="Name"
                  darkBackGround
                  disableDebounce
                  onChange={(e) => e !== undefined && setName(e)}
                  onSave={() => {
                    doSave();
                  }}
                  value={request.Name}
                />
              </div>
              <div className="w-6">
                <EPGSelector
                  value={stationChannelName?.Channel}
                  buttonDarkBackground
                  label="EPG"
                  smChannel={tempSMChannel}
                  onChange={(e) => e !== undefined && setEPGId(e)}
                />
              </div>
            </div>
            <div className="flex w-12 gap-1">
              <div className="w-6">
                <SMChannelGroupDropDown label="GROUP" darkBackGround value={request.Group} onChange={(e) => e !== undefined && setGroup(e)} />
              </div>
              <div className="w-6">
                <StreamingProxyTypeSelector darkBackGround label="Proxy" data={tempSMChannel} onChange={(e) => setCommandProfileName(e)} />
              </div>
            </div>
            <div className="flex w-12 gap-1">
              <div className="w-6 justify-content-start align-items-center">
                <StringEditor
                  autoFocus
                  label="Client User Agent"
                  darkBackGround
                  disableDebounce
                  onChange={(e) => e !== undefined && setClientUserAgent(e)}
                  onSave={() => {
                    doSave();
                  }}
                  value={request.ClientUserAgent}
                />
              </div>
            </div>
          </div>

          <div className="w-3 flex flex-column justify-content-start align-items-center ">
            <div className="w-9">
              <NumberEditor
                label="Channel #"
                showButtons
                disableDebounce
                darkBackGround
                onChange={(e) => e !== undefined && setChannelNumber(e)}
                value={request.ChannelNumber}
              />
            </div>
            <IconSelector darkBackGround label="Logo" large enableEditMode onChange={(e) => setLogo(e)} value={request.Logo} />
          </div>
        </div>

        <div className="layout-padding-bottom-lg sm-headerBg" />
      </div>
      <div className="layout-padding-bottom-lg sm-bgColor" />
      <Suspense>
        {smChannel !== null && smChannel !== undefined ? (
          <SMChannelSMStreamDialog name={request.Name} selectionKey={dataKey + '1'} smChannel={smChannel} />
        ) : (
          <SMChannelSMStreamFromDataKeyDialog dataKey={dataKey} name={request.Name} />
        )}
      </Suspense>
    </>
  );
});

SMChannelDialog.displayName = 'SMChannelDialog';

export default React.memo(SMChannelDialog);
