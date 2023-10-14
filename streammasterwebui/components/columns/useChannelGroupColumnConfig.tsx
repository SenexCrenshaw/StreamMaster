import { useChannelGroupsGetChannelGroupNamesQuery } from '@lib/iptvApi';
import ChannelGroupEditor from '../channelGroups/ChannelGroupEditor';
import createMultiSelectColumnConfigHook from './createMultiSelectColumnConfigHook';

export const useChannelGroupColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'user_Tvg_group',
  EditorComponent: ChannelGroupEditor,
  headerTitle: 'Group',
  queryHook: useChannelGroupsGetChannelGroupNamesQuery,
  width: 10,
});
