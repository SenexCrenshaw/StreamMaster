import SMChannelLogoEditor from '@components/smchannels/SMChannelLogoEditor';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelLogoColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  EditorComponent: SMChannelLogoEditor,
  dataField: 'logo',
  fieldType: 'image',
  headerTitle: 'Logo',
  useFilter: false
});
