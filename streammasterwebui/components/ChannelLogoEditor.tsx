import { memo } from 'react';

import { UpdateVideoStreamRequest, VideoStreamDto } from '@lib/iptvApi';
import { UpdateVideoStream } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import IconSelector from './selectors/IconSelector';

export interface StreamDataSelectorProperties {
  readonly data: VideoStreamDto;
  readonly enableEditMode?: boolean;
}

const ChannelLogoEditor = ({ data, enableEditMode }: StreamDataSelectorProperties) => {
  const onUpdateVideoStream = async (Logo: string) => {
    if (data.id === '') {
      return;
    }

    const request: UpdateVideoStreamRequest = {};

    request.id = data.id;

    if (Logo && Logo !== '' && data.user_Tvg_logo !== Logo) {
      request.tvg_logo = Logo;
    }

    await UpdateVideoStream(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      });
  };

  return (
    <IconSelector
      className="p-inputtext-sm"
      enableEditMode={enableEditMode || enableEditMode === undefined}
      onChange={async (e: string) => {
        await onUpdateVideoStream(e);
      }}
      value={data.user_Tvg_logo}
    />
  );
};

ChannelLogoEditor.displayName = 'Logo Editor';

export default memo(ChannelLogoEditor);
