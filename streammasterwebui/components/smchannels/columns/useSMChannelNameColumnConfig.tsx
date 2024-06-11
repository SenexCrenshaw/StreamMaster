import SMChannelNameEditor from '@components/smchannels/SMChannelNameEditor';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelNameColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  dataField: 'Name',
  EditorComponent: SMChannelNameEditor,
  headerTitle: 'NAME',
  minWidth: 300,
  sortable: true
});
