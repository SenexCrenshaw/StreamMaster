import React, { useCallback, useEffect, useImperativeHandle, useMemo, useRef, useState, forwardRef } from 'react';
import OKButton from '@components/buttons/OKButton';
import ChannelGroupSelector from '@components/channelGroups/ChannelGroupSelector';
import EPGSelector from '@components/epg/EPGSelector';
import IconSelector from '@components/icons/IconSelector';
import NumberEditor from '@components/inputs/NumberEditor';
import StringEditor from '@components/inputs/StringEditor';
import { CreateSMChannelRequest, SMChannelDto, SMStreamDto, StationChannelName } from '@lib/smAPI/smapiTypes';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import SMChannelSMStreamDialog from './SMChannelSMStreamDialog';
import StreamingProxyTypeSelector from './StreamingProxyTypeSelector';
import useGetStationChannelNames from '@lib/smAPI/SchedulesDirect/useGetStationChannelNames';

interface SMChannelDialogProperties {
  onSave: (request: CreateSMChannelRequest) => void;
  readonly onSaveEnabled?: (saveEnabled: boolean) => void;
  readonly smChannel?: SMChannelDto;
}

export interface SMChannelDialogRef {
  save: () => void;
}

const SMChannelDialog = forwardRef<SMChannelDialogRef, SMChannelDialogProperties>(({ smChannel, onSaveEnabled, onSave }, ref) => {
  const dataKey = 'SMChannelSMStreamDialog-SMStreamDataForSMChannelSelector';
  const { selectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const [tempSMChannel, setTempSMChannel] = useState<SMChannelDto | undefined>(undefined);

  const query = useGetStationChannelNames();
  const [request, setRequest] = useState<CreateSMChannelRequest>({} as CreateSMChannelRequest);
  const [, setStationChannelName] = useState<StationChannelName | undefined>(undefined);

  useEffect(() => {
    if (smChannel && smChannel.Name !== request.Name) {
      setTempSMChannel(smChannel);
      setRequest({ ...smChannel });
    }
  }, [smChannel, request.Name]);

  const doSave = useCallback(() => {
    if (selectedItems.length > 0) {
      request.SMStreamsIds = selectedItems.map((e) => e.Id);
    }
    onSave(request);
  }, [onSave, request, selectedItems]);

  const setName = useCallback(
    (value: string) => {
      if (request.Name !== value) {
        setRequest((prevRequest) => ({ ...prevRequest, Name: value }));
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

  const setProxy = useCallback(
    (value: number) => {
      if (request.StreamingProxyType !== value) {
        setRequest((prevRequest) => ({ ...prevRequest, StreamingProxyType: value }));
      }
    },
    [request.StreamingProxyType]
  );

  useEffect(() => {
    if (!smChannel) {
      onSaveEnabled && onSaveEnabled(Boolean(request.Name && request.Name !== ''));
      return;
    }

    onSaveEnabled &&
      onSaveEnabled(
        request.Name !== smChannel.Name ||
          request.Logo !== smChannel.Logo ||
          request.Group !== smChannel.Group ||
          request.ChannelNumber !== smChannel.ChannelNumber ||
          request.EPGId !== smChannel.EPGId
      );
  }, [onSaveEnabled, request.ChannelNumber, request.EPGId, request.Group, request.Logo, request.Name, smChannel]);

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
          <div className="flex flex-column w-10 gap-1">
            <div className="flex w-12 gap-1">
              <div className="w-6 justify-content-start align-items-center">
                <StringEditor label="Name" placeholder="Name" darkBackGround disableDebounce onChange={(e) => e && setName(e)} value={request.Name} />
              </div>
              <div className="w-6 justify-content-start align-items-center">
                <EPGSelector label="EPG" darkBackGround smChannel={tempSMChannel} onChange={(e) => e && setEPGId(e)} />
              </div>
            </div>
            <div className="flex w-12 gap-1">
              <div className="w-6 justify-content-start align-items-center">
                <ChannelGroupSelector dataKey="SMChannelDialog" label="GROUP" darkBackGround onChange={(e) => e && setGroup(e)} value={request.Group} />
              </div>
              <div className="w-6 justify-content-start align-items-center">
                <NumberEditor label="Channel #" showButtons darkBackGround onChange={(e) => e && setChannelNumber(e)} value={request.ChannelNumber} />
              </div>
            </div>
          </div>

          <div className="w-2 flex flex-column justify-content-start align-items-center">
            <IconSelector darkBackGround label="Logo" large enableEditMode onChange={(e) => setLogo(e)} value={request.Logo} />
          </div>
        </div>
        <div className="layout-padding-bottom" />
        <div className="flex w-10 gap-1 pl-2">
          <div className="w-6 gap-1 w-full h-full">
            <StreamingProxyTypeSelector label="Proxy" data={smChannel} onChange={(e) => setProxy(e)} />
          </div>
        </div>
      </div>
      <div className="layout-padding-bottom-lg sm-headerBg border-radius-bottom" />
      <div className="layout-padding-bottom-lg" />
      <div className="w-12">
        <SMChannelSMStreamDialog name={request.Name} smChannel={smChannel} />
      </div>
    </>
  );
});

SMChannelDialog.displayName = 'SMChannelDialog';

export default React.memo(SMChannelDialog);
