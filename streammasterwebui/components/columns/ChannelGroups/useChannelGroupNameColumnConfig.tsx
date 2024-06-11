import ChannelGroupNameEditor from '@components/channelGroups/ChannelGroupNameEditor';
import createChannelGroupMultiSelectColumnConfigHook from './createChannelGroupMultiSelectColumnConfigHook';

export const useChannelGroupNameColumnConfig = createChannelGroupMultiSelectColumnConfigHook({
  dataField: 'Name',
  EditorComponent: ChannelGroupNameEditor,
  headerTitle: 'Name',
  sortable: true,
  width: '11rem'
});
