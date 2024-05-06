import OKButton from '@components/buttons/OKButton';
import ChannelGroupSelector from '@components/channelGroups/ChannelGroupSelector';
import EPGSelector from '@components/epg/EPGSelector';
import IconSelector from '@components/icons/IconSelector';
import NumberEditor from '@components/inputs/NumberEditor';
import StringEditor from '@components/inputs/StringEditor';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import useGetStationChannelNames from '@lib/smAPI/SchedulesDirect/useGetStationChannelNames';
import { CreateSMChannelRequest, StationChannelName } from '@lib/smAPI/smapiTypes';

import React, { useCallback, useRef, useState } from 'react';
import StreamingProxyTypeSelector from './StreamingProxyTypeSelector';

interface SMChannelDialogProperties {}

const SMChannelDialog = () => {
  const smDialogRef = useRef<SMDialogRef>(null);
  const query = useGetStationChannelNames();
  const [request, setRequest] = React.useState<CreateSMChannelRequest>({ Logo: '/images/StreamMaster.png' } as CreateSMChannelRequest);
  const [, setStationChannelName] = useState<StationChannelName | undefined>(undefined);
  const ReturnToParent = React.useCallback(() => {}, []);

  const onSave = React.useCallback(async () => {}, []);

  const setName = useCallback(
    (value: string) => {
      if (request.Name !== value) {
        request.Name = value;
        setRequest(request);
      }
    },
    [request]
  );

  const setLogo = useCallback(
    (value: string) => {
      if (request.Logo !== value) {
        request.Logo = value;
        setRequest(request);
      }
    },
    [request]
  );

  const setGroup = useCallback(
    (value: string) => {
      if (request.Group !== value) {
        request.Group = value;
        setRequest(request);
      }
    },
    [request]
  );

  const setChannelNumber = useCallback(
    (value: number) => {
      if (request.ChannelNumber !== value) {
        request.ChannelNumber = value;
        setRequest(request);
      }
    },
    [request]
  );
  const setEPGId = useCallback(
    (value: string) => {
      if (request.EPGId !== value) {
        request.EPGId = value;
        if (query.data) {
          const station = query.data.find((e) => e.Id === value);
          if (station) {
            setStationChannelName(station);
          }
        }
        setRequest(request);
      }
    },
    [query.data, request]
  );

  return (
    <SMDialog
      ref={smDialogRef}
      position="top"
      title="CREATE CHANNEL"
      onHide={() => ReturnToParent()}
      buttonClassName="icon-green-filled"
      icon="pi-plus"
      widthSize={4}
      info="General"
      tooltip="Create Channel"
    >
      <div className="flex w-12 gap-2">
        <div className="flex flex-column w-10 gap-2">
          <div className="flex w-12 gap-2">
            <div className="w-6 justify-content-start align-items-center">
              <StringEditor label="Name" darkBackGround disableDebounce onChange={(e) => e && setName(e)} onSave={(e) => {}} value={request.Name} />
            </div>
            <div className="w-6 justify-content-start align-items-center">
              <label>EPG</label>
              <div className="pt-small" />
              <span className="flex align-items-center input-border-dark">
                <EPGSelector value={request.EPGId} onChange={(e) => e && setEPGId(e)} />
              </span>
            </div>
          </div>
          <div className="flex w-12 gap-2">
            <div className="w-6 justify-content-start align-items-center">
              <label>GROUP</label>
              <div className="pt-small" />
              <ChannelGroupSelector onChange={(e) => e && setGroup(e)} value={request.Group} />
            </div>
            <div className="w-6 justify-content-start align-items-center ">
              <NumberEditor darkBackGround label="Channel #" onChange={(e) => e && setChannelNumber(e)} value={request.ChannelNumber} />
            </div>
          </div>
        </div>

        <div className="w-2 flex flex-column justify-content-start align-items-center">
          <div>Logo</div>
          <div className=" flex flex-column justify-content-center align-items-center w-full h-full">
            <IconSelector
              large
              enableEditMode
              onChange={async (e: string) => {
                setLogo(e);
              }}
              value={request.Logo}
            />
          </div>
        </div>
      </div>
      <div className="layout-padding-bottom" />
      <div className="flex w-12 gap-2">
        <div className="w-6 gap-2 w-full h-full">
          <label>Proxy</label>
          <div className="pt-small" />
          <StreamingProxyTypeSelector onChange={(e) => console.log(e)} />
        </div>
      </div>
      <div className="layout-padding-bottom-lg" />

      <div className="layout-padding-bottom-lg surface-ground" />
      <div className="flex w-12 gap-2 input-border">
        <div className="w-6 gap-2 w-full h-full">
          <label>Proxy</label>
        </div>
      </div>
      <div className="layout-padding-bottom-lg surface-ground" />
      <div className="flex col-12 gap-2 mt-4 justify-content-center ">
        <OKButton onClick={async () => await onSave()} />
      </div>
    </SMDialog>
  );
};

SMChannelDialog.displayName = 'SMChannelDialog';

export default React.memo(SMChannelDialog);
