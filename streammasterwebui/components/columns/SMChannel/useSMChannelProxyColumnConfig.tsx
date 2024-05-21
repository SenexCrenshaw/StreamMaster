import StreamingProxyTypeSelector from '@components/smchannels/StreamingProxyTypeSelector';
import createSMChannelMultiSelectColumnConfigHook from '../createSMChannelMultiSelectColumnConfigHook';

export const useSMChannelProxyColumnConfig = createSMChannelMultiSelectColumnConfigHook({
  dataField: 'StreamingProxyType',
  EditorComponent: StreamingProxyTypeSelector,
  headerTitle: 'PROXY',
  maxWidth: 10,
  sortable: true,
  width: 10
});
