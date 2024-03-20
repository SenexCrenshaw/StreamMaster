import { Suspense, lazy, memo, useState } from 'react';

import { SMChannelDto, SMChannelLogoRequest } from '@lib/apiDefs';
import { SetSMChannelLogo } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { ProgressSpinner } from 'primereact/progressspinner';

export interface StreamDataSelectorProperties {
  readonly data: SMChannelDto;
  readonly enableEditMode?: boolean;
}

const SMChannelLogoEditor = ({ data, enableEditMode }: StreamDataSelectorProperties) => {
  const IconSelector = lazy(() => import('../selectors/IconSelector'));
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const onSetLogo = (Logo: string) => {
    if (data.id === 0) {
      return;
    }
    setIsLoading(true);
    const request = { smChannelId: data.id, logo: Logo } as SMChannelLogoRequest;

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
    <Suspense fallback={<div></div>}>
      <IconSelector
        enableEditMode={enableEditMode || enableEditMode === undefined}
        onChange={async (e: string) => {
          await onSetLogo(e);
        }}
        value={data.logo}
      />
    </Suspense>
  );
};

SMChannelLogoEditor.displayName = 'Logo Editor';

export default memo(SMChannelLogoEditor);
