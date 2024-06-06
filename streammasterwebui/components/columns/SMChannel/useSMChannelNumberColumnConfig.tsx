import SMChannelNumberEditor from '@components/smchannels/SMChannelNumberEditor';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelNumberColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  dataField: 'ChannelNumber',
  EditorComponent: SMChannelNumberEditor,
  headerTitle: '#',
  sortable: true,
  width: '3rem'
});
