import { isEmptyObject } from '@lib/common/common';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { SetSMChannelGroup } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { ChannelGroupDto, SMChannelDto, SetSMChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { ReactNode, memo, useCallback, useMemo, useRef, useState } from 'react';

import SMDropDown, { SMDropDownRef } from '@components/sm/SMDropDown';

interface SMChannelGroupEditorProperties {
  readonly smChannelDto: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
}

const SMChannelGroupEditor = ({ darkBackGround, smChannelDto, onChange }: SMChannelGroupEditorProperties) => {
  const smDropownRef = useRef<SMDropDownRef>(null);
  const [isSaving, setIsSaving] = useState<boolean>(false);
  const { data } = useGetChannelGroups();

  const handleOnChange = useCallback(
    async (newGroup: string) => {
      if (isEmptyObject(smChannelDto)) {
        return;
      }
      setIsSaving(true);
      const request: SetSMChannelGroupRequest = {
        Group: newGroup,
        SMChannelId: smChannelDto.Id
      };

      await SetSMChannelGroup(request).finally(() => {
        setIsSaving(false);
      });
    },
    [smChannelDto]
  );

  const itemTemplate = (option: ChannelGroupDto) => {
    if (option === undefined) {
      return null;
    }

    return <span>{option.Name}</span>;
  };

  const buttonTemplate = useMemo((): ReactNode => {
    if (!smChannelDto) {
      return <div className="text-xs text-container text-white-alpha-40">None</div>;
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{smChannelDto.Group}</div>
      </div>
    );
  }, [smChannelDto]);

  return (
    <SMDropDown
      buttonLabel="GROUP"
      buttonDarkBackground={darkBackGround}
      buttonTemplate={buttonTemplate}
      data={data}
      dataKey="Group"
      filter
      filterBy="Name"
      isLoading={isSaving}
      itemTemplate={itemTemplate}
      onChange={(e) => {
        handleOnChange(e.Name);
      }}
      ref={smDropownRef}
      title="GROUP"
      value={smChannelDto}
      optionValue="Name"
      contentWidthSize="2"
    />
  );
};

export default memo(SMChannelGroupEditor);
