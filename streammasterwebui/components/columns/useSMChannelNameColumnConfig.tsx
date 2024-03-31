import SMChannelNameEditor from '@components/smchannels/SMChannelNameEditor';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelNameColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  EditorComponent: SMChannelNameEditor,
  dataField: 'name',
  headerTitle: 'Name'
});
