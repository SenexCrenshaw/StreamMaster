import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';
import BaseChannelGroupPagedSelector from './BaseChannelGroupPagedSelector';

type ChannelGroupSelectorProperties = {
  readonly dataKey: string;
  readonly autoPlacement?: boolean;
  readonly enableEditMode?: boolean;
  readonly useSelectedItemsFilter?: boolean;
  readonly label?: string;
  readonly value?: string;
  readonly width?: string;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
};

const ChannelGroupSelector = (props: ChannelGroupSelectorProperties) => {
  return <BaseChannelGroupPagedSelector {...props} getNamesQuery={useGetChannelGroups} />;
};

ChannelGroupSelector.displayName = 'ChannelGroupSelector';
export default memo(ChannelGroupSelector);
