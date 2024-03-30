import IconSelector from '@components/icons/IconSelector';
import { SetSMChannelLogo } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelLogoRequest } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import { memo, useState } from 'react';

export interface StreamDataSelectorProperties {
  readonly data: SMChannelDto;
  readonly enableEditMode?: boolean;
}

const SMChannelLogoEditor = ({ data, enableEditMode }: StreamDataSelectorProperties) => {
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const onSetLogo = (Logo: string) => {
    if (data.id === 0) {
      return;
    }
    setIsLoading(true);
    const request = { smChannelId: data.id, logo: Logo } as SetSMChannelLogoRequest;

    SetSMChannelLogo(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  if (isLoading) {
    return <ProgressSpinner className="iconselector" />;
  }

  return (
    <IconSelector
      enableEditMode={enableEditMode || enableEditMode === undefined}
      onChange={async (e: string) => {
        await onSetLogo(e);
      }}
      value={data.logo}
    />
  );
};

SMChannelLogoEditor.displayName = 'Logo Editor';

export default memo(SMChannelLogoEditor);
