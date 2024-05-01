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

interface CopySMChannelProperties {}

const CreateSMChannelDialog = () => {
  const smDialogRef = useRef<SMDialogRef>(null);
  const query = useGetStationChannelNames();
  const [request, setRequest] = React.useState<CreateSMChannelRequest>({} as CreateSMChannelRequest);
  const [stationChannelName, setStationChannelName] = useState<StationChannelName | undefined>(undefined);
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
      <div className="flex w-12 border-1 gap-2">
        <div className="flex flex-column w-10 gap-2">
          <div className="flex w-12 gap-2">
            <div className="w-6 justify-content-start align-items-center">
              <StringEditor label="Name" darkBackGround disableDebounce onChange={(e) => e && setName(e)} onSave={(e) => {}} value={request.Name} />
            </div>
            <div className="w-6 justify-content-start align-items-center">
              <label>EPG</label>
              <span className="flex align-items-center input-border-dark">
                <EPGSelector value={request.EPGId} onChange={(e) => e && setEPGId(e)} />
              </span>
            </div>
          </div>
          <div className="flex w-12 gap-2">
            <div className="w-6 justify-content-start align-items-center">
              <label>GROUP</label>
              <ChannelGroupSelector className="input-border-dark" onChange={(e) => e && setGroup(e)} value={request.Group} />
            </div>
            <div className="w-6 justify-content-start align-items-center border-1">
              <NumberEditor darkBackGround label="Channel #" onChange={(e) => e && setChannelNumber(e)} value={request.ChannelNumber} />
            </div>
          </div>
        </div>

        <div className="w-2 border-1 flex flex-column justify-content-start align-items-center">
          <div>Logo</div>
          <div className="border-1 flex flex-column justify-content-center align-items-center w-full h-full">
            <IconSelector
              large
              enableEditMode
              onChange={async (e: string) => {
                console.log(e);
              }}
              value={request.Logo}
            />
          </div>
        </div>
      </div>
      <div className="layout-padding-bottom-lg" />
      <div className="flex w-12 border-1 gap-2 input-border">
        <div className="flex w-6 gap-2"></div>
      </div>
      <div className="flex col-12 gap-2 mt-4 justify-content-center ">
        <OKButton onClick={async () => await onSave()} />
      </div>
      <div className="layout-padding-bottom-lg" />
    </SMDialog>
  );
};

CreateSMChannelDialog.displayName = 'CreateSMChannelDialog';

export default React.memo(CreateSMChannelDialog);
