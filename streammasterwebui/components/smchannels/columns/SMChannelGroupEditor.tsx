import { isEmptyObject } from '@lib/common/common';
import { SetSMChannelGroup } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { ChannelGroupDto, SMChannelDto, SetSMChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback } from 'react';

import useIsCellLoading from '@lib/redux/hooks/useIsCellLoading';
import SMChannelGroupDropDown from '../../inputs/SMChannelGroupDropDown';

interface SMChannelGroupEditorProperties {
  readonly smChannelDto: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly autoPlacement?: boolean;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
}

const SMChannelGroupEditor = ({ darkBackGround, autoPlacement = false, smChannelDto, onChange }: SMChannelGroupEditorProperties) => {
  const [isCellLoading, setIsCellLoading] = useIsCellLoading({
    Entity: 'SMChannel',
    Field: 'Group',
    Id: smChannelDto.Id.toString()
  });

  const handleOnChange = useCallback(
    async (newGroup: string) => {
      if (isEmptyObject(smChannelDto)) {
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
      isLoading={isCellLoading}
      smChannelDto={smChannelDto}
      onChange={async (e) => handleOnChange(e)}
    />
  );
};

export default memo(SMChannelGroupEditor);
