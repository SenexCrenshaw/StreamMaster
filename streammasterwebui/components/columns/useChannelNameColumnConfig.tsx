import ChannelNameEditor from '../ChannelNameEditor';
import createMultiSelectColumnConfigHook from './createMultiSelectColumnConfigHook';

export const useChannelNameColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'user_Tvg_name',
  EditorComponent: ChannelNameEditor,
  headerTitle: 'Name',
});
