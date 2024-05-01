import SMChannelGroupEditor from './SMChannel/SMChannelGroupEditor';
import createMultiSelectColumnConfigHook from './createVideoStreamMultiSelectColumnConfigHook';

export const useChannelGroupColumnConfig = createMultiSelectColumnConfigHook({
  EditorComponent: SMChannelGroupEditor,
  dataField: 'user_Tvg_group',
  headerTitle: 'Group',
  // queryHook: useChannelGroupsGetChannelGroupNamesQuery,
  width: 10
});
