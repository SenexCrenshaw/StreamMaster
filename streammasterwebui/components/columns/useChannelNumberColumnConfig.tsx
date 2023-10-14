import ChannelNumberEditor from '../ChannelNumberEditor';
import createMultiSelectColumnConfigHook from './createMultiSelectColumnConfigHook';

export const useChannelNumberColumnConfig = createMultiSelectColumnConfigHook({
  dataField: 'user_Tvg_chno',
  EditorComponent: ChannelNumberEditor,
  headerTitle: 'Ch.',
  useFilter: false,
  width: 4,
});
