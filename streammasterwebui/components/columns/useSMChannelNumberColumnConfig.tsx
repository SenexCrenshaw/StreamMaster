import SMChannelNumberEditor from '@components/smchannels/SMChannelNumberEditor';
import createSMChannelMultiSelectColumnConfigHook from './createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelNumberColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  EditorComponent: SMChannelNumberEditor,
  dataField: 'channelNumber',
  headerTitle: 'Ch.',
  useFilter: false,
  width: 4
});
