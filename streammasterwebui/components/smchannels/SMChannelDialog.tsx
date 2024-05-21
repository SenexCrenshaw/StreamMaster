import OKButton from '@components/buttons/OKButton';
import ChannelGroupSelector from '@components/channelGroups/ChannelGroupSelector';
import EPGSelector from '@components/epg/EPGSelector';
import IconSelector from '@components/icons/IconSelector';
import NumberEditor from '@components/inputs/NumberEditor';
import StringEditor from '@components/inputs/StringEditor';
import useGetStationChannelNames from '@lib/smAPI/SchedulesDirect/useGetStationChannelNames';
import { CreateSMChannelRequest, SMChannelDto, SMStreamDto, StationChannelName } from '@lib/smAPI/smapiTypes';

import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import React, { useCallback, useEffect, useMemo, useState } from 'react';

import SMChannelSMStreamDialog from './SMChannelSMStreamDialog';
import StreamingProxyTypeSelector from './StreamingProxyTypeSelector';

interface SMChannelDialogProperties {
  onSave: (request: CreateSMChannelRequest) => void;
  readonly smChannel?: SMChannelDto;
}

const SMChannelDialog = ({ smChannel, onSave }: SMChannelDialogProperties) => {
  const dataKey = 'SMChannelSMStreamDialog-SMStreamDataForSMChannelSelector';
  const { selectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const [tempSMChannel, setTempSMChannel] = useState<SMChannelDto | undefined>(undefined);

  const query = useGetStationChannelNames();
  const [request, setRequest] = React.useState<CreateSMChannelRequest>({} as CreateSMChannelRequest);
  const [, setStationChannelName] = useState<StationChannelName | undefined>(undefined);

  useEffect(() => {
    if (smChannel && smChannel.Name !== request.Name) {
      setTempSMChannel(smChannel);
      setRequest({ ...smChannel });
    } else {
      // if (request.Logo === undefined || request.Logo === '') {
      //   setRequest({ Logo: '/images/StreamMaster.png' } as CreateSMChannelRequest);
      // }
    }
  }, [request.Logo, request.Name, smChannel]);

  const doSave = React.useCallback(() => {
    if (selectedItems.length > 0) {
      request.SMStreamsIds = selectedItems.map((e) => e.Id);
    }
    onSave(request);
  }, [onSave, request, selectedItems]);

  const setName = useCallback(
    (value: string) => {
      if (request.Name !== value) {
        request.Name = value;
        setRequest({ ...request });
      }
    },
    [request]
  );

  const setLogo = useCallback(
    (value: string) => {
      if (request.Logo !== value) {
        request.Logo = value;
        setRequest({ ...request });
      }
    },
    [request]
  );

  const setGroup = useCallback(
    (value: string) => {
      if (request.Group !== value) {
        request.Group = value;
        setRequest({ ...request });
      }
    },
    [request]
  );

  const setChannelNumber = useCallback(
    (value: number) => {
      if (request.ChannelNumber !== value) {
        request.ChannelNumber = value;
        setRequest({ ...request });
      }
    },
    [request]
  );

  const setEPGId = useCallback(
    (value: string) => {
      if (request.EPGId !== value) {
        request.EPGId = value;

        if (!tempSMChannel) {
          const newTempSMChannel = { EPGId: value } as SMChannelDto;
          setTempSMChannel(newTempSMChannel);
        } else {
          const newTempSMChannel = { ...tempSMChannel, EPGId: value };
          setTempSMChannel(newTempSMChannel);
        }

        if (query.data) {
          const station = query.data.find((e) => e.Id === value);
          if (station) {
            setStationChannelName(station);
          }
        }
        setRequest(request);
      }
    },
    [query.data, request, tempSMChannel]
  );

  const setProxy = useCallback(
    (value: number) => {
      if (request.StreamingProxyType !== value) {
        request.StreamingProxyType = value;
        setRequest({ ...request });
      }
    },
    [request]
  );

  const isSaveEnabled = useMemo(() => {
    if (!smChannel) {
      if (request.Name && request.Name !== '') {
        return true;
      }
    }

    if (request.Name && request.Name !== smChannel?.Name) {
      return true;
    }

    if (request.Logo && request.Logo !== smChannel?.Logo) {
      return true;
    }

    if (request.Group && request.Group !== smChannel?.Group) {
      return true;
    }

    if (request.ChannelNumber && request.ChannelNumber !== smChannel?.ChannelNumber) {
      return true;
    }

    if (request.EPGId && request.EPGId !== smChannel?.EPGId) {
      return true;
    }

    return false;
  }, [request.ChannelNumber, request.EPGId, request.Group, request.Logo, request.Name, smChannel]);

  return (
    <>
      <div className="sm-headerBg dialog-padding border-sides">
        <div className="flex w-12 gap-1 pl-2 ">
          <div className="flex flex-column w-10 gap-1 ">
            <div className="flex w-12 gap-1">
              <div className="w-6 justify-content-start align-items-center ">
                <StringEditor
                  label="Name"
                  placeholder="Name"
                  darkBackGround
                  disableDebounce
                  onChange={(e) => {
                    e && setName(e);
                  }}
                  onSave={(e) => {}}
                  value={request.Name}
                />
              </div>
              <div className="w-6 justify-content-start align-items-center">
                <EPGSelector label="EPG" darkBackGround smChannel={tempSMChannel} onChange={(e) => e && setEPGId(e)} />
              </div>
            </div>
            <div className="flex w-12 gap-1">
              <div className="w-6 justify-content-start align-items-center ">
                <ChannelGroupSelector
                  dataKey="SMChannelDialog"
                  label="GROUP"
                  darkBackGround
                  onChange={(e) => {
                    if (e) {
                      setGroup(e);
                    }
                  }}
                  value={request.Group}
                />
              </div>
              <div className="w-6 justify-content-start align-items-center ">
                <NumberEditor label="Channel #" showButtons darkBackGround onSave={(e) => e && setChannelNumber(e)} value={request.ChannelNumber} />
              </div>
            </div>
          </div>

          <div className="w-2 flex flex-column justify-content-start align-items-center">
            <IconSelector
              darkBackGround
              label="Logo"
              large
              enableEditMode
              onChange={async (e: string) => {
                setLogo(e);
              }}
              value={request.Logo}
            />
          </div>
        </div>
        <div className="layout-padding-bottom" />
        <div className="flex w-10 gap-1 pl-2">
          <div className="w-6 gap-1 w-full h-full">
            <StreamingProxyTypeSelector label="Proxy" smChannelDto={smChannel} onChange={(e) => setProxy(e)} />
          </div>
        </div>
      </div>
      <div className="layout-padding-bottom-lg sm-headerBg border-radius-bottom " />
      {/* <div className="layout-padding-bottom-lg surface-ground border-round-md" /> */}
      <div className="layout-padding-bottom-lg" />
      <div className="w-12">
        <SMChannelSMStreamDialog name={request.Name} smChannel={smChannel} />
      </div>
      {/* <div className="layout-padding-bottom-lg" />
      <div className="layout-padding-bottom-lg surface-ground border-round-md" />
      <div className="layout-padding-bottom-lg" /> */}
      <div className="flex col-12 gap-1 mt-1 justify-content-center ">
        <OKButton disabled={!isSaveEnabled} onClick={async () => doSave()} />
      </div>
    </>
  );
};

SMChannelDialog.displayName = 'SMChannelDialog';

export default React.memo(SMChannelDialog);
