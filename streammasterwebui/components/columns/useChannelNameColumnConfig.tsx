import ChannelNameEditor from '../ChannelNameEditor';
import createMultiSelectColumnConfigHook from './createVideoStreamMultiSelectColumnConfigHook';

export const useChannelNameColumnConfig = createMultiSelectColumnConfigHook({
  EditorComponent: ChannelNameEditor,
  dataField: 'user_Tvg_name',
  headerTitle: 'Name'
});
