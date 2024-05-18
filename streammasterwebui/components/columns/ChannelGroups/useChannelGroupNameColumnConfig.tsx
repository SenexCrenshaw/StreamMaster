import createChannelGroupMultiSelectColumnConfigHook from '../createChannelGroupMultiSelectColumnConfigHook';
import ChannelGroupNameEditor from '@components/channelGroups/ChannelGroupNameEditor';

export const useChannelGroupNameColumnConfig = createChannelGroupMultiSelectColumnConfigHook({
  dataField: 'Name',
  EditorComponent: ChannelGroupNameEditor,
  headerTitle: 'Name',
  sortable: true
});
