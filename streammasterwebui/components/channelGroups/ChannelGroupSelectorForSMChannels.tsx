import useGetChannelGroupsFromSMChannels from '@lib/smAPI/ChannelGroups/useGetChannelGroupsFromSMChannels';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';
import BaseChannelGroupSelector from './BaseChannelGroupSelector';

type ChannelGroupSelectorForSMChannelsProperties = {
  readonly dataKey: string;
  readonly fixed?: boolean;
  readonly enableEditMode?: boolean;
  readonly useSelectedItemsFilter?: boolean;
  readonly label?: string;
  readonly value?: string;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
};

const ChannelGroupSelectorForSMChannels = (props: ChannelGroupSelectorForSMChannelsProperties) => {
  return <BaseChannelGroupSelector {...props} getNamesQuery={useGetChannelGroupsFromSMChannels} />;
};

ChannelGroupSelectorForSMChannels.displayName = 'ChannelGroupSelectorForSMChannels';
export default memo(ChannelGroupSelectorForSMChannels);
