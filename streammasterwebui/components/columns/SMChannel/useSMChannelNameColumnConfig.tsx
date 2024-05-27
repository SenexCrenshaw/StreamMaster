import SMChannelNameEditor from '@components/smchannels/SMChannelNameEditor';
import createSMChannelMultiSelectColumnConfigHook from '../createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelNameColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  dataField: 'Name',
  EditorComponent: SMChannelNameEditor,
  headerTitle: 'NAME',
  sortable: true
});
