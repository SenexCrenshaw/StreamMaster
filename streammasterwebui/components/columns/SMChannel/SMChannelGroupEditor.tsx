import { ChannelGroupDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';
import ChannelGroupSelector from '../../channelGroups/ChannelGroupSelector';

interface SMChannelGroupEditorProperties {
  readonly data: SMChannelDto;
  readonly tableDataKey: string;
  readonly useSelectedItemsFilter?: boolean;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
}

const SMChannelGroupEditor = ({ data, tableDataKey, onChange, useSelectedItemsFilter }: SMChannelGroupEditorProperties) => {
  const dataKey = tableDataKey + '-SMChannelGroupEditor';

  return (
    <ChannelGroupSelector dataKey={dataKey} onChange={async (e: ChannelGroupDto[]) => {}} useSelectedItemsFilter={useSelectedItemsFilter} value={data.Group} />
  );
};

export default memo(SMChannelGroupEditor);
