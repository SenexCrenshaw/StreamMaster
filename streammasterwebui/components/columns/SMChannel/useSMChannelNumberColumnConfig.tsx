import SMChannelNumberEditor from '@components/smchannels/SMChannelNumberEditor';
import createSMChannelMultiSelectColumnConfigHook from '../createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelNumberColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  dataField: 'ChannelNumber',
  EditorComponent: SMChannelNumberEditor,
  headerTitle: '#',
  maxWidth: 3,
  sortable: true,
  // useFilter: false,
  width: 3
});
