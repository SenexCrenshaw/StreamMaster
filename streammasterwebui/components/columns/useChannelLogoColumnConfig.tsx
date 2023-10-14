import ChannelLogoEditor from '../ChannelLogoEditor';
import createMultiSelectColumnConfigHook from './createMultiSelectColumnConfigHook';

export const useChannelLogoColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'user_Tvg_logo',
  EditorComponent: ChannelLogoEditor,
  fieldType: 'image',
  headerTitle: 'Logo',
  useFilter: false,
});
