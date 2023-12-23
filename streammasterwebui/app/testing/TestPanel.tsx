import ChannelLogoEditor from '@components/ChannelLogoEditor';
import { VideoStreamDto, useVideoStreamsGetVideoStreamQuery } from '@lib/iptvApi';
import { memo } from 'react';

export interface StreamDataSelectorProperties {
  readonly data: VideoStreamDto;
  readonly enableEditMode?: boolean;
}

const TestPanel = () => {
  const videoStreamsGetVideoStreamQuery = useVideoStreamsGetVideoStreamQuery('3c8ef639142e3ffc5dfe169c9dd70979');
  // const { columnConfig: channelLogoColumnConfig } = useChannelLogoColumnConfig({ enableEdit: false });
  if (videoStreamsGetVideoStreamQuery.data === undefined) return <div>loading</div>;

  return <ChannelLogoEditor enableEditMode={false} data={videoStreamsGetVideoStreamQuery.data} />;
};
export default memo(TestPanel);
