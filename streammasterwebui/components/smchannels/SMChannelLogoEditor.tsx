import IconSelector from '@components/icons/IconSelector';
import { SetSMChannelLogo } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelLogoRequest } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';

export interface StreamDataSelectorProperties {
  readonly data: SMChannelDto;
  readonly enableEditMode?: boolean;
}

const SMChannelLogoEditor = ({ data, enableEditMode }: StreamDataSelectorProperties) => {
  const onSetLogo = (Logo: string) => {
    if (data.Id === 0) {
      return;
    }
    const request = { SMChannelId: data.Id, Logo: Logo } as SetSMChannelLogoRequest;

    SetSMChannelLogo(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
        throw error;
      })
      .finally(() => {});
  };

  return (
    <IconSelector
      enableEditMode={enableEditMode || enableEditMode === undefined}
      onChange={async (e: string) => {
        onSetLogo(e);
      }}
      value={data.Logo}
    />
  );
};

SMChannelLogoEditor.displayName = 'Logo Editor';

export default memo(SMChannelLogoEditor);
