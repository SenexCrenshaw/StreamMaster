import SMChannelNameEditor from '@components/smchannels/SMChannelNameEditor';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelNameColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  dataField: 'Name',
  EditorComponent: SMChannelNameEditor,
  headerTitle: 'NAME',
  minWidth: '8',
  sortable: true
});
