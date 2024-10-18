import IconSelector from '@components/icons/IconSelector';
import useIsCellLoading from '@lib/redux/hooks/useIsCellLoading';
import { SetSMChannelLogo } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelLogoRequest } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';

export interface StreamDataSelectorProperties {
  readonly data: SMChannelDto;
}

const SMChannelLogoEditor = ({ data }: StreamDataSelectorProperties) => {
  const [isCellLoading, setIsCellLoading] = useIsCellLoading({
    Entity: 'SMChannel',
    Field: 'Logo',
    Id: data.Id.toString()
  });

  const onSetLogo = (Logo: string) => {
    if (data.Id === 0) {
      return;
    }
    setIsCellLoading(true);
    const request = { Logo: Logo, SMChannelId: data.Id } as SetSMChannelLogoRequest;

    SetSMChannelLogo(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
        throw error;
      })
      .finally(() => {
        setIsCellLoading(false);
      });
  };

  return (
    <IconSelector
      enableEditMode={data.IsSystem !== true}
      isLoading={isCellLoading}
      onChange={async (e: string) => {
        onSetLogo(e);
      }}
      isCustomPlayList={data.IsSystem}
      value={data.Logo}
    />
  );
};

SMChannelLogoEditor.displayName = 'Logo Editor';

export default memo(SMChannelLogoEditor);
