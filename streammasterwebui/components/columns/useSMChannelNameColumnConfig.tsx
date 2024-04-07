import SMChannelNameEditor from '@components/smchannels/SMChannelNameEditor';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelNameColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  EditorComponent: SMChannelNameEditor,
  dataField: 'Name',
  headerTitle: 'Name'
});
