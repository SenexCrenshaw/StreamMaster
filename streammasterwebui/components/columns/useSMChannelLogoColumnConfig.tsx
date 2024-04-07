import SMChannelLogoEditor from '@components/smchannels/SMChannelLogoEditor';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelLogoColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  EditorComponent: SMChannelLogoEditor,
  dataField: 'Logo',
  fieldType: 'image',
  headerTitle: '',
  useFilter: false
});
