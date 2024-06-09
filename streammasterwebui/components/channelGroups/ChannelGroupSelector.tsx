import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';
import BaseChannelGroupSelector from './BaseChannelGroupSelector';

type ChannelGroupSelectorProperties = {
  readonly dataKey: string;
  readonly fixed?: boolean;
  readonly enableEditMode?: boolean;
  readonly useSelectedItemsFilter?: boolean;
  readonly label?: string;
  readonly value?: string;
  readonly width?: string;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
};

const ChannelGroupSelector = (props: ChannelGroupSelectorProperties) => {
  return <BaseChannelGroupSelector {...props} getNamesQuery={useGetChannelGroups} />;
};

ChannelGroupSelector.displayName = 'ChannelGroupSelector';
export default memo(ChannelGroupSelector);
