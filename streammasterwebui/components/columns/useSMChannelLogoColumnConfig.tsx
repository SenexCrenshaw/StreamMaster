import SMChannelLogoEditor from '@components/smchannels/SMChannelLogoEditor';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelLogoColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  dataField: 'Logo',
  EditorComponent: SMChannelLogoEditor,
  fieldType: 'image',
  headerTitle: '',
  sortable: false,
  useFilter: false
});
