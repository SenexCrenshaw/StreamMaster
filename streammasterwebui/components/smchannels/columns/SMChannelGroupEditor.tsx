import { SetSMChannelGroupRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useState } from 'react';

import SMChannelGroupDropDown from '../../inputs/SMChannelGroupDropDown';
import useIsCellLoading from '@lib/redux/hooks/useIsCellLoading';
import { isEmptyObject } from '@lib/common/common';
import { SetSMChannelGroup } from '@lib/smAPI/SMChannels/SMChannelsCommands';

interface SMChannelGroupEditorProperties {
  readonly smChannelDto?: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly autoPlacement?: boolean;
  readonly onChange?: (value: string) => void;
}

const SMChannelGroupEditor = ({ darkBackGround, autoPlacement = false, smChannelDto, onChange }: SMChannelGroupEditorProperties) => {
  const [group, setGroup] = useState<string>('Dummy');
  const [isCellLoading, setIsCellLoading] = useIsCellLoading({
    Entity: 'SMChannel',
    Field: 'Group',
    Id: smChannelDto?.Id.toString() ?? 'Dummy'
  });

  const handleOnChange = useCallback(
    async (newGroup: string) => {
      setGroup(newGroup);
      if (smChannelDto === undefined || smChannelDto.Id === undefined || isEmptyObject(smChannelDto)) {
        return;
      }
      setIsCellLoading(true);
      const request: SetSMChannelGroupRequest = {
        Group: newGroup,
        SMChannelId: smChannelDto.Id
      };

      await SetSMChannelGroup(request).finally(() => {
        setIsCellLoading(false);
      });
    },
    [smChannelDto, setIsCellLoading]
  );

  return (
    <SMChannelGroupDropDown
      autoPlacement={autoPlacement}
      darkBackGround={darkBackGround}
      smChannelDto={smChannelDto}
      isLoading={isCellLoading}
      onChange={(e) => {
        handleOnChange(e);
        onChange?.(e);
      }}
      value={smChannelDto?.Group || group}
    />
  );
};

export default memo(SMChannelGroupEditor);
