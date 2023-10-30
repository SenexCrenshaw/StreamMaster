import { useChannelGroupsGetChannelGroupNamesQuery } from '@lib/iptvApi';
import ChannelGroupEditor from '../channelGroups/ChannelGroupEditor';
import createMultiSelectColumnConfigHook from './createMultiSelectColumnConfigHook';

export const useChannelGroupColumnConfig = createMultiSelectColumnConfigHook({
  EditorComponent: ChannelGroupEditor,
  dataField: 'user_Tvg_group',
  headerTitle: 'Group',
  queryHook: useChannelGroupsGetChannelGroupNamesQuery,
  width: 10
});
