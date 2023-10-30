import ChannelLogoEditor from '../ChannelLogoEditor';
import createMultiSelectColumnConfigHook from './createMultiSelectColumnConfigHook';

export const useChannelLogoColumnConfig = createMultiSelectColumnConfigHook({
  EditorComponent: ChannelLogoEditor,
  dataField: 'user_Tvg_logo',
  fieldType: 'image',
  headerTitle: 'Logo',
  useFilter: false
});
