import { isEmptyObject } from '@lib/common/common';
import { SetSMChannelGroup } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { ChannelGroupDto, SMChannelDto, SetSMChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback } from 'react';

import SMChannelGroupDropDown from '../../inputs/SMChannelGroupDropDown';

interface SMChannelGroupEditorProperties {
  readonly smChannelDto: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly fixed?: boolean;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
}

const SMChannelGroupEditor = ({ darkBackGround, fixed = false, smChannelDto, onChange }: SMChannelGroupEditorProperties) => {
  const handleOnChange = useCallback(
    async (newGroup: string) => {
      if (isEmptyObject(smChannelDto)) {
        return;
      }
      // setIsSaving(true);
      const request: SetSMChannelGroupRequest = {
        Group: newGroup,
        SMChannelId: smChannelDto.Id
      };

      await SetSMChannelGroup(request).finally(() => {
        // setIsSaving(false);
      });
    },
    [smChannelDto]
  );

  return <SMChannelGroupDropDown fixed={fixed} darkBackGround={darkBackGround} value={smChannelDto.Group} onChange={async (e) => handleOnChange(e)} />;
};

export default memo(SMChannelGroupEditor);
